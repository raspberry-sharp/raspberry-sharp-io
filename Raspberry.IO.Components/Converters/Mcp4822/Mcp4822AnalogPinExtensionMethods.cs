namespace Raspberry.IO.Components.Converters.Mcp4822
{
    public static class Mcp4822AnalogPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Creates an output analog pin.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="scale">The maximum.</param>
        /// <returns>The pin.</returns>
        public static Mcp4822OutputAnalogPin Out(this Mcp4822SpiConnection connection, Mcp4822Channel channel)
        {
            return new Mcp4822OutputAnalogPin(connection, channel);
        }
            
        #endregion
    }
}