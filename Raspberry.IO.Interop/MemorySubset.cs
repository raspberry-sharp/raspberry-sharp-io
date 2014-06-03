using System;
using System.Collections;
using System.Collections.Generic;

namespace Raspberry.IO.Interop
{
    /// <summary>
    /// A subset of an already allocated memory block.
    /// </summary>
    public class MemorySubset : IMemory
    {
        #region Fields
        private readonly IMemory memory;

        private int memoryLength;
        private int memoryOffset;
        private bool owner;
        private IntPtr memoryPointer;
        #endregion

        #region Instance Management   
        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySubset"/> class.
        /// </summary>
        /// <param name="memoryBlock">The origin memory block</param>
        /// <param name="startOffset">Start offset of the origin memory block</param>
        /// <param name="length">Length of this memory subset in bytes</param>
        /// <param name="isOwner">If <c>true</c> the origin <paramref name="memoryBlock"/> will be disposed on <see cref="Dispose"/>.</param>
        public MemorySubset(IMemory memoryBlock, int startOffset, int length, bool isOwner) {
            if (ReferenceEquals(memoryBlock, null)) {
                throw new ArgumentNullException("memoryBlock");
            }
            if (startOffset < 0 || startOffset > memoryBlock.Length) {
                var message = string.Format("The offset must be between 0 and {0}", memoryBlock.Length);
                throw new ArgumentOutOfRangeException("startOffset", startOffset, message);
            }
            if (length < 0 || startOffset + length > memoryBlock.Length) {
                throw new ArgumentOutOfRangeException("length", length, "Invalid size");
            }

            memory = memoryBlock;
            memoryOffset = startOffset;
            memoryLength = length;
            memoryPointer = memory.Pointer + memoryOffset;
            owner = isOwner;
        }

        /// <summary>
        /// If owner, managed memory will be released. Otherwise this method does nothing.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free managed memory. This method does nothing if it is not owner of origin memory block.
        /// </summary>
        /// <param name="disposing">If this instance is the owner of the memory block it will always release the origin memory block to avoid memory leaks. If you don't want this, don't call this method (<see cref="Dispose(bool)"/>) in your derived class.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // free managed here
            }

            if (owner) {
                memoryLength = 0;
                memoryPointer = IntPtr.Zero;
                memoryOffset = 0;
                memory.Dispose();
                owner = false;
            }
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
            get { return memoryLength; }
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
        /// Returns an enumerator
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/>
        /// </returns>
        public IEnumerator<byte> GetEnumerator() {
            var tmp = new byte[memoryLength];
            memory.Copy(memoryOffset, tmp, 0, memoryLength);
            
            return ((IEnumerable<byte>) tmp)
                .GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/>
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Writes <paramref name="data"/> at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="data">Data that shall be written.</param>
        public void Write(int offset, byte data) {
            if (offset < 0 || offset >= memoryLength) {
                throw new ArgumentOutOfRangeException("offset", offset, "invalid offset");
            }
            memory.Write(memoryOffset + offset, data);
        }

        /// <summary>
        /// Reads a byte at <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <returns>The data.</returns>
        public byte Read(int offset) {
            if (offset < 0 || offset >= memoryLength) {
                throw new ArgumentOutOfRangeException("offset", offset, "invalid offset");
            }
            return memory.Read(memoryOffset + offset);
        }

        /// <summary>
        /// Copies the bytes from <paramref name="source"/> to the memory.
        /// </summary>
        /// <param name="source">Source byte array.</param>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the memory.</param>
        /// <param name="length">Copies <paramref name="length"/> bytes.</param>
        public void Copy(byte[] source, int sourceIndex, int destinationIndex, int length) {
            if (destinationIndex < 0 || destinationIndex > memoryLength) {
                var message = string.Format("destination index must be greater than 0 and lower or equal to {0}", 
                    memoryLength);
                throw new ArgumentOutOfRangeException("destinationIndex", destinationIndex, 
                    message);
            }
            if (destinationIndex + length > memoryLength) {
                throw new ArgumentOutOfRangeException("length", length, "invalid length");
            }

            memory.Copy(source, sourceIndex, memoryOffset + destinationIndex, length);
        }

        /// <summary>
        /// Copies data from the memory to the supplied <paramref name="destination"/> byte array.
        /// </summary>
        /// <param name="sourceIndex">Copies the data starting from <paramref name="sourceIndex"/>.</param>
        /// <param name="destination">Destination byte array.</param>
        /// <param name="destinationIndex">Copies the data starting at <paramref name="destinationIndex"/> to the destination byte array.</param>
        /// <param name="length">Copies <paramref name="length"/> bytes.</param>
        public void Copy(int sourceIndex, byte[] destination, int destinationIndex, int length) {
            if (sourceIndex < 0 || sourceIndex > memoryLength) {
                var message = string.Format("source index must be greater than 0 and lower or equal to {0}", 
                    memoryLength);
                throw new ArgumentOutOfRangeException("sourceIndex", sourceIndex, message);
            }
            if (sourceIndex + length > memoryLength) {
                throw new ArgumentOutOfRangeException("length", length, "invalid length");
            }

            memory.Copy(memoryOffset + sourceIndex, destination, destinationIndex, length);
        }
        #endregion
    }
}