using System;
using System.Runtime.InteropServices;
using Raspberry.IO.Interop;

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A Linux I/O control device that additionally can send/receive SPI data structures.
    /// </summary>
    public class SpiControlDevice : ControlDevice, ISpiControlDevice
    {
        #region Libc imports
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int ioctl(int descriptor, UInt32 request, ref SpiTransferControlStructure data);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int ioctl(int descriptor, UInt32 request, SpiTransferControlStructure[] data);
        #endregion

        #region Instance Management
        /// <summary>
        /// Initializes a new instance of the <see cref="SpiControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <remarks><paramref name="file"/> will be disposed if the user calls Dispose on this instance.</remarks>
        public SpiControlDevice(IFile file) 
            :base(file)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <param name="disposeFile">If <c>true</c> the supplied <paramref name="file"/> will be disposed if the user calls Dispose on this instance.</param>
        public SpiControlDevice(IFile file, bool disposeFile) 
            :base(file, disposeFile){}
        #endregion

        #region Methods
        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control(UInt32 request, ref SpiTransferControlStructure data) {
            var result = ioctl(file.Descriptor, request, ref data);
            return result;
        }
        
        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The SPI control data structures to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control(UInt32 request, SpiTransferControlStructure[] data) {
            var result = ioctl(file.Descriptor, request, data);
            return result;
        }
        #endregion
    }
}