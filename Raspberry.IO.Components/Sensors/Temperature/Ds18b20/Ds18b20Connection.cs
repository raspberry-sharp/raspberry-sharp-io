#region References

using System;
using System.Globalization;
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

        private const string baseDir = @"/sys/bus/w1/devices/";

        private readonly string deviceFolder;
        private string deviceFile { get {return deviceFolder + @"/w1_slave";} }

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Ds18b20Connection"/> class.
        /// </summary>
        public Ds18b20Connection(string deviceId)
        {
            deviceFolder = baseDir + deviceId;
            if (!Directory.Exists(deviceFolder))
                throw new ArgumentException(string.Format("Sensor with Id {0} not found. {1}", deviceId, ModprobeExceptionMessage), deviceId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ds18b20Connection"/> class.
        /// </summary>
        public Ds18b20Connection(int deviceIndex)
        {
            var deviceFolders = Directory.GetDirectories(baseDir, "28*").ToList();
            var deviceCount = deviceFolders.Count();
            if (deviceCount == 0)
                throw new InvalidOperationException(string.Format("No sensors were found in {0}. {1}", baseDir, ModprobeExceptionMessage));

            if (deviceCount <= deviceIndex)
                throw new IndexOutOfRangeException(string.Format("There were only {0} devices found, so index {1} is invalid", deviceCount, deviceIndex));

            deviceFolder = deviceFolders[deviceIndex];
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
        /// Gets the temperature.
        /// </summary>
        /// <returns>The temperature.</returns>
        public UnitsNet.Temperature GetTemperature()
        {
            var lines = File.ReadAllLines(deviceFile);
            while (!lines[0].Trim().EndsWith("YES"))
            {
                Thread.Sleep(2);
                lines = File.ReadAllLines(deviceFile);
            }

            var equalsPos = lines[1].IndexOf("t=", StringComparison.InvariantCultureIgnoreCase);
            if (equalsPos == -1)
                throw new InvalidOperationException("Unable to read temperature");

            var temperatureString = lines[1].Substring(equalsPos + 2);
            
            return UnitsNet.Temperature.FromDegreesCelsius(double.Parse(temperatureString, CultureInfo.InvariantCulture) / 1000.0);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
        }

        #endregion

        #region Private Helpers

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

        #endregion
    }
}
