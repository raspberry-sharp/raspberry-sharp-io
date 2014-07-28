namespace Raspberry.IO.Components.Converters.Mcp4822
{
    public class Mcp4822OutputAnalogPin : IOutputAnalogPin
    {
        #region Fields

        private readonly Mcp4822SpiConnection connection;
        private readonly Mcp4822Channel channel;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp4822OutputAnalogPin" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        public Mcp4822OutputAnalogPin(Mcp4822SpiConnection connection, Mcp4822Channel channel)
        {
            this.connection = connection;
            this.channel = channel;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the specified value to the pin.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(AnalogValue value)
        {
            connection.Write(channel, value);
        }

        #endregion
    }
}