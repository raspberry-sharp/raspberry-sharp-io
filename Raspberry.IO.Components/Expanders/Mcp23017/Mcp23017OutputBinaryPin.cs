namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public class Mcp23017OutputBinaryPin : IOutputBinaryPin
    {
        private readonly Mcp23017I2cConnection connection;
        private readonly Mcp23017Pin pin;

        public Mcp23017OutputBinaryPin(Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23017PinDirection.Output);
            connection.SetResistor(pin, resistor);
            connection.SetPolarity(pin, polarity);
        }

        public void Dispose(){}

        public void Write(bool state)
        {
            connection.SetPinStatus(pin, state);
        }
    }
}