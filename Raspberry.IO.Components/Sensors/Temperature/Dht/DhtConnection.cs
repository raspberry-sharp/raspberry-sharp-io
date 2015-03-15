#region References

using System;
using System.Globalization;
using Common.Logging;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;
using UnitsNet;

#endregion

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    /// <summary>
    /// Represents a connection to a DHT-11 or DHT humidity / temperature sensor.
    /// </summary>
    /// <remarks>
    /// Requires a fast input/output switch (such as <see cref="MemoryGpioConnectionDriver"/>).
    /// Based on <see href="https://www.virtuabotix.com/virtuabotix-dht22-pinout-coding-guide/"/>, <see cref="https://github.com/RobTillaart/Arduino/tree/master/libraries/DHTlib"/>
    /// Datasheet : <see cref="http://www.micropik.com/PDF/dht11.pdf"/>.
    /// </remarks>
    public class DhtConnection : IDisposable
    {
        #region Fields

        private readonly IInputOutputBinaryPin pin;
        
        private DateTime previousRead;
        private bool started;
        
        private static readonly TimeSpan timeout = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// The minimum sampling interval (1s).
        /// </summary>
        public static readonly TimeSpan MinimumSamplingInterval = TimeSpan.FromSeconds(1);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="DhtConnection" /> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="autoStart">if set to <c>true</c>, DHT is automatically started. Default value is <c>true</c>.</param>
        public DhtConnection(IInputOutputBinaryPin pin, bool autoStart = true)
        {
            this.pin = pin;

            if (autoStart)
                Start();
            else
                Stop(); 
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Close();
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
            pin.Dispose();
        }

        #endregion

        #region Private Helpers

        private DhtData TryGetData()
        {
            // Prepare buffer
            var data = new byte[5];
            for (var i = 0; i < 5; i++)
                data[i] = 0;

            var remainingSamplingInterval = MinimumSamplingInterval - (DateTime.UtcNow - previousRead);
            if (remainingSamplingInterval > TimeSpan.Zero)
                HighResolutionTimer.Sleep((int)remainingSamplingInterval.TotalMilliseconds);

            // Measure required by host : 18ms down then put to up
            pin.Write(false);
            HighResolutionTimer.Sleep(18m);
            pin.Write(true);

            // Prepare for reading
            try
            {
                // Read acknowledgement from DHT
                pin.Wait(true, timeout);
                pin.Wait(false, timeout);

                // Read 40 bits output, or time-out
                var cnt = 7;
                var idx = 0;
                for (var i = 0; i < 40; i++)
                {
                    pin.Wait(true, timeout);
                    var start = DateTime.UtcNow.Ticks;
                    pin.Wait(false, timeout);
                    var ticks = (DateTime.UtcNow.Ticks - start);
                    if (ticks > 400)
                        data[idx] |= (byte) (1 << cnt);

                    if (cnt == 0)
                    {
                        idx++; // next byte
                        cnt = 7; // restart at MSB
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

            // these bits are always zero, masking them reduces errors. 
            data[0] &= 0x7F;
            data[2] &= 0x7F;

            var humidity = data[0];     // data[1] always 0; 
            var temperature = data[2];  // data[3] always 0; 

            var checkSum = data[0] + data[2]; // data[1] and data[3] always 0 
            if (checkSum != data[4])
                throw new InvalidChecksumException("Invalid checksum on DHT data", data[4], checkSum);

            /*
            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
                throw new InvalidChecksumException("Invalid checksum on DHT data", data[4], (checkSum & 0xff));

            var humidity = ((data[0] << 8) + data[1])/256m;

            var sign = 1;
            if ((data[2] & 0x80) != 0) // negative temperature
            {
                data[2] = (byte) (data[2] & 0x7F);
                sign = -1;
            }
            var temperature = sign * ((data[2] << 8) + data[3])/256m;
            */

            return new DhtData
            {
                RelativeHumidity = Ratio.FromPercent((double)humidity),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius((double)temperature)
            };
        }

        #endregion
    }
}