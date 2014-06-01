using System;
using System.Collections.Generic;

namespace Raspberry.IO.Interop
{
    /// <summary>
    /// Managed/Unmanaged memory that can be used for P/Invoke operations.
    /// </summary>
    public interface IMemory : IDisposable, IEnumerable<byte> {
        /// <summary>
        /// Pointer to the memory address.
        /// </summary>
        IntPtr Pointer { get; }
        
        /// <summary>
        /// Size in bytes
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself.
        /// </summary>
        /// <param name="index">Offset to memory</param>
        /// <returns>Byte at/from the specified position <paramref name="index"/>.</returns>
        byte this[int index] { get; set; }

        /// <summary>
        /// Writes <paramref name="data"/> at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="data">Data that shall be written.</param>
        void Write(int offset, byte data);

        /// <summary>
        /// Reads a byte at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <returns>The data.</returns>
        byte Read(int offset);

        /// <summary>
        /// Copies the bytes from <paramref name="source"/> to the memory.
        /// </summary>
        /// <param name="source">Source byte array.</param>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the memory.</param>
        /// <param name="length">Copies <paramref name="length"/> bytes.</param>
        void Copy(byte[] source, int sourceIndex, int destinationIndex, int length);
        
        /// <summary>
        /// Copies data from the memory to the supplied <paramref name="destination"/> byte array.
        /// </summary>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destination">Destination byte array.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the destination byte array.</param>
        /// <param name="length">Copies <paramref name="length"/> bytes.</param>
        void Copy(int sourceIndex, byte[] destination, int destinationIndex, int length);
    }
}