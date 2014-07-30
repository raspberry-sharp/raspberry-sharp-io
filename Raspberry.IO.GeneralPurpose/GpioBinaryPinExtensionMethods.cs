namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extensions methods GPIO binary pins.
    /// </summary>
    public static class GpioBinaryPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Gets an output pin on the current driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>The GPIO output binary pin.</returns>
        public static GpioOutputBinaryPin Out(this IGpioConnectionDriver driver, ConnectorPin pin)
        {
            return driver.Out(pin.ToProcessor());
        }

        /// <summary>
        /// Gets an output pin on the current driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>The GPIO output binary pin.</returns>
        public static GpioOutputBinaryPin Out(this IGpioConnectionDriver driver, ProcessorPin pin)
        {
            return new GpioOutputBinaryPin(driver, pin);
        }

        /// <summary>
        /// Gets an input pin on the current driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns>
        /// The GPIO input binary pin.
        /// </returns>
        public static GpioInputBinaryPin In(this IGpioConnectionDriver driver, ConnectorPin pin, PinResistor resistor = PinResistor.None)
        {
            return driver.In(pin.ToProcessor(), resistor);
        }

        /// <summary>
        /// Gets an input pin on the current driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns>
        /// The GPIO input binary pin.
        /// </returns>
        public static GpioInputBinaryPin In(this IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            return new GpioInputBinaryPin(driver, pin, resistor);
        }

        /// <summary>
        /// Gets a bidirectional pin on the current driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns>
        /// The GPIO input binary pin.
        /// </returns>
        public static GpioInputOutputBinaryPin InOut(this IGpioConnectionDriver driver, ConnectorPin pin, PinResistor resistor = PinResistor.None)
        {
            return driver.InOut(pin.ToProcessor(), resistor);
        }

        /// <summary>
        /// Gets a bidirectional pin on the current driver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <returns>
        /// The GPIO input binary pin.
        /// </returns>
        public static GpioInputOutputBinaryPin InOut(this IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            return new GpioInputOutputBinaryPin(driver, pin, resistor);
        }

        #endregion
    }
}