#region References

using System;
using System.Runtime.InteropServices;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A IOCTL structure that describes a single SPI transfer
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct SpiTransferControlStructure
    {
        #region Properties

        /// <summary>
        /// Holds pointer to userspace buffer with transmit data, or 0. If no data is provided, zeroes are shifted out
        /// </summary>
        public UInt64 Tx;

        /// <summary>
        /// Holds pointer to userspace buffer for receive data, or 0
        /// </summary>
        public UInt64 Rx;

        /// <summary>
        /// Length of <see cref="Tx"/> and <see cref="Rx"/> buffers, in bytes
        /// </summary>
        public UInt32 Length;

        /// <summary>
        /// Temporary override of the device's bitrate (in Hz)
        /// </summary>
        public UInt32 Speed;

        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer
        /// </summary>
        public UInt16 Delay;

        /// <summary>
        /// Temporary override of the device's wordsize
        /// </summary>
        public Byte BitsPerWord;

        /// <summary>
        /// Set to <c>true</c> to deselect device before starting the next transfer
        /// </summary>
        public Byte ChipSelectChange;

        /// <summary>
        /// Pad
        /// </summary>
        public UInt32 Pad;

        #endregion
    }
}