#region References

using System;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides an interface for output, binary pins.
    /// </summary>
    public interface IOutputBinaryPin : IDisposable
    {
        #region Methods

        /// <summary>
        /// Writes the value of the pin.
        /// </summary>
        /// <param name="state">if set to <c>true</c>, pin is set to high state.</param>
        void Write(bool state);

        #endregion
    }
}