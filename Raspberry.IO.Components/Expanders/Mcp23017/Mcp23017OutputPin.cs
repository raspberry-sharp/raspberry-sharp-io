namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public class Mcp23017OutputPin : IOutputPin
    {
        private readonly Mcp23017I2cConnection connection;
        private readonly Mcp23017Pin pin;

        public Mcp23017OutputPin(Mcp23017I2cConnection connection, Mcp23017Pin pin)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23017PinDirection.Output);
            connection.SetPolarity(pin, Mcp23017PinPolarity.Normal);
        }

        public void Dispose(){}

        public void Write(bool state)
        {
            connection.SetPinStatus(pin, state);
        }
    }
}