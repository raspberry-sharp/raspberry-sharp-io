#region References

using System;
using System.Linq;
using Raspberry.IO.Interop;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// Native SPI connection that communicates with Linux's userspace SPI driver (e.g. /dev/spidev0.0) using IOCTL.
    /// </summary>
    public class NativeSpiConnection : INativeSpiConnection
    {
        #region Constants
        internal const UInt32 SPI_IOC_RD_MODE = 0x80016b01;
        internal const UInt32 SPI_IOC_WR_MODE = 0x40016b01;
        internal const UInt32 SPI_IOC_RD_LSB_FIRST = 0x80016b02;
        internal const UInt32 SPI_IOC_WR_LSB_FIRST = 0x40016b02;
        internal const UInt32 SPI_IOC_RD_BITS_PER_WORD = 0x80016b03;
        internal const UInt32 SPI_IOC_WR_BITS_PER_WORD = 0x40016b03;
        internal const UInt32 SPI_IOC_RD_MAX_SPEED_HZ = 0x80046b04;
        internal const UInt32 SPI_IOC_WR_MAX_SPEED_HZ = 0x40046b04;
        #endregion

        #region Fields
        private readonly ISpiControlDevice deviceFile;
        private UInt16 delay;
        private UInt32 maxSpeed;
        private UInt32 mode;
        private byte bitsPerWord;
        #endregion

        #region Instance Management

        /// <summary>
        /// Creates a new instance of the <see cref="NativeSpiConnection"/> class.
        /// </summary>
        /// <param name="deviceFile">A control device (IOCTL) to the device file (e.g. /dev/spidev0.0).</param>
        public NativeSpiConnection(ISpiControlDevice deviceFile) {
            this.deviceFile = deviceFile;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NativeSpiConnection"/> class and initializes it.
        /// </summary>
        /// <param name="deviceFile">A control device (IOCTL) to the device file (e.g. /dev/spidev0.0).</param>
        /// <param name="settings">Connection settings</param>
        public NativeSpiConnection(ISpiControlDevice deviceFile, SpiConnectionSettings settings)
            : this(deviceFile)
        {
            Init(settings);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="NativeSpiConnection"/> class and initializes it.
        /// </summary>
        /// <param name="deviceFilePath">Full path to the SPI device file (e.g. /dev/spidev0.0).</param>
        /// <param name="settings">Connection settings</param>
        public NativeSpiConnection(string deviceFilePath, SpiConnectionSettings settings) 
            : this(new SpiControlDevice(new UnixFile(deviceFilePath, UnixFileMode.ReadWrite)), settings)
        {}

        /// <summary>
        /// Creates a new instance of the <see cref="NativeSpiConnection"/> class.
        /// </summary>
        /// <param name="deviceFilePath">Full path to the SPI device file (e.g. /dev/spidev0.0).</param>
        public NativeSpiConnection(string deviceFilePath)
            : this(new SpiControlDevice(new UnixFile(deviceFilePath, UnixFileMode.ReadWrite))) 
        {}

        /// <summary>
        /// Dispose instance and free all resources.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="disposing">If <c>true</c> all managed resources will be disposed</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                deviceFile.Dispose();
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        public UInt16 Delay {
            get { return delay; }
        }

        /// <summary>
        /// Maximum clock speed in Hz.
        /// </summary>
        public UInt32 MaxSpeed {
            get { return maxSpeed; }
        }

        /// <summary>
        /// SPI mode
        /// </summary>
        public SpiMode Mode {
            get { return (SpiMode)mode; }
        }

        /// <summary>
        /// The device's wordsize
        /// </summary>
        public byte BitsPerWord {
            get { return bitsPerWord; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Sets the <see cref="Delay"/>.
        /// </summary>
        /// <param name="delayInMicroSeconds">Delay in µsec.</param>
        public void SetDelay(UInt16 delayInMicroSeconds) {
            delay = delayInMicroSeconds;
        }

        /// <summary>
        /// Sets the maximum clock speed.
        /// </summary>
        /// <param name="maxSpeedInHz">The speed in Hz</param>
        public void SetMaxSpeed(UInt32 maxSpeedInHz) {
            maxSpeed = maxSpeedInHz;
            deviceFile.Control(SPI_IOC_WR_MAX_SPEED_HZ, ref maxSpeedInHz)
                .ThrowOnPInvokeError<SetMaxSpeedException>("Can't set max speed in HZ (SPI_IOC_WR_MAX_SPEED_HZ). Error {1}: {2}");
            deviceFile.Control(SPI_IOC_RD_MAX_SPEED_HZ, ref maxSpeedInHz)
                .ThrowOnPInvokeError<SetMaxSpeedException>("Can't set max speed in HZ (SPI_IOC_RD_MAX_SPEED_HZ). Error {1}: {2}");
        }

        /// <summary>
        /// Sets the device's wordsize <see cref="BitsPerWord"/>.
        /// </summary>
        /// <param name="wordSize">Bits per word</param>
        public void SetBitsPerWord(byte wordSize) {
            bitsPerWord = wordSize;
            deviceFile.Control(SPI_IOC_WR_BITS_PER_WORD, ref wordSize)
                .ThrowOnPInvokeError<SetBitsPerWordException>("Can't set bits per word (SPI_IOC_WR_BITS_PER_WORD). Error {1}: {2}");
            deviceFile.Control(SPI_IOC_RD_BITS_PER_WORD, ref wordSize)
                .ThrowOnPInvokeError<SetBitsPerWordException>("Can't set bits per word (SPI_IOC_RD_BITS_PER_WORD). Error {1}: {2}");
        }

        /// <summary>
        /// Sets the <see cref="SpiMode"/>.
        /// </summary>
        /// <param name="spiMode">SPI mode</param>
        public void SetSpiMode(SpiMode spiMode) {
            mode = (UInt32) spiMode;
            deviceFile.Control(SPI_IOC_WR_MODE, ref mode)
                .ThrowOnPInvokeError<SetSpiModeException>("Can't set SPI mode (SPI_IOC_WR_MODE). Error {1}: {2}");
            deviceFile.Control(SPI_IOC_RD_MODE, ref mode)
                .ThrowOnPInvokeError<SetSpiModeException>("Can't set SPI mode (SPI_IOC_RD_MODE). Error {1}: {2}");
        }

        /// <summary>
        /// Creates a transfer buffer of the given <see paramref="sizeInBytes"/> and copies the connection settings to it.
        /// </summary>
        /// <param name="sizeInBytes">Memory size in bytes.</param>
        /// <param name="transferMode">The transfer mode.</param>
        /// <returns>The requested transfer buffer.</returns>
        public ISpiTransferBuffer CreateTransferBuffer(int sizeInBytes, SpiTransferMode transferMode) {
            return new SpiTransferBuffer(sizeInBytes, transferMode) {
                BitsPerWord = bitsPerWord,
                Delay = delay,
                Speed = maxSpeed
            };
        }

        /// <summary>
        /// Creates transfer buffers for <paramref name="numberOfMessages"/>.
        /// </summary>
        /// <param name="numberOfMessages">The number of messages to send.</param>
        /// <param name="messageSizeInBytes">Message size in bytes.</param>
        /// <param name="transferMode">The transfer mode.</param>
        /// <returns>The requested transfer buffer collection.</returns>
        public ISpiTransferBufferCollection CreateTransferBufferCollection(int numberOfMessages, int messageSizeInBytes, SpiTransferMode transferMode) {
            var collection = new SpiTransferBufferCollection(numberOfMessages, messageSizeInBytes, transferMode);
            foreach (var transferBuffer in collection) {
                transferBuffer.BitsPerWord = bitsPerWord;
                transferBuffer.Delay = delay;
                transferBuffer.Speed = maxSpeed;
            }
            return collection;
        }

        /// <summary>
        /// Starts the SPI data transfer.
        /// </summary>
        /// <param name="buffer">The transfer buffer that contains data to be send and/or the received data.</param>
        /// <returns>An <see cref="int"/> that contains the result of the transfer operation.</returns>
        public int Transfer(ISpiTransferBuffer buffer) {
            if (buffer == null) {
                throw new ArgumentNullException("buffer");
            }

            var request = Interop.GetSpiMessageRequest(1);
            var structure = buffer.ControlStructure;
            var result = deviceFile.Control(request, ref structure);
          
            result.ThrowOnPInvokeError<SendSpiMessageException>("Can't send SPI message. Error {1}: {2}");
            
            return result;
        }

        /// <summary>
        /// Starts the SPI data transfer.
        /// </summary>
        /// <param name="transferBuffers">The transfer buffers that contain data to be send and/or the received data.</param>
        /// <returns>An <see cref="int"/> that contains the result of the transfer operation.</returns>
        public int Transfer(ISpiTransferBufferCollection transferBuffers) {
            if (transferBuffers == null) {
                throw new ArgumentNullException("transferBuffers");
            }

            var request = Interop.GetSpiMessageRequest(transferBuffers.Length);

            var structures = transferBuffers
                .Select(buf => buf.ControlStructure)
                .ToArray();
            var result = deviceFile.Control(request, structures);

            result.ThrowOnPInvokeError<SendSpiMessageException>("Can't send SPI messages. Error {1}: {2}");

            return result;
        }

        #endregion

        #region Private Helpers
        private void Init(SpiConnectionSettings settings) {
            SetSpiMode(settings.Mode);
            SetBitsPerWord(settings.BitsPerWord);
            SetMaxSpeed(settings.MaxSpeed);
            SetDelay(settings.Delay);
        }
        #endregion
    }
}