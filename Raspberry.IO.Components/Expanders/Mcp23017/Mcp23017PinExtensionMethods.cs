namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    /// <summary>
    /// Provides extension methods for MCP23017 pins.
    /// </summary>
    public static class Mcp23017PinExtensionMethods
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
        public static Mcp23017OutputBinaryPin Out(this Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017OutputBinaryPin(connection, pin, resistor, polarity);
        }

        /// <summary>
        /// Creates an input binary pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        /// <returns>The pin.</returns>
        public static Mcp23017InputBinaryPin In(this Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017InputBinaryPin(connection, pin, resistor, polarity);
        }

        #endregion
    }
}