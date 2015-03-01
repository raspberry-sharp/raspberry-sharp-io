namespace Raspberry.IO.Components.Converters.Mcp3208
{
    public class Mcp3208InputAnalogPin : IInputAnalogPin
    {
        #region Fields

        private readonly Mcp3208SpiConnection connection;
        private readonly Mcp3208Channel channel;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp3208InputAnalogPin" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        public Mcp3208InputAnalogPin(Mcp3208SpiConnection connection, Mcp3208Channel channel)
        {
            this.connection = connection;
            this.channel = channel;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){}

        #endregion

        #region Methods

        /// <summary>
        /// Reads the value of the pin.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        public AnalogValue Read()
        {
            return connection.Read(channel);
        }

        #endregion
    }
}