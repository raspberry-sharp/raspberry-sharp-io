namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public class Mcp23017OutputBinaryPin : IOutputBinaryPin
    {
        #region Fields

        private readonly Mcp23017I2cConnection connection;
        private readonly Mcp23017Pin pin;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23017OutputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        public Mcp23017OutputBinaryPin(Mcp23017I2cConnection connection, Mcp23017Pin pin)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23017PinDirection.Output);
            connection.SetPolarity(pin, Mcp23017PinPolarity.Normal);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }

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