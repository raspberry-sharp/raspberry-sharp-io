#region References

using System;
using Raspberry.IO.SerialPeripheralInterface;

#endregion

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// A connection the one or more TLC59711 devices.
    /// </summary>
    public class Tlc59711Connection : ITlc59711Connection
    {
        #region Constants
        private const int BITS_PER_WORD = 8;
        private const SpiMode SPI_MODE_0 = SpiMode.Mode0;
        private const int SPEED = 10000000; // Spec say max 10Mhz (RaspberryPI will only use about ~7Mhz)
        private const int DELAY = 20;
        #endregion

        #region Fields
        private readonly INativeSpiConnection connection;
        private readonly ISpiTransferBuffer transferBuffer;
        private readonly Tlc59711Cluster deviceCluster;
        #endregion

        #region Instance Management

        /// <summary>
        /// Creates a new instance of the <see cref="Tlc59711Connection"/> class and initializes it.
        /// </summary>
        /// <param name="connection">A open SPI connection</param>
        /// <param name="initializeWithDefault">If <c>true</c> the SPI connection will be initialized with common data transfers settings.</param>
        /// <param name="numberOfDevices">Number of <see cref="ITlc59711Device"/>s connected together.</param>
        public Tlc59711Connection(INativeSpiConnection connection, bool initializeWithDefault, int numberOfDevices) {
            if (ReferenceEquals(connection, null))
                throw new ArgumentNullException("connection");

            if (numberOfDevices <= 0)
                throw new ArgumentOutOfRangeException("numberOfDevices", "You need at least one device.");

            this.connection = connection;
            
            if (initializeWithDefault) {
                connection.SetBitsPerWord(BITS_PER_WORD);
                connection.SetSpiMode(SPI_MODE_0);
                connection.SetMaxSpeed(SPEED);
                connection.SetDelay(DELAY);
            }

            var requiredMemorySize = numberOfDevices * Tlc59711Device.COMMAND_SIZE;
            transferBuffer = connection.CreateTransferBuffer(requiredMemorySize, SpiTransferMode.Write);

            deviceCluster = new Tlc59711Cluster(transferBuffer.Tx, numberOfDevices);
        }

        /// <summary>
        /// Releases all managed resources. The SPI connection will be closed.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all managed resources.
        /// </summary>
        /// <param name="disposing">If <c>true</c>, all managed resources including the SPI connection will be released/closed.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                transferBuffer.Dispose();
                connection.Dispose();
            }
        }

        #endregion

        #region Properties 
        /// <summary>
        /// A chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711. 
        /// </summary>
        public ITlc59711Cluster Devices {
            get { return deviceCluster; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a TLC59711 command and sends it to the first device using the SPI bus.
        /// </summary>
        public void Update() {
            connection.Transfer(transferBuffer);
        }
        #endregion
    }
}