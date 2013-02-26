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
        /// Waits for the specified pin to reach a given state.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c>, waits for the pin to become up; otherwise, waits for the pin to become down.</param>
        /// <param name="timeout">The timeout.</param>
        public static void Wait(this IGpioConnectionDriver driver, ProcessorPin pin, bool waitForUp = true, int timeout = 0)
        {
            var startWait = DateTime.Now;
            if (timeout == 0)
                timeout = 5000;

            while (driver.Read(pin) != waitForUp)
            {
                if (DateTime.Now.Ticks - startWait.Ticks >= 10000*timeout)
                    throw new TimeoutException("A timeout occurred while waiting");
            }
        }

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
        public static decimal Time(this IGpioConnectionDriver driver, ProcessorPin pin, bool waitForDown = true, int timeout = 0)
        {
            var waitDown = DateTime.Now;
            if (timeout == 0)
                timeout = 5000;

            while (driver.Read(pin) == waitForDown)
            {
                if (DateTime.Now.Ticks - waitDown.Ticks >= 10000*timeout)
                    throw new TimeoutException("A timeout occurred while measuring time");
            }

            return (DateTime.Now.Ticks - waitDown.Ticks)/10000m;
        }

        #endregion
    }
}