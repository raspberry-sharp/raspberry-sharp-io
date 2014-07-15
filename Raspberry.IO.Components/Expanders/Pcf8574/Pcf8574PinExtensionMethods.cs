namespace Raspberry.IO.Components.Expanders.Pcf8574
{
    /// <summary>
    /// Provides extension methods for PCF8574 pins.
    /// </summary>
    public static class Pcf8574PinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Creates an output binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin.</returns>
        public static Pcf8574OutputBinaryPin Out(this Pcf8574I2cConnection connection, Pcf8574Pin pin)
        {
            return new Pcf8574OutputBinaryPin(connection, pin);
        }

        /// <summary>
        /// Creates an input binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin.</returns>
        public static Pcf8574InputBinaryPin In(this Pcf8574I2cConnection connection, Pcf8574Pin pin)
        {
            return new Pcf8574InputBinaryPin(connection, pin);
        }

        #endregion
    }
}