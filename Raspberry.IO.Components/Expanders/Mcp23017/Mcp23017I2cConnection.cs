using Raspberry.IO.InterIntegratedCircuit;

namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    /// <summary>
    /// Represents a I2C connection to a MCP23017 I/O Expander.
    /// </summary>
    /// <remarks>See <see cref="http://www.adafruit.com/datasheets/mcp23017.pdf"/> for more information.</remarks>
    public class Mcp23017I2cConnection
    {
        private readonly I2cDeviceConnection connection;

        public Mcp23017I2cConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;
        }

        public void SetDirection(Mcp23017Pin pin, Mcp23017PinDirection direction)
        {
            var register = ((int) pin & 0x0100) == 0x0000 ? Register.IODIRA : Register.IODIRB;

            connection.WriteByte((byte)register);
            var directions = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newDirections = (direction == Mcp23017PinDirection.Input)
                                    ? directions | bit
                                    : directions & ~bit;

            connection.Write(new[] { (byte)register, (byte)newDirections });
        }

        public void SetPolarity(Mcp23017Pin pin, Mcp23017PinPolarity polarity)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.IPOLA : Register.IPOLB;

            connection.WriteByte((byte)register);
            var polarities = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newPolarities = (polarity == Mcp23017PinPolarity.Inverted)
                                    ? polarities | bit
                                    : polarities & ~bit;

            connection.Write(new[] { (byte)register, (byte)newPolarities });
        }

        public void SetResistor(Mcp23017Pin pin, Mcp23017PinResistor resistor)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.GPPUA : Register.GPPUB;

            connection.WriteByte((byte)register);
            var resistors = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newResistors = (resistor == Mcp23017PinResistor.PullUp)
                                    ? resistors | bit
                                    : resistors & ~bit;

            connection.Write(new[] { (byte)register, (byte)newResistors });
        }

        public void SetPinStatus(Mcp23017Pin pin, bool enabled)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.GPIOA : Register.GPIOB;

            connection.WriteByte((byte)register);
            var status = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newStatus = enabled
                                    ? status | bit
                                    : status & ~bit;

            connection.Write((byte)register, (byte)newStatus);
        }

        public bool GetPinStatus(Mcp23017Pin pin)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.GPIOA : Register.GPIOB;

            connection.WriteByte((byte)register);
            var status = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            return (status & bit) != 0x00;
        }

        public void Toogle(Mcp23017Pin pin)
        {
            var register = ((int)pin & 0x0100) == 0x0000 ? Register.GPIOA : Register.GPIOB;

            connection.WriteByte((byte)register);
            var status = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var bitEnabled = (status & bit) != 0x00;
            var newBitEnabled = !bitEnabled;

            var newStatus = newBitEnabled
                                    ? status | bit
                                    : status & ~bit;

            connection.Write((byte)register, (byte)newStatus);
        }

        private enum Register
        {
            IODIRA = 0x00,
            IODIRB = 0x01,
            IPOLA = 0x02,
            IPOLB = 0x03,
            GPPUA = 0x0c,
            GPPUB = 0x0d,
            GPIOA = 0x12,
            GPIOB = 0x13
        }
    }
}