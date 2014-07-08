#region References

using System;
using Raspberry.IO.Interop;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A transfer buffer used to read from / write to the SPI bus.
    /// </summary>
    public class SpiTransferBuffer : ISpiTransferBuffer
    {
        #region Fields
        private readonly IMemory txBuf;
        private readonly IMemory rxBuf;

        private readonly SpiTransferMode mode;
        private SpiTransferControlStructure control;

        #endregion

        #region Properties

        /// <summary>
        /// Temporary override of the device's wordsize
        /// </summary>
        public byte BitsPerWord {
            get { return control.BitsPerWord; }
            set { control.BitsPerWord = value; }
        }

        /// <summary>
        /// Temporary override of the device's bitrate (in Hz)
        /// </summary>
        public UInt32 Speed {
            get { return control.Speed; }
            set { control.Speed = value; }
        }

        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        public UInt16 Delay {
            get { return control.Delay; }
            set { control.Delay = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to deselect device before starting the next transfer.
        /// </summary>
        public bool ChipSelectChange {
            get { return control.ChipSelectChange == 1; }
            set {
                control.ChipSelectChange = value
                    ? (byte) 1
                    : (byte) 0;
            }
        }

        /// <summary>
        /// Pad
        /// </summary>
        public UInt32 Pad {
            get { return control.Pad; }
            set { control.Pad = value; }
        }

        /// <summary>
        /// Specifies if the transfer shall read and/or write. <see cref="SpiTransferMode"/>
        /// </summary>
        public SpiTransferMode TransferMode {
            get { return mode; }
        }

        /// <summary>
        /// Length of <see cref="Tx"/> and <see cref="Rx"/> buffers, in bytes
        /// </summary>
        public int Length {
            get { return unchecked((int)control.Length); }
        }

        /// <summary>
        /// Holds pointer to userspace buffer with transmit data, or <c>null</c>. If no data is provided, zeroes are shifted out
        /// </summary>
        public IMemory Tx {
            get { return txBuf; }
        }

        /// <summary>
        /// Holds pointer to userspace buffer for receive data, or <c>null</c>
        /// </summary>
        public IMemory Rx {
            get { return rxBuf; }
        }

        /// <summary>
        /// The IOCTL structure that contains control information for a single SPI transfer
        /// </summary>
        public SpiTransferControlStructure ControlStructure {
            get { return control; }
        }

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiTransferBuffer"/> class.
        /// </summary>
        /// <param name="lengthInBytes">Size of data that shall be transmitted.</param>
        /// <param name="transferMode">Specifies read and/or write mode.</param>
        public SpiTransferBuffer(int lengthInBytes, SpiTransferMode transferMode) {
            if (transferMode == 0) {
                throw new ArgumentException("An appropriate transfer mode must be specified (read/write)", "transferMode");
            }

            mode = transferMode;

            if ((TransferMode & SpiTransferMode.Write) == SpiTransferMode.Write) {
                txBuf = new ManagedMemory(lengthInBytes);
            }
            if ((TransferMode & SpiTransferMode.Read) == SpiTransferMode.Read) {
                rxBuf = new ManagedMemory(lengthInBytes);
            }

            var txPtr = ReferenceEquals(Tx, null)
                ? 0
                : unchecked((UInt64) Tx.Pointer.ToInt64());

            var rxPtr = ReferenceEquals(Rx, null)
                ? 0
                : unchecked((UInt64) Rx.Pointer.ToInt64());

            control.Length = unchecked((uint)lengthInBytes);
            control.Tx = txPtr;
            control.Rx = rxPtr;
        }

        /// <summary>
        /// Finalizer for <see cref="SpiTransferBuffer"/>
        /// </summary>
        ~SpiTransferBuffer() {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release all unmanaged memory.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        /// <param name="disposing">The memory will always be released to avoid memory leaks. If you don't want this, don't call this method (<see cref="Dispose(bool)"/>) in your derived class.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // free managed stuff here
            }

            control.Rx = 0;
            control.Tx = 0;
            control.Length = 0;

            // always free managed/unmanaged memory
            if (!ReferenceEquals(txBuf, null)) {
                txBuf.Dispose();
            }
            if (!ReferenceEquals(rxBuf, null)) {
                rxBuf.Dispose();
            }
        }

        #endregion
    }
}