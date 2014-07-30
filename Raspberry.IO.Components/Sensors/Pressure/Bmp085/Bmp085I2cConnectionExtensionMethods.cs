#region References

using System;

#endregion

namespace Raspberry.IO.Components.Sensors.Pressure.Bmp085
{
    /// <summary>
    /// Provides extension methods for <see cref="Bmp085I2cConnection"/>.
    /// </summary>
    public static class Bmp085I2cConnectionExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Gets the sea-level pressure.
        /// </summary>
        /// <param name="connection">The BMP085 connection.</param>
        /// <param name="currentAltitude">The current altitude, in meters.</param>
        /// <returns>The pressure, in hPa.</returns>
        public static decimal GetSealevelPressure(this Bmp085I2cConnection connection, decimal currentAltitude)
        {
            var pressure = connection.GetPressure();
            return (decimal) ((double) pressure/Math.Pow(1.0 - (double) currentAltitude/44330, 5.255));
        }

        /// <summary>
        /// Gets the altitude.
        /// </summary>
        /// <param name="connection">The BMP085 connection.</param>
        /// <param name="sealevelPressure">The sealevel pressure, in hPa.</param>
        /// <returns>The altitude, in meters</returns>
        public static decimal GetAltitude(this Bmp085I2cConnection connection, decimal sealevelPressure)
        {
            var pressure = connection.GetPressure();
            return (decimal) (44330*(1.0 - Math.Pow((double) (pressure/sealevelPressure), 0.1903)));
        }

        #endregion
    }
}