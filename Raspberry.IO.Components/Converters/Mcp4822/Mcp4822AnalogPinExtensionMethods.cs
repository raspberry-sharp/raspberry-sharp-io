namespace Raspberry.IO.Components.Converters.Mcp4822
{
    public static class Mcp4822AnalogPinExtensionMethods
    {
        public static Mcp4822OutputAnalogPin Out(this Mcp4822SpiConnection connection, Mcp4822Channel channel, decimal scale = 1.0m)
        {
            return new Mcp4822OutputAnalogPin(connection, channel, scale);
        }
    }
}