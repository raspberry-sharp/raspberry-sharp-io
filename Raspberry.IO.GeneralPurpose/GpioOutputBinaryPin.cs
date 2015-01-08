namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents an output pin on GPIO interface.
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
        /// <param name="resistor">The resistor.</param>
        public GpioOutputBinaryPin(IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            this.driver = driver;
            this.pin = pin;

            driver.Allocate(pin, PinDirection.Output);
            if ((driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) > 0)
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
        /// Writes the specified state.
        /// </summary>
        /// <param name="state">The pin state.</param>
        public void Write(bool state)
        {
            driver.Write(pin, state);
        }

        #endregion
    }
}