#region References

using System;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides an interface for input, binary pins.
    /// </summary>
    public interface IInputBinaryPin : IDisposable
    {
        #region Methods

        /// <summary>
        /// Reads the state of the pin.
        /// </summary>
        /// <returns><c>true</c> if the pin is in high state; otherwise, <c>false</c>.</returns>
        bool Read();

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up.</param>
        /// <param name="timeout">The timeout, in milliseconds.</param>
        void Wait(bool waitForUp = true, decimal timeout = 0);

        #endregion
    }
}