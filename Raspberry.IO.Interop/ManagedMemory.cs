using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Raspberry.IO.Interop
{
    public class ManagedMemory : IMemory
    {
        #region Fields
        private byte[] byteArray;
        private GCHandle handle;
        private IntPtr memoryPointer;
        #endregion

        #region Instance Management
        /// <summary>
        /// Allocates a byte array of the requested size and pins it in order to prevent GC from free/moving it.
        /// </summary>
        /// <param name="length">Memory size in bytes.</param>
        public ManagedMemory(int length) {
            byteArray = new byte[length];
            handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            memoryPointer = handle.AddrOfPinnedObject();
        }
        
        ~ManagedMemory() {
            Dispose(false);
        }

        /// <summary>
        /// Free managed memory.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free managed memory.
        /// </summary>
        /// <param name="disposing">The pinned managed memory will always be released to avoid memory leaks. If you don't want this, don't call this method (<see cref="Dispose(bool)"/>) in your derived class.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // Free managed here
            }
            
            Trace.Assert(disposing,
                string.Format("ERROR: GC finalized managed memory of {0} bytes for address {1} that was not disposed!",
                byteArray.Length, memoryPointer.ToString("X8")));


            if (handle.IsAllocated) {
                handle.Free();
            }

            memoryPointer = IntPtr.Zero;
            byteArray = new byte[0];
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
        /// Size in bytes
        /// </summary>
        public int Length {
            get { return byteArray.Length; }
        }

        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself.
        /// </summary>
        /// <param name="index">Offset to memory</param>
        /// <returns>Byte at/from the specified position <paramref name="index"/>.</returns>
        public byte this[int index] {
            get { return byteArray[index]; }
            set { byteArray[index] = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Writes <paramref name="data"/> at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="data">Data that shall be written.</param>
        public void Write(int offset, byte data) {
            byteArray[offset] = data;
        }

        /// <summary>
        /// Reads a byte at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <returns>The data.</returns>
        public byte Read(int offset) {
            return byteArray[offset];
        }

        /// <summary>
        /// Copies the bytes from <paramref name="source"/> to the memory.
        /// </summary>
        /// <param name="source">Source byte array.</param>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the memory.</param>
        /// <param name="length">Copies <paramref name="length"/> bytes.</param>
        public void Copy(byte[] source, int sourceIndex, int destinationIndex, int length) {
            Array.Copy(source, sourceIndex, byteArray, destinationIndex, length);
        }

        /// <summary>
        /// Copies data from the memory to the supplied <paramref name="destination"/> byte array.
        /// </summary>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destination">Destination byte array.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the destination byte array.</param>
        /// <param name="length">Copies <paramref name="length"/> bytes.</param>
        public void Copy(int sourceIndex, byte[] destination, int destinationIndex, int length) {
            Array.Copy(byteArray, sourceIndex, destination, destinationIndex, length);
        }

        /// <summary>
        /// Returns an enumerator;
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator;
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<byte> GetEnumerator() {
            return ((IEnumerable<byte>) byteArray)
                .GetEnumerator();
        }

        /// <summary>
        /// Allocates unmanaged memory for <paramref name="structure"/> and copies its content into it.
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <param name="structure">The structure that shall be copied into the requested memory buffer.</param>
        /// <returns>The unmanaged memory buffer containing <paramref name="structure"/>.</returns>
        public static ManagedMemory CreateAndCopy<T>(T structure) {
            var requiredSize = Marshal.SizeOf(structure);

            var memory = new ManagedMemory(requiredSize);
            Marshal.StructureToPtr(structure, memory.Pointer, false);

            return memory;
        }

        /// <summary>
        /// Allocates unmanaged memory with the size of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Structure type</typeparam>
        /// <returns>The unmanaged memory buffer of size <typeparamref name="T"/>.</returns>
        public static ManagedMemory CreateFor<T>() {
            var requiredSize = Marshal.SizeOf(typeof(T));
            return new ManagedMemory(requiredSize);
        }
        #endregion
    }
}