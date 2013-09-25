namespace Raspberry.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Represents a connection to the I2C device.
    /// </summary>
    public class I2cDeviceConnection
    {
        #region Fields

        private readonly I2cDriver driver;
        private readonly int deviceAddress;

        #endregion

        #region Instance Management

        internal I2cDeviceConnection(I2cDriver driver, int deviceAddress)
        {
            this.driver = driver;
            this.deviceAddress = deviceAddress;
        }

        #endregion

        #region Properties

        public int DeviceAddress
        {
            get { return deviceAddress; }
        }


        #endregion

        #region Methods

        public void Write(params byte[] buffer)
        {
            driver.Write(deviceAddress, buffer);
        }

        public void WriteByte(byte value)
        {
            Write(value);
        }

        public byte[] Read(int byteCount)
        {
            return driver.Read(deviceAddress, byteCount);
        }

        public byte ReadByte()
        {
            return Read(1)[0];
        }

        #endregion
    }
}