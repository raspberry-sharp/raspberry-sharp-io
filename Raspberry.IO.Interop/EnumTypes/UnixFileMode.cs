using System;

namespace Raspberry.IO.Interop
{
    /// <summary>
    /// File access mode
    /// </summary>
    [Flags]
    public enum UnixFileMode 
    {
        /// <summary>
        /// The file will be opened with read-only access.
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// The file will be opened with read/write access.
        /// </summary>
        ReadWrite = 2,
        /// <summary>
        /// When possible, the file is opened in nonblocking mode.
        /// </summary>
        NonBlocking = 4,
        /// <summary>
        /// The file is opened for synchronous I/O.
        /// </summary>
        Synchronized = 10000
    }
}