#region References

using System;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides a generic interface for binary input pins.
    /// </summary>
    public interface IInputPin : IDisposable
    {
        #region Methods

        /// <summary>
        /// Reads the current value.
        /// </summary>
        /// <returns>The pin value.</returns>
        bool Read();

        /// <summary>
        /// Waits the current pin to be up.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c>, waits for the pin to be up; otherwise, waits for the pin to be down.</param>
        /// <param name="timeout">The timeout.</param>
        void Wait(bool waitForUp = true, decimal timeout = 0);

        #endregion
    }
}