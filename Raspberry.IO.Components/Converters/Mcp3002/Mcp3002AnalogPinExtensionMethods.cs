namespace Raspberry.IO.Components.Converters.Mcp3002
{
    public static class Mcp3002AnalogPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Creates an analog input pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The pin.</returns>
        public static Mcp3002InputAnalogPin In(this Mcp3002SpiConnection connection, Mcp3002Channel channel)
        {
            return new Mcp3002InputAnalogPin(connection, channel);
        }

        #endregion
    }
}