#region References

using System;
using System.Collections.Generic;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// A collection of transfer buffers that can be used to read from / write to the SPI bus.
    /// </summary>
    public interface ISpiTransferBufferCollection : IDisposable, IEnumerable<ISpiTransferBuffer>
    {
        #region Properties

        /// <summary>
        /// Number of <see cref="ISpiTransferBuffer"/> structures.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Can be used to request a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        ISpiTransferBuffer this[int index] { get; }

        #endregion

        #region Methods

        /// <summary>
        /// A method that returns a specific <see cref="ISpiTransferBuffer"/> from the collection.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>The requested <see cref="ISpiTransferBuffer"/></returns>
        ISpiTransferBuffer Get(int index);

        #endregion
    }
}