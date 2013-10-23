namespace Raspberry.IO.Components.Converters.Mcp4822
{
    public class Mcp4822OutputAnalogPin : IOutputAnalogPin
    {
        private readonly Mcp4822SpiConnection connection;
        private readonly Mcp4822Channel channel;
        private readonly decimal scale;

        public Mcp4822OutputAnalogPin(Mcp4822SpiConnection connection, Mcp4822Channel channel, decimal scale = 1.0m)
        {
            this.connection = connection;
            this.channel = channel;
            this.scale = scale;
        }

        public void Dispose() { }

        public void Write(decimal value)
        {
            connection.Write(channel, value / scale);
        }
    }
}