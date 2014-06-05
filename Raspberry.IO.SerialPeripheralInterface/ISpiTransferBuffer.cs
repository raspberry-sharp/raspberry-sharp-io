#region References

using System;
using Raspberry.IO.Interop;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A transfer buffer used to read from / write to the SPI bus.
    /// </summary>
    public interface ISpiTransferBuffer : IDisposable
    {
        #region Properties

        /// <summary>
        /// Temporary override of the device's wordsize
        /// </summary>
        /// <value>
        /// The bits per word.
        /// </value>
        byte BitsPerWord { get; set; }

        /// <summary>
        /// Temporary override of the device's bitrate (in Hz)
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        UInt32 Speed { get; set; }

        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        /// <value>
        /// The delay.
        /// </value>
        UInt16 Delay { get; set; }

        /// <summary>
        /// Set to <c>true</c> to deselect device before starting the next transfer.
        /// </summary>
        /// <value>
        ///   <c>true</c> if device is delected before starting next transfer; otherwise, <c>false</c>.
        /// </value>
        bool ChipSelectChange { get; set; }

        /// <summary>
        /// Pad.
        /// </summary>
        /// <value>
        /// The pad.
        /// </value>
        UInt32 Pad { get; set; }

        /// <summary>
        /// Specifies if the transfer shall read and/or write. <see cref="SpiTransferMode" />
        /// </summary>
        /// <value>
        /// The transfer mode.
        /// </value>
        SpiTransferMode TransferMode { get; }

        /// <summary>
        /// Length of <see cref="Tx" /> and <see cref="Rx" /> buffers, in bytes
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        int Length { get; }

        /// <summary>
        /// Holds pointer to userspace buffer with transmit data, or <c>null</c>. If no data is provided, zeroes are shifted out
        /// </summary>
        /// <value>
        /// The tx.
        /// </value>
        IMemory Tx { get; }

        /// <summary>
        /// Holds pointer to userspace buffer for receive data, or <c>null</c>
        /// </summary>
        /// <value>
        /// The rx.
        /// </value>
        IMemory Rx { get; }

        /// <summary>
        /// The IOCTL structure that contains control information for a single SPI transfer
        /// </summary>
        /// <value>
        /// The control structure.
        /// </value>
        SpiTransferControlStructure ControlStructure { get; }

        #endregion

    }
}