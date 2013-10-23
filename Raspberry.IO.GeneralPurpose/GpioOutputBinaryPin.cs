namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a GPIO output binary pin.
    /// </summary>
    public class GpioOutputBinaryPin : IOutputBinaryPin
    {
        #region Fields

        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioOutputBinaryPin"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        public GpioOutputBinaryPin(IGpioConnectionDriver driver, ProcessorPin pin)
        {
            this.driver = driver;
            this.pin = pin;

            driver.Allocate(pin, PinDirection.Output);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            driver.Release(pin);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the value of the pin.
        /// </summary>
        /// <param name="state">if set to <c>true</c>, pin is set to high state.</param>
        public void Write(bool state)
        {
            driver.Write(pin, state);
        }

        #endregion
    }
}