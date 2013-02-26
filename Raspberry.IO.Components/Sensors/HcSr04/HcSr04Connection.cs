#region References

using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.Components.Sensors.HcSr04
{
    /// <summary>
    /// Represents a connection to HC-SR04 distance sensor.
    /// </summary>
    public class HcSr04Connection : IDisposable
    {
        #region Fields

        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin triggerPin;
        private readonly ProcessorPin echoPin;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="HcSr04Connection"/> class.
        /// </summary>
        /// <param name="triggerPin">The trigger pin.</param>
        /// <param name="echoPin">The echo pin.</param>
        public HcSr04Connection(ProcessorPin triggerPin, ProcessorPin echoPin)
        {
            this.triggerPin = triggerPin;
            this.echoPin = echoPin;

            driver = new MemoryGpioConnectionDriver();

            driver.Allocate(triggerPin, PinDirection.Output);
            driver.Write(triggerPin, false);

            driver.Allocate(echoPin, PinDirection.Input);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the distance, in meters.
        /// </summary>
        /// <returns>The distance, in meters.</returns>
        public decimal GetDistance()
        {
            driver.Write(triggerPin, true);
            Timer.Sleep(0.01m);
            driver.Write(triggerPin, false);

            driver.Wait(echoPin);
            return Units.Velocity.Sound.ToDistance(driver.Time(echoPin));
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            driver.Release(triggerPin);
            driver.Release(echoPin);
        }

        #endregion
    }
}