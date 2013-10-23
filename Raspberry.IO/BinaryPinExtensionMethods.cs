#region References

using System;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides extension methods for binary pins.
    /// </summary>
    public static class BinaryPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Waits for a pin to reach the specified state, then measures the time it remains in this state.
        /// </summary>
        /// <param name="pin">The measure pin.</param>
        /// <param name="waitForUp">if set to <c>true</c>, wait for the pin to be up.</param>
        /// <param name="phase1Timeout">The first phase timeout.</param>
        /// <param name="phase2Timeout">The second phase timeout.</param>
        /// <returns>
        /// The time the pin remains up, in milliseconds.
        /// </returns>
        public static decimal Time(this IInputBinaryPin pin, bool waitForUp = true, decimal phase1Timeout = 0, decimal phase2Timeout = 0)
        {
            pin.Wait(waitForUp, phase1Timeout);

            var waitDown = DateTime.Now.Ticks;
            pin.Wait(!waitForUp, phase2Timeout);

            return (DateTime.Now.Ticks - waitDown)/10000m;
        }

        #endregion
    }
}