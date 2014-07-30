using System;
using Raspberry.Timers;

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    /// <summary>
    /// Represents a connection to a DHT-11 or DHT-22 humidity / temperature sensor.
    /// </summary>
    /// <remarks>This is preliminary work and is not working.</remarks>
    public class DhtConnection : IDisposable
    {
        private readonly IInputOutputBinaryPin pin;

        public DhtConnection(IInputOutputBinaryPin pin)
        {
            this.pin = pin;

            pin.Write(true);
            Timer.Sleep(100);
        }

        public void Dispose()
        {
            Close();
        }

        public DhtData GetData()
        {
            // pull pin down for 18ms
            pin.Write(false);
            Timer.Sleep(18m);

            // pull pin up for 40µs
            pin.Write(true);
            Timer.Sleep(40m / 1000m);
            
            pin.Wait(true, 40m / 1000m);

            // Wait for pin to be down (at most 80µs)
            pin.Wait(false, 80m / 1000m);

            // Wait for pin to be up (at most 80µs)
            pin.Wait(true, 80m / 1000m);

            var data = new byte[]{0,0,0,0,0};
            for (var i = 0; i < 5; i++)
            for (var j = 0; j < 8; j++)
            {
                pin.Wait(false, 50m / 1000m);

                var start = DateTime.UtcNow;
                pin.Wait(true, 100m / 1000m);

                //  bit "0" has 26-28us high-voltage length, bit "1" has 70us high-voltage length
                var value = (DateTime.UtcNow - start).TotalMilliseconds > 40.0/1000 ? 1 : 0;

                data[i] = (byte) (data[i] << 1 + value);
            }

            Console.WriteLine(
                "{0:X2} {1:X2} {2:X2} {3:X2} {4:X2}",
                data[0], 
                data[1],
                data[2],
                data[3],
                data[4]);

            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
                throw new InvalidOperationException("Checksum is not valid");

            pin.Write(true);

            return new DhtData
            {
                Humidity = (data[0] << 8 + data[1])/10.0m,
                Temperature = ((data[2] & 0x80) == 0 ? 1 : -1)*((data[2] & 0x7F) << 8 + data[3])/10.0m
            };
        }

        public void Close()
        {
            pin.Dispose();
        }
    }
}
