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

        /// <summary>
        /// Gets the device address.
        /// </summary>
        /// <value>
        /// The device address.
        /// </value>
        public int DeviceAddress
        {
            get { return deviceAddress; }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void Write(params byte[] buffer)
        {
            driver.Write(deviceAddress, buffer);
        }

        /// <summary>
        /// Writes the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteByte(byte value)
        {
            Write(value);
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns>The buffer.</returns>
        public byte[] Read(int byteCount)
        {
            return driver.Read(deviceAddress, byteCount);
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns>The byte.</returns>
        public byte ReadByte()
        {
            return Read(1)[0];
        }

        #endregion
    }
}