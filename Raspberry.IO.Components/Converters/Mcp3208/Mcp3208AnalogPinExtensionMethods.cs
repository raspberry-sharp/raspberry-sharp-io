namespace Raspberry.IO.Components.Converters.Mcp3208
{
    public static class Mcp3208AnalogPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Creates an analog input pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The pin.</returns>
        public static Mcp3208InputAnalogPin In(this Mcp3208SpiConnection connection, Mcp3208Channel channel)
        {
            return new Mcp3208InputAnalogPin(connection, channel);
        }

        #endregion
    }
}