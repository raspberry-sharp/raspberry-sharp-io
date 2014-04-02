namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extension methods to create GPIO pins
    /// </summary>
    public static class GpioPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Created an <see cref="IOutputBinaryPin"/> output pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioOutputBinaryPin Out(this IGpioConnectionDriver driver, ConnectorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return driver.Out(pin.ToProcessor(), resistor);
        }

        /// <summary>
        /// Created an <see cref="IOutputBinaryPin"/> output pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioOutputBinaryPin Out(this IGpioConnectionDriver driver, ProcessorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return new GpioOutputBinaryPin(driver, pin, resistor);
        }

        /// <summary>
        /// Created an <see cref="IInputBinaryPin"/> input pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioInputBinaryPin In(this IGpioConnectionDriver driver, ConnectorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return driver.In(pin.ToProcessor(), resistor);
        }

        /// <summary>
        /// Created an <see cref="IInputBinaryPin"/> input pin on the specified driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns></returns>
        public static GpioInputBinaryPin In(this IGpioConnectionDriver driver, ProcessorPin pin,
            PinResistor resistor = PinResistor.None)
        {
            return new GpioInputBinaryPin(driver, pin, resistor);
        }

        #endregion
    }
}