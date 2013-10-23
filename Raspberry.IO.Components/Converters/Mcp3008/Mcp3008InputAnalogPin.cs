namespace Raspberry.IO.Components.Converters.Mcp3008
{
    public class Mcp3008InputAnalogPin : IInputAnalogPin
    {
        private readonly Mcp3008SpiConnection connection;
        private readonly Mcp3008Channel channel;
        private readonly decimal scale;

        public Mcp3008InputAnalogPin(Mcp3008SpiConnection connection, Mcp3008Channel channel, decimal scale = 1.0m)
        {
            this.connection = connection;
            this.channel = channel;
            this.scale = scale;
        }

        public void Dispose(){}

        public decimal Read()
        {
            return connection.Read(channel)*scale;
        }
    }
}