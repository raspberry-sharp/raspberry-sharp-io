namespace Raspberry.IO.InterIntegratedCircuit
{
    using System;

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
        /// Executes the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public void Execute(I2cTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            driver.Execute(deviceAddress, transaction);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void Write(params byte[] buffer)
        {
            Execute(new I2cTransaction(new I2cWriteAction(buffer)));
        }

        /// <summary>
        /// Writes the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteByte(byte value)
        {
            Execute(new I2cTransaction(new I2cWriteAction(value)));
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns>The buffer.</returns>
        public byte[] Read(int byteCount)
        {
            var readAction = new I2cReadAction(new byte[byteCount]);
            Execute(new I2cTransaction(readAction));

            return readAction.Buffer;
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