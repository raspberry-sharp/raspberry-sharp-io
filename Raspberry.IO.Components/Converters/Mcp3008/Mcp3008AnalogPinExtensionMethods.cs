namespace Raspberry.IO.Components.Converters.Mcp3008
{
    public static class Mcp3008AnalogPinExtensionMethods
    {
        public static Mcp3008InputAnalogPin In(this Mcp3008SpiConnection connection, Mcp3008Channel channel, decimal scale = 1.0m)
        {
            return new Mcp3008InputAnalogPin(connection, channel, scale);
        }
    }
}