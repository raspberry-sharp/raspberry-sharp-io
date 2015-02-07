#region References

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

#endregion

namespace Raspberry.IO.Components.Sensors.Temperature.Ds18b20
{
    /// <summary>
    /// Represents a connection to a DS18B20 temperature sensor.
    /// </summary>
    /// <remarks>See <see cref="https://learn.adafruit.com/adafruits-raspberry-pi-lesson-11-ds18b20-temperature-sensing"/>.</remarks>
    public class Ds18b20Connection : IDisposable
    {
        #region Fields

        private readonly string _deviceFolder;
        private string DeviceFile { get {return _deviceFolder + @"/w1_slave";} }
        private const string BaseDir = @"/sys/bus/w1/devices/";

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Ds18b20Connection"/> class.
        /// </summary>
        public Ds18b20Connection(string deviceId)
        {
            _deviceFolder = BaseDir + deviceId;
            if (!Directory.Exists(_deviceFolder))
            {
                throw new ArgumentException(string.Format("Sensor with Id {0} not found. {1}", deviceId, ModprobeExceptionMessage), deviceId);
            }

        }

        private string ModprobeExceptionMessage
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("Make sure you have loaded the required kernel models.");
                sb.AppendFormat("\tmodprobe w1-gpio{0}", Environment.NewLine);
                sb.AppendFormat("\tmodprobe w1-therm{0}", Environment.NewLine);
                return sb.ToString();
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Ds18b20Connection"/> class.
        /// </summary>
        public Ds18b20Connection(int deviceIndex)
        {
            var deviceFolders = Directory.GetDirectories(BaseDir, "28*").ToList();
            var deviceCount = deviceFolders.Count();
            if (deviceCount == 0)
            {
                throw new InvalidOperationException(string.Format("No sensors were found in {0}. {1}", BaseDir, ModprobeExceptionMessage));
            }

            if (deviceCount <= deviceIndex)
            {
                throw new IndexOutOfRangeException(string.Format("There were only {0} devices found, so index {1} is invalid", deviceCount, deviceIndex ));
            }

            _deviceFolder = deviceFolders[deviceIndex];

        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the temperature, in Celsius.
        /// </summary>
        /// <returns>The temperature, in Celsius.</returns>
        public double GetTemperatureCelsius()
        {
            var lines = File.ReadAllLines(DeviceFile);
            while (!lines[0].Trim().EndsWith("YES"))
            {
                Thread.Sleep(2);
                lines = File.ReadAllLines(DeviceFile);
            }

            var equalsPos = lines[1].IndexOf("t=");
            if (equalsPos == -1)
            {
                throw new Exception("invalid temp reading");
            }

            var temperatureString = lines[1].Substring(equalsPos + 2);
            var tempC = Double.Parse(temperatureString) / 1000.0;
            return tempC;

        }

        /// <summary>
        /// Gets the temperature, in Fahrenheit.
        /// </summary>
        /// <returns>The temperature, in Fahrenheit.</returns>
        public double GetTemperatureFahrenheit()
        {
            var tempC = GetTemperatureCelsius();
            var tempF = tempC * 9.0 / 5.0 + 32.0;
            return tempF;
        }


        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
        }

        #endregion
    }
}
