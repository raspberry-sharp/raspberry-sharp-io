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
    /// <remarks>
    ///     <see cref="https://docs.google.com/document/d/1Y-yZnNhMYy7rwhAgyL_pfa39RsB-x2qR4vP8saG73rE/edit"/> for hardware specification and 
    ///     <see cref="http://www.raspberrypi-spy.co.uk/2012/12/ultrasonic-distance-measurement-using-python-part-1/"/> for implementation details.
    /// </remarks>
    public class HcSr04Connection : IDisposable
    {
        #region Fields

        private const decimal triggerTime = 0.01m;  // Waits at least 10µs = 0.01ms
        private const decimal echoUpTimeout = 500m;

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

            Timeout = DefaultTimeout;

            driver = GpioConnectionSettings.DefaultDriver;

            driver.Allocate(echoPin, PinDirection.Input);
            driver.Allocate(triggerPin, PinDirection.Output);

            try
            {
                GetDistance();
            } catch {}
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The default timeout (50ms).
        /// </summary>
        /// <remarks>Maximum time (if no obstacle) is 38ms.</remarks>
        public const decimal DefaultTimeout = 50m;

        /// <summary>
        /// Gets or sets the timeout for distance measure, in milliseconds.
        /// </summary>
        /// <value>
        /// The timeout, in milliseconds.
        /// </value>
        public decimal Timeout { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the distance, in meters.
        /// </summary>
        /// <returns>The distance, in meters.</returns>
        public decimal GetDistance()
        {
            driver.Write(triggerPin, true);
            Timer.Sleep(triggerTime);
            driver.Write(triggerPin, false);

            var upTime = driver.Time(echoPin, true, echoUpTimeout, Timeout);
            return Units.Velocity.Sound.ToDistance(upTime);
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