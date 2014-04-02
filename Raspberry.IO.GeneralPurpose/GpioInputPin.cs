namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents an input pin on a GPIO interface.
    /// </summary>
    public class GpioInputPin : IInputPin
    {
        #region Fields

        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioInputPin"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public GpioInputPin(IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            this.driver = driver;
            this.pin = pin;

            driver.Allocate(pin, PinDirection.Input);
            driver.SetPinResistor(pin, resistor);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            driver.Release(pin);
        }

        /// <summary>
        /// Reads the current value.
        /// </summary>
        /// <returns>
        /// The pin value.
        /// </returns>
        public bool Read()
        {
            return driver.Read(pin);
        }

        /// <summary>
        /// Waits the current pin to be up.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c>, waits for the pin to be up; otherwise, waits for the pin to be down.</param>
        /// <param name="timeout">The timeout.</param>
        public void Wait(bool waitForUp = true, decimal timeout = 0)
        {
            driver.Wait(pin, waitForUp, timeout);
        }

        #endregion
    }
}