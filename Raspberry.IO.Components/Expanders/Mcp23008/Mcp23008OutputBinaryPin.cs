using System;

namespace Raspberry.IO.Components.Expanders.Mcp23008
{
    /// <summary>
    /// Represents a binary output pin on a MCP23008 I/O expander.
    /// </summary>
    public class Mcp23008OutputBinaryPin : IOutputBinaryPin
    {
        #region Properties

        private readonly Mcp23008I2cConnection connection;
        private readonly Mcp23008Pin pin;

        #endregion

                #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23008OutputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        public Mcp23008OutputBinaryPin(Mcp23008I2cConnection connection, Mcp23008Pin pin,
            Mcp23008PinResistor resistor = Mcp23008PinResistor.None,
            Mcp23008PinPolarity polarity = Mcp23008PinPolarity.Normal)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23008PinDirection.Output);
            connection.SetResistor(pin, resistor);
            connection.SetPolarity(pin, polarity);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){}

        #endregion

        #region Methods

        /// <summary>
        /// Writes the value of the pin.
        /// </summary>
        /// <param name="state">if set to <c>true</c>, pin is set to high state.</param>
        public void Write(bool state)
        {
            connection.SetPinStatus(pin, state);
        }

        #endregion
    }
}
