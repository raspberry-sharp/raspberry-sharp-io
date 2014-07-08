namespace Raspberry.IO.Components.Converters.Mcp3008
{
    public static class Mcp3008AnalogPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Creates an analog input pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The pin.</returns>
        public static Mcp3008InputAnalogPin In(this Mcp3008SpiConnection connection, Mcp3008Channel channel, decimal scale = 1.0m)
        {
            return new Mcp3008InputAnalogPin(connection, channel, scale);
        }

        #endregion
    }
}