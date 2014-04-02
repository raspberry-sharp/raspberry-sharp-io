namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extension methods to create GPIO pins
    /// </summary>
    public static class GpioPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Created an <see cref="IOutputPin"/> output pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioOutputPin Out(this IGpioConnectionDriver driver, ConnectorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return driver.Out(pin.ToProcessor(), resistor);
        }

        /// <summary>
        /// Created an <see cref="IOutputPin"/> output pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioOutputPin Out(this IGpioConnectionDriver driver, ProcessorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return new GpioOutputPin(driver, pin, resistor);
        }

        /// <summary>
        /// Created an <see cref="IInputPin"/> input pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioInputPin In(this IGpioConnectionDriver driver, ConnectorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return driver.In(pin.ToProcessor(), resistor);
        }

        /// <summary>
        /// Created an <see cref="IInputPin"/> input pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioInputPin In(this IGpioConnectionDriver driver, ProcessorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return new GpioInputPin(driver, pin, resistor);
        }

        #endregion
    }
}