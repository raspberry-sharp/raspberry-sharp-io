#region References

using System;
using Raspberry.IO.SerialPeripheralInterface;

#endregion

namespace Raspberry.IO.Components.Converters.Mcp4822
{
    /// <summary>
    /// Represents a SPI connection to a MCP4802/4812/4822 DAC.
    /// </summary>
    /// <remarks>See http://ww1.microchip.com/downloads/en/DeviceDoc/22249A.pdf for specifications.</remarks>
    public class Mcp4822SpiConnection : IDisposable
    {
        #region Fields

        private readonly SpiConnection spiConnection;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp4822SpiConnection" /> class.
        /// </summary>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="slaveSelectPin">The slave select pin.</param>
        /// <param name="mosiPin">The mosi pin.</param>
        public Mcp4822SpiConnection(IOutputBinaryPin clockPin, IOutputBinaryPin slaveSelectPin, IOutputBinaryPin mosiPin)
        {
            spiConnection = new SpiConnection(clockPin, slaveSelectPin, null, mosiPin, Endianness.LittleEndian);
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
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            spiConnection.Close();
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="data">The data.</param>
        public void Write(Mcp4822Channel channel, AnalogValue data)
        {
            using (spiConnection.SelectSlave())
            {
                var value = (uint)(data.Relative * 0xFFF);
                if (value > 0xFFF)
                    value = 0xFFF;

                // Set active channel
                spiConnection.Write(channel == Mcp4822Channel.ChannelB);

                // Ignored bit
                spiConnection.Synchronize();

                // Select 1x Gain
                spiConnection.Write(true);

                // Active mode operation
                spiConnection.Write(true);

                // Write 12 bits data (some lower bits are ignored by MCP4802/4812
                spiConnection.Write(value, 12);
            }
        }

        #endregion
    }
}