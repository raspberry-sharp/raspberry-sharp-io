#region References

using System;
using Raspberry.IO.GeneralPurpose;
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
        private readonly decimal scale;

        #endregion

        #region Instance Management

        public Mcp4822SpiConnection(ProcessorPin clock, ProcessorPin ss, ProcessorPin mosi, decimal scale)
        {
            spiConnection = new SpiConnection(clock, ss, null, mosi, Endianness.LittleEndian);
            this.scale = scale;
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        public void Close()
        {
            spiConnection.Close();
        }

        public void Write(Mcp4822Channel channel, decimal data)
        {
            using (spiConnection.SelectSlave())
            {
                var value = (uint) (data*4096m/scale);
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