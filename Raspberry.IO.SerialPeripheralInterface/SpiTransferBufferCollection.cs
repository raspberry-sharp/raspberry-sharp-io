#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A collection of transfer buffers that can be used to read from / write to the SPI bus.
    /// </summary>
    public class SpiTransferBufferCollection : ISpiTransferBufferCollection
    {
        #region Fields
        private ISpiTransferBuffer[] transferBuffers;
        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="SpiTransferBufferCollection"/> class.
        /// </summary>
        /// <param name="numberOfMessages">Number of tranfer messages</param>
        /// <param name="messageLengthInBytes">Message size in bytes</param>
        /// <param name="transferMode">Transfer mode</param>
        public SpiTransferBufferCollection(int numberOfMessages, int messageLengthInBytes, SpiTransferMode transferMode) {
            if (numberOfMessages <= 0) {
                throw new ArgumentOutOfRangeException("numberOfMessages", numberOfMessages, string.Format(CultureInfo.InvariantCulture, "{0} is not a valid number of messages", numberOfMessages));
            }

            transferBuffers = new ISpiTransferBuffer[numberOfMessages];
            for (var i = 0; i < numberOfMessages; i++) {
                transferBuffers[i] = new SpiTransferBuffer(messageLengthInBytes, transferMode);
            }
        }

        /// <summary>
        /// Finalizer for <see cref="SpiTransferBufferCollection"/>
        /// </summary>
        ~SpiTransferBufferCollection() {
            Dispose(false);
        }
        
        /// <summary>
        /// Disposes the collection of <see cref="ISpiTransferBuffer"/>.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the collection of <see cref="ISpiTransferBuffer"/>.
        /// </summary>
        /// <param name="disposing">If <c>true</c> all transfer buffers will be disposed.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                foreach (var buffer in transferBuffers) {
                    buffer.Dispose();
                }
                transferBuffers = new ISpiTransferBuffer[0];
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of <see cref="ISpiTransferBuffer"/> structures.
        /// </summary>
        public int Length {
            get { return transferBuffers.Length; }
        }

        /// <summary>
        /// Can be used to request a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        public ISpiTransferBuffer this[int index] {
            get { return transferBuffers[index]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A method that returns a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        public ISpiTransferBuffer Get(int index) {
            return transferBuffers[index];
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<ISpiTransferBuffer> GetEnumerator() {
            return transferBuffers.OfType<ISpiTransferBuffer>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}