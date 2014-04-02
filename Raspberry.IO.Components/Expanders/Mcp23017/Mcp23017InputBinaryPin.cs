using System;

namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public class Mcp23017InputBinaryPin : IInputBinaryPin
    {
        private readonly Mcp23017I2cConnection connection;
        private readonly Mcp23017Pin pin;

        public Mcp23017InputBinaryPin(Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23017PinDirection.Input);
            connection.SetResistor(pin, resistor);
            connection.SetPolarity(pin, polarity);
        }

        public void Dispose(){}

        public bool Read()
        {
            return connection.GetPinStatus(pin);
        }

        public void Wait(bool waitForUp = true, decimal timeout = 0)
        {
            var startWait = DateTime.Now;
            if (timeout == 0)
                timeout = 5000;

            while (Read() != waitForUp)
            {
                if (DateTime.Now.Ticks - startWait.Ticks >= 10000 * timeout)
                    throw new TimeoutException("A timeout occurred while waiting");
            }
        }
    }
}