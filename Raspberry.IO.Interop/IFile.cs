using System;

namespace Raspberry.IO.Interop
{
    /// <summary>
    /// A file resource that is controlled by the underling operation system.
    /// </summary>
    public interface IFile : IDisposable {
        /// <summary>
        /// The file descriptor
        /// </summary>
        int Descriptor { get; }

        /// <summary>
        /// The pathname to the file
        /// </summary>
        string Filename { get; }
    }
}