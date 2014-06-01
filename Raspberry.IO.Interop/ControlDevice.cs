using System;
using System.Runtime.InteropServices;

namespace Raspberry.IO.Interop
{
    /// <summary>
    /// A Linux I/O control device.
    /// </summary>
    public class ControlDevice : IControlDevice
    {
        #region Classes 
        private static class GenericIoControl<T> {
            #region Libc imports
            [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
            public static extern int ioctl(int descriptor, UInt32 request, ref T data);
            
            [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
            public static extern int ioctl(int descriptor, UInt32 request, T data);
            #endregion
        }
        #endregion

        #region Fields
        private readonly IFile file;
        private readonly bool disposeFile;

        #endregion

        #region Libc imports
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int ioctl(int descriptor, UInt32 request, IntPtr data);
        #endregion

        #region Instance Management
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <remarks><paramref name="file"/> will be disposed if the user calls <see cref="Dispose"/> on this instance.</remarks>
        public ControlDevice(IFile file): this(file, true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDevice"/> class.
        /// </summary>
        /// <param name="file">A opened special file that can be controlled using ioctl-system calls.</param>
        /// <param name="disposeFile">If <c>true</c> the supplied <paramref name="file"/> will be disposed if the user calls <see cref="Dispose"/> on this instance.</param>
        public ControlDevice(IFile file, bool disposeFile) {
            this.file = file;
            this.disposeFile = disposeFile;
        }

        ~ControlDevice() {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">If <c>true</c> the managed resources will be disposed as well.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing && disposeFile) {
                file.Dispose();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <typeparam name="T">Data type that will be marshaled and send.</typeparam>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control<T>(UInt32 request, ref T data) {
            var result = GenericIoControl<T>.ioctl(file.Descriptor, request, ref data);
            return result;
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <typeparam name="T">Data type that will be marshaled and send.</typeparam>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">The data to be transmitted.</param>
        /// <returns>Usually, on success zero is returned. A few ioctls use the return value as an output parameter and return a nonnegative value on success. On error, -1 is returned, and errno is set appropriately.</returns>
        public int Control<T>(uint request, T data) {
            var result = GenericIoControl<T>.ioctl(file.Descriptor, request, data);
            return result;
        }

        /// <summary>
        /// The function manipulates the underlying device parameters of special files. In particular, many operating characteristics of character special files (e.g. terminals) may be controlled with ioctl requests.
        /// </summary>
        /// <param name="request">A device-dependent request code.</param>
        /// <param name="data">An untyped pointer to memory that contains the command/request data.</param>
        /// <returns></returns>
        public int Control(UInt32 request, IntPtr data) {
            var result = ioctl(file.Descriptor, request, data);
            return result;
        }

        #endregion
    }
}