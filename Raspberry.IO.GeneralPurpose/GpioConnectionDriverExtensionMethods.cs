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
        /// Measures the time the specified pin remains up.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The measure pin.</param>
        /// <param name="waitForDown">if set to <c>true</c>, waits for the pin to become down; otherwise, waits for the pin to become up.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// The time the pin remains up, in milliseconds.
        /// </returns>
        public static decimal Time(this IGpioConnectionDriver driver, ProcessorPin pin, bool waitForDown = true, decimal timeout = 0)
        {
            var waitDown = DateTime.Now;
            driver.Wait(pin, !waitForDown, timeout);

            return (DateTime.Now.Ticks - waitDown.Ticks)/10000m;
        }

        #endregion
    }
}