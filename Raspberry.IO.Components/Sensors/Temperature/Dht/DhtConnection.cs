#region References

using System;
using System.Globalization;
using Common.Logging;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    /// <summary>
    /// Represents a base class for connections to a DHT-11 or DHT-22 humidity / temperature sensor.
    /// </summary>
    /// <remarks>
    /// Requires a fast input/output switch (such as <see cref="MemoryGpioConnectionDriver"/>).
    /// Based on <see href="https://www.virtuabotix.com/virtuabotix-dht22-pinout-coding-guide/"/>, <see cref="https://github.com/RobTillaart/Arduino/tree/master/libraries/DHTlib"/>
    /// Datasheet : <see cref="http://www.micropik.com/PDF/dht11.pdf"/>.
    /// </remarks>
    public abstract class DhtConnection : IDisposable
    {
        #region Fields

        private readonly IInputOutputBinaryPin pin;
        private TimeSpan samplingInterval;
        
        private DateTime previousRead;
        private bool started;

        private static readonly TimeSpan timeout = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan bitSetUptime = new TimeSpan(10 * (26 +70) / 2); // 26µs for "0", 70µs for "1"

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="DhtConnection" /> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="autoStart">if set to <c>true</c>, DHT is automatically started. Default value is <c>true</c>.</param>
        protected DhtConnection(IInputOutputBinaryPin pin, bool autoStart = true)
        {
            this.pin = pin;

            if (autoStart)
                Start();
            else
                Stop(); 
        }

        ~DhtConnection()
        {
            Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sampling interval.
        /// </summary>
        /// <value>
        /// The sampling interval.
        /// </value>
        public TimeSpan SamplingInterval
        {
            get { return samplingInterval != TimeSpan.Zero ? samplingInterval : DefaultSamplingInterval; }
            set { samplingInterval = value; }
        }

        #endregion

        #region Method

        /// <summary>
        /// Starts the DHT sensor. If not called, sensor will be automatically enabled before getting data.
        /// </summary>
        public void Start()
        {
            started = true;
            pin.Write(true);
            previousRead = DateTime.UtcNow;
        }

        /// <summary>
        /// Stops the DHT sensor. If not called, sensor will be automatically disabled after getting data.
        /// </summary>
        public void Stop()
        {
            pin.Write(false);
            started = false;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The DHT data.</returns>
        public DhtData GetData()
        {
            if (!started)
            {
                pin.Write(true);
                previousRead = DateTime.UtcNow;
            }

            DhtData data = null;
            var tryCount = 0;
            while (data == null && tryCount++ <= 10)
            {
                try
                {
                    data = TryGetData();
                    data.AttemptCount = tryCount;
                }
                catch(Exception ex)
                {
                    var logger = LogManager.GetLogger<DhtConnection>();
                    logger.Error(
                        CultureInfo.InvariantCulture,
                        h => h("Failed to read data from DHT11, try {0}", tryCount), 
                        ex);
                }
            }

            if (!started)
                pin.Write(false);
        
            return data;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            GC.SuppressFinalize(this);
            pin.Dispose();
        }

        #endregion

        #region Protected Methods

        protected abstract DhtData GetDhtData(int temperatureValue, int humidityValue);

        protected abstract TimeSpan DefaultSamplingInterval { get; }

        protected abstract TimeSpan WakeupInterval { get; }

        #endregion

        #region Private Helpers

        private DhtData TryGetData()
        {
            // Prepare buffer
            var data = new byte[5];
            for (var i = 0; i < 5; i++)
                data[i] = 0;

            var remainingSamplingInterval = SamplingInterval - (DateTime.UtcNow - previousRead);
            if (remainingSamplingInterval > TimeSpan.Zero)
                HighResolutionTimer.Sleep(remainingSamplingInterval);

            // Prepare for reading
            try
            {
                // Measure required by host : pull down then pull up
                pin.Write(false);
                HighResolutionTimer.Sleep(WakeupInterval);
                pin.Write(true);

                // Read acknowledgement from DHT
                pin.Wait(true, timeout);
                pin.Wait(false, timeout);

                // Read 40 bits output, or time-out
                var cnt = 7;
                var idx = 0;
                for (var i = 0; i < 40; i++)
                {
                    pin.Wait(true, timeout);
                    var start = DateTime.UtcNow;
                    pin.Wait(false, timeout);

                    // Determine whether bit is "1" or "0"
                    if (DateTime.UtcNow - start > bitSetUptime)
                        data[idx] |= (byte)(1 << cnt);

                    if (cnt == 0)
                    {
                        idx++;      // next byte
                        cnt = 7;    // restart at MSB
                    }
                    else
                        cnt--;
                }
            }
            finally
            {
                // Prepare for next reading
                previousRead = DateTime.UtcNow;
                pin.Write(true);
            }

            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
                throw new InvalidChecksumException("Invalid checksum on DHT data", data[4], (checkSum & 0xff));

            var sign = 1;
            if ((data[2] & 0x80) != 0) // negative temperature
            {
                data[2] = (byte)(data[2] & 0x7F);
                sign = -1;
            }

            var humidity = (data[0] << 8) + data[1];
            var temperature = sign * ((data[2] << 8) + data[3]);

            return GetDhtData(temperature, humidity);
        }

        #endregion
    }
}