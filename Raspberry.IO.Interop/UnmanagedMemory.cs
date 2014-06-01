using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Raspberry.IO.Interop
{
    /// <summary>
    /// An memory buffer (reference) to unmanaged memory that can be used for P/Invoke operations.
    /// </summary>
    public class UnmanagedMemory : IMemory
    {
        #region Fields
        private readonly bool owner;
        private IntPtr memoryPointer;
        private int length;
        #endregion

        #region Instance Management
        /// <summary>
        /// Encapsulates the supplied memory address to unmanaged memory.
        /// </summary>
        /// <param name="memoryPointer">Unmanaged memory address</param>
        /// <param name="length">Size in bytes</param>
        /// <param name="owner">If <c>true</c> the memory will be released if the user calls <see cref="Dispose"/>.</param>
        public UnmanagedMemory(IntPtr memoryPointer, int length, bool owner) {
            this.memoryPointer = memoryPointer;
            this.length = length;
            this.owner = owner;
        }

        /// <summary>
        /// Allocates unmanaged memory of the requested size.
        /// </summary>
        /// <param name="length">Memory size in bytes.</param>
        public UnmanagedMemory(int length) {
            this.length = length;
            memoryPointer = Marshal.AllocHGlobal(length);
            owner = true;
        }

        ~UnmanagedMemory() {
            Dispose(false);
        }

        /// <summary>
        /// Free the unmanaged memory.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free the unmanaged memory.
        /// </summary>
        /// <param name="disposing">The unmanaged memory will always be released to avoid memory leaks. If you don't want this, don't call this method (<see cref="Dispose(bool)"/>) in your derived class.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // free managed here
            }
            
            Trace.Assert(disposing,
                string.Format("ERROR: GC finalized unmanaged memory of {0} bytes for address {1} that was not disposed!",
                    length, memoryPointer.ToString("X8")));

            if (memoryPointer != IntPtr.Zero && owner) {
                // free unmanaged memory to avoid memory leeks
                Marshal.FreeHGlobal(memoryPointer);
            }
            memoryPointer = IntPtr.Zero;
            length = 0;
        }
        #endregion

        #region Properties
        
        /// <summary>
        /// Pointer to the memory address.
        /// </summary>
        public IntPtr Pointer {
            get { return memoryPointer; }
        }

        /// <summary>
        /// Size of the allocated unmanaged memory in bytes.
        /// </summary>
        public int Length {
            get { return length; }
        }

        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself.
        /// </summary>
        /// <param name="index">Offset to memory</param>
        /// <returns>Byte at/from the specified position <paramref name="index"/>.</returns>
        public byte this[int index] {
            get { return Read(index); }
            set { Write(index, value); }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Writes <paramref name="data"/> at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="data">Data that shall be written.</param>
        public void Write(int offset, byte data) {
            if (memoryPointer == IntPtr.Zero) {
                throw new ObjectDisposedException("Unmanaged memory already disposed.");
            }

            if (offset < 0 || offset >= length) {
                throw new ArgumentOutOfRangeException("offset");
            }

            Marshal.WriteByte(memoryPointer, offset, data);
        }

        /// <summary>
        /// Reads a byte at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <returns>The data.</returns>
        public byte Read(int offset) {
            if (memoryPointer == IntPtr.Zero) {
                throw new ObjectDisposedException("Unmanaged memory already disposed.");
            }

            if (offset < 0 || offset >= length) {
                throw new ArgumentOutOfRangeException("offset");
            }

            return Marshal.ReadByte(memoryPointer, offset);
        }

        /// <summary>
        /// Copies the bytes from <paramref name="source"/> to the memory.
        /// </summary>
        /// <param name="source">Source byte array.</param>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the memory.</param>
        /// <param name="lengthInBytes">Copies <paramref name="lengthInBytes"/> bytes.</param>
        public void Copy(byte[] source, int sourceIndex, int destinationIndex, int lengthInBytes) {
            if (memoryPointer == IntPtr.Zero) {
                throw new ObjectDisposedException("Unmanaged memory already disposed.");
            }

            if (destinationIndex < 0 || destinationIndex >= length) {
                throw new ArgumentOutOfRangeException("destinationIndex");
            }

            if (lengthInBytes < 0 || (destinationIndex + lengthInBytes) > length) {
                throw new ArgumentOutOfRangeException("lengthInBytes");
            }

            Marshal.Copy(source, sourceIndex, memoryPointer + destinationIndex, lengthInBytes);
        }

        /// <summary>
        /// Copies data from the memory to the supplied <paramref name="destination"/> byte array.
        /// </summary>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destination">Destination byte array.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the destination byte array.</param>
        /// <param name="lengthInBytes">Copies <paramref name="lengthInBytes"/> bytes.</param>
        public void Copy(int sourceIndex, byte[] destination, int destinationIndex, int lengthInBytes) {
            if (memoryPointer == IntPtr.Zero) {
                throw new ObjectDisposedException("Unmanaged memory already disposed.");
            }

            if (sourceIndex < 0 || sourceIndex >= length) {
                throw new ArgumentOutOfRangeException("sourceIndex");
            }

            if (sourceIndex + lengthInBytes >= length) {
                throw new ArgumentOutOfRangeException("lengthInBytes");
            }

            Marshal.Copy(memoryPointer + sourceIndex, destination, destinationIndex, lengthInBytes);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<byte> GetEnumerator() {
            var tmp = new byte[length];
            Copy(0, tmp, 0, length);

            return ((IEnumerable<byte>) tmp)
                .GetEnumerator();
        }

        /// <summary>
        /// Allocates unmanaged memory for <paramref name="structure"/> and copies its content into it.
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="structure">The structure that shall be copied into the requested memory buffer.</param>
        /// <returns>The unmanaged memory buffer containing <paramref name="structure"/>.</returns>
        public static UnmanagedMemory CreateAndCopy<T>(T structure) {
            var requiredSize = Marshal.SizeOf(structure);
            
            var memory = new UnmanagedMemory(requiredSize);
            Marshal.StructureToPtr(structure, memory.Pointer, false);
            
            return memory;
        }

        /// <summary>
        /// Allocates unmanaged memory with the size of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <returns>The unmanaged memory buffer of size <typeparamref name="T"/>.</returns>
        public static UnmanagedMemory CreateFor<T>() {
            var requiredSize = Marshal.SizeOf(typeof(T));
            return new UnmanagedMemory(requiredSize);
        }
        #endregion
    }
}