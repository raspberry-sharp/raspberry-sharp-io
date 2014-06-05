namespace Raspberry.IO.Components.Converters.Mcp3008
{
    public class Mcp3008InputAnalogPin : IInputAnalogPin
    {
        #region Fields

        private readonly Mcp3008SpiConnection connection;
        private readonly Mcp3008Channel channel;
        private readonly decimal scale;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp3008InputAnalogPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="scale">The scale.</param>
        public Mcp3008InputAnalogPin(Mcp3008SpiConnection connection, Mcp3008Channel channel, decimal scale = 1.0m)
        {
            this.connection = connection;
            this.channel = channel;
            this.scale = scale;
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
        public decimal Read()
        {
            return connection.Read(channel)*scale;
        }

        #endregion
    }
}