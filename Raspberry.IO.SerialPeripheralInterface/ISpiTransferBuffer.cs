using System;
using Raspberry.IO.Interop;

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A transfer buffer used to read from / write to the SPI bus.
    /// </summary>
    public interface ISpiTransferBuffer : IDisposable
    {
        /// <summary>
        /// Temporary override of the device's wordsize
        /// </summary>
        byte BitsPerWord { get; set; }
        /// <summary>
        /// Temporary override of the device's bitrate (in Hz)
        /// </summary>
        UInt32 Speed { get; set; }
        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer.
        /// </summary>
        UInt16 Delay { get; set; }
        /// <summary>
        /// Set to <c>true</c> to deselect device before starting the next transfer.
        /// </summary>
        bool ChipSelectChange { get; set; }
        /// <summary>
        /// Pad
        /// </summary>
        UInt32 Pad { get; set; }
        /// <summary>
        /// Specifies if the transfer shall read and/or write. <see cref="SpiTransferMode"/>
        /// </summary>
        SpiTransferMode TransferMode { get; }
        /// <summary>
        /// Length of <see cref="Tx"/> and <see cref="Rx"/> buffers, in bytes
        /// </summary>
        int Length { get; }
        /// <summary>
        /// Holds pointer to userspace buffer with transmit data, or <c>null</c>. If no data is provided, zeroes are shifted out
        /// </summary>
        IMemory Tx { get; }
        /// <summary>
        /// Holds pointer to userspace buffer for receive data, or <c>null</c>
        /// </summary>
        IMemory Rx { get; }

        /// <summary>
        /// The IOCTL structure that contains control information for a single SPI transfer
        /// </summary>
        SpiTransferControlStructure ControlStructure { get; }
    }
}