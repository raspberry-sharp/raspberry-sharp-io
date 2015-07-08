#region References

using Raspberry.IO.InterIntegratedCircuit;

#endregion

namespace Raspberry.IO.Components.Expanders.Mcp23008
{
    /// <summary>
    /// Represents a I2C connection to a MCP23008 I/O Expander.
    /// </summary>
    /// <remarks>See <see cref="http://www.adafruit.com/datasheets/MCP23008.pdf"/> for more information.</remarks>
    public class Mcp23008I2cConnection
    {
        #region Fields

        private readonly I2cDeviceConnection connection;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23008I2cConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Mcp23008I2cConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void SetDirection(Mcp23008Pin pin, Mcp23008PinDirection direction)
        {
            var register = Register.IODIR;

            connection.WriteByte((byte)register);
            var directions = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newDirections = (direction == Mcp23008PinDirection.Input)
                                    ? directions | bit
                                    : directions & ~bit;

            connection.Write(new[] { (byte)register, (byte)newDirections });
        }

        /// <summary>
        /// Sets the polarity.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="polarity">The polarity.</param>
        public void SetPolarity(Mcp23008Pin pin, Mcp23008PinPolarity polarity)
        {
            var register = Register.IPOL;

            connection.WriteByte((byte)register);
            var polarities = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newPolarities = (polarity == Mcp23008PinPolarity.Inverted)
                                    ? polarities | bit
                                    : polarities & ~bit;

            connection.Write(new[] { (byte)register, (byte)newPolarities });
        }

        /// <summary>
        /// Sets the resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public void SetResistor(Mcp23008Pin pin, Mcp23008PinResistor resistor)
        {
            var register = Register.GPPU;

            connection.WriteByte((byte)register);
            var resistors = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newResistors = (resistor == Mcp23008PinResistor.PullUp)
                                   ? resistors | bit
                                   : resistors & ~bit;

            connection.Write(new[] { (byte)register, (byte)newResistors });
        }

        /// <summary>
        /// Sets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="enabled">if set to <c>true</c>, pin is enabled.</param>
        public void SetPinStatus(Mcp23008Pin pin, bool enabled)
        {
            var register = Register.GPIO;

            connection.WriteByte((byte)register);
            var status = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            var newStatus = enabled
                                ? status | bit
                                : status & ~bit;

            connection.Write((byte)register, (byte)newStatus);
        }


        /// <summary>
        /// Gets the pin status.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin status.</returns>
        public bool GetPinStatus(Mcp23008Pin pin)
        {
            var register = Register.GPIO;

            connection.WriteByte((byte)register);
            var status = connection.ReadByte();

            var bit = (byte)((int)pin & 0xFF);
            return (status & bit) != 0x00;
        }

        /// <summary>
        /// Toogles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toogle(Mcp23008Pin pin)
        {
            var register = Register.GPIO;

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

        #endregion

        #region Private Helpers

        private enum Register
        {
            IODIR = 0x00,
            IPOL = 0x01,
            GPINTEN = 0x02,
            DEFVAL = 0x03,
            INTCON = 0x04,
            IOCON = 0x05,
            GPPU = 0x06,
            INTF = 0x07,
            INTCAP = 0x08,
            GPIO = 0x09,
            OLAT = 0x0A
        }

        #endregion

    }
}
