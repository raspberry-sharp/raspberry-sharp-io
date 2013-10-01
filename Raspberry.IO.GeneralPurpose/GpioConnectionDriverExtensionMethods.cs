#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extension methods for <see cref="IGpioConnectionDriver"/>.
    /// </summary>
    public static class GpioConnectionDriverExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Waits for a pin to reach the specified state, then measures the time it remains in this state.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The measure pin.</param>
        /// <param name="waitForUp">if set to <c>true</c>, wait for the pin to be up.</param>
        /// <param name="phase1Timeout">The first phase timeout.</param>
        /// <param name="phase2Timeout">The second phase timeout.</param>
        /// <returns>
        /// The time the pin remains up, in milliseconds.
        /// </returns>
        public static decimal Time(this IGpioConnectionDriver driver, ProcessorPin pin, bool waitForUp = true, decimal phase1Timeout = 0, decimal phase2Timeout = 0)
        {
            driver.Wait(pin, waitForUp, phase1Timeout);

            var waitDown = DateTime.Now.Ticks;
            driver.Wait(pin, !waitForUp, phase2Timeout);

            return (DateTime.Now.Ticks - waitDown)/10000m;
        }

        #endregion
    }
}