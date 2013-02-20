using System;
using Raspberry.IO.GeneralPurpose;

namespace Test.Gpio.HCSR04
{
    internal static class GpioConnectionDriverExtensionMethods
    {
        /// <summary>
        /// Waits for the specified pin to reach a given state.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c>, waits for the pin to become up; otherwise, waits for the pin to become down.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   <c>true</c> is the pin has reached the expected status, <c>false</c> if a timeout occurred.
        /// </returns>
        public static bool Wait(this IGpioConnectionDriver driver, ProcessorPin pin, bool waitForUp = true, int timeout = 0)
        {
            if (timeout == 0)
                timeout = 5000;

            var startWait = DateTime.Now;
            while (driver.Read(pin) != waitForUp)
            {
                if (DateTime.Now.Ticks - startWait.Ticks >= 10000 * timeout)
                    return false;
            }

            return true;
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
            if (timeout == 0)
                timeout = 5000;

            var waitDown = DateTime.Now;
            while (driver.Read(pin) == waitForDown)
            {
                if (DateTime.Now.Ticks - waitDown.Ticks >= 10000 * timeout)
                    return decimal.MinValue;
            }

            return (DateTime.Now.Ticks - waitDown.Ticks) / 10000m;
        }
    }
}