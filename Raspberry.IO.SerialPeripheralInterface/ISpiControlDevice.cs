using System;
using Raspberry.IO.Interop;

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A Linux I/O control device that additionally can send/receive SPI data structures.
    /// </summary>
    public interface ISpiControlDevice : IControlDevice
    {
        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The SPI control data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        int Control(UInt32 request, ref SpiTransferControlStructure data);

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The SPI control data structures to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        int Control(UInt32 request, SpiTransferControlStructure[] data);
    }
}