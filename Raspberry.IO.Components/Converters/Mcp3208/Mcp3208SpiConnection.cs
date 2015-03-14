#region References

using System;
using Raspberry.IO.SerialPeripheralInterface;

#endregion

namespace Raspberry.IO.Components.Converters.Mcp3208
{
    /// <summary>
    /// Represents a connection to MCP3204/3208 ADC converter.
    /// </summary>
    /// <remarks>
    /// See specification at http://www.adafruit.com/datasheets/MCP3208.pdf
    /// </remarks>
    public class Mcp3208SpiConnection : IDisposable
    {
        #region Fields

        private readonly SpiConnection spiConnection;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp3208SpiConnection"/> class.
        /// </summary>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="slaveSelectPin">The slave select pin.</param>
        /// <param name="misoPin">The miso pin.</param>
        /// <param name="mosiPin">The mosi pin.</param>
        public Mcp3208SpiConnection(IOutputBinaryPin clockPin, IOutputBinaryPin slaveSelectPin, IInputBinaryPin misoPin, IOutputBinaryPin mosiPin)
        {
            spiConnection = new SpiConnection(clockPin, slaveSelectPin, misoPin, mosiPin, Endianness.LittleEndian);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>The value</returns>
        public AnalogValue Read(Mcp3208Channel channel)
        {
            using (spiConnection.SelectSlave())
            {
                // Start bit
                spiConnection.Write(true);

                // Channel is single-ended
                spiConnection.Write(true);

                spiConnection.Write((byte)channel, 3);

                // Let one clock to sample
                spiConnection.Synchronize();

                // Read 12 bits
                var data = (int)spiConnection.Read(12);

                return new AnalogValue(data, 0xFFF);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            spiConnection.Close();
        }

        #endregion
    }
}