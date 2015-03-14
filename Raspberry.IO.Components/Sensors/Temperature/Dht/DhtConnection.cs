#region References

using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;
using UnitsNet;

#endregion

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    /// <summary>
    /// Represents a connection to a DHT-11 or DHT-22 humidity / temperature sensor.
    /// </summary>
    /// <remarks>
    /// Requires a fast IO connection (such as <see cref="MemoryGpioConnectionDriver"/>).
    /// Based on <see href="https://www.virtuabotix.com/virtuabotix-dht22-pinout-coding-guide/"/>.
    /// </remarks>
    public class DhtConnection : IDisposable
    {
        #region Fields

        private readonly IInputOutputBinaryPin pin;
        
        private static TimeSpan timeout = TimeSpan.FromMilliseconds(100);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="DhtConnection"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public DhtConnection(IInputOutputBinaryPin pin)
        {
            this.pin = pin;
            pin.AsOutput();
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
        /// Gets the data.
        /// </summary>
        /// <returns>The DHT data.</returns>
        public DhtData GetData()
        {
            DhtData data = null;
            var retryCount = 10;
            while (data == null && retryCount-- > 0)
            {
                try
                {
                    data = TryGetData();
                }
                catch
                {
                    data = null;
                }
            }

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

            pin.Write(true);
            HighResolutionTimer.Sleep(100m);

            // Measure required by host : 18ms down then put to up
            pin.Write(false);
            HighResolutionTimer.Sleep(18m);
            pin.Write(true);

            // Prepare for reading
            pin.AsInput();
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
                pin.Write(true);
            }

            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
                return null;

            var humidity = ((data[0] << 8) + data[1])/256m;

            var sign = 1;
            if ((data[2] & 0x80) != 0) // negative temperature
            {
                data[2] = (byte) (data[2] & 0x7F);
                sign = -1;
            }
            var temperature = sign*((data[2] << 8) + data[3])/256m;

            return new DhtData
            {
                RelativeHumidity = Ratio.FromPercent((double)humidity),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius((double)temperature)
            };
        }

        #endregion
    }
}