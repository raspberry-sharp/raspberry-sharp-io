namespace Raspberry.IO.Components.Sensors.Pressure.Bmp085
{
    /// <summary>
    /// Represents data from a <see cref="Bmp085I2cConnection"/>.
    /// </summary>
    public struct Bmp085Data
    {
        public UnitsNet.Temperature Temperature;
        public UnitsNet.Pressure Pressure;
    }
}