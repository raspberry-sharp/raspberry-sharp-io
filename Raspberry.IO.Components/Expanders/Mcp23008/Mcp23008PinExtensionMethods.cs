namespace Raspberry.IO.Components.Expanders.Mcp23008
{
    /// <summary>
    /// Provides extension methods for MCP23008 pins.
    /// </summary>
    public static class Mcp23008PinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Creates an output binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The pin.</returns>
        public static Mcp23008OutputBinaryPin Out(this Mcp23008I2cConnection connection, Mcp23008Pin pin, Mcp23008PinResistor resistor = Mcp23008PinResistor.None, Mcp23008PinPolarity polarity = Mcp23008PinPolarity.Normal)
        {
            return new Mcp23008OutputBinaryPin(connection, pin, resistor, polarity);
        }

        /// <summary>
        /// Creates an input binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The pin.</returns>
        public static Mcp23008InputBinaryPin In(this Mcp23008I2cConnection connection, Mcp23008Pin pin, Mcp23008PinResistor resistor = Mcp23008PinResistor.None, Mcp23008PinPolarity polarity = Mcp23008PinPolarity.Normal)
        {
            return new Mcp23008InputBinaryPin(connection, pin, resistor, polarity);
        }

        #endregion
    }
}
