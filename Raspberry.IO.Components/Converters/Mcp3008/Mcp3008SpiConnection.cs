#region References

using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.SerialPeripheralInterface;

#endregion

namespace Raspberry.IO.Components.Converters.Mcp3008
{
    /// <summary>
    /// Represents a connection to MCP3004/3008 ADC converter.
    /// </summary>
    /// <remarks>
    /// See specification at http://www.adafruit.com/datasheets/MCP3008.pdf
    /// </remarks>
    public class Mcp3008SpiConnection : IDisposable
    {
        #region Fields

        private readonly SpiConnection spiConnection;
        private readonly decimal scale;

        #endregion

        #region Instance Management

        public Mcp3008SpiConnection(ProcessorPin clock, ProcessorPin cs, ProcessorPin miso, ProcessorPin mosi, decimal scale)
        {
            this.scale = scale;
            spiConnection = new SpiConnection(clock, cs, miso, mosi, Endianness.LittleEndian);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        public decimal Read(Mcp3008Channel channel)
        {
            using(spiConnection.SelectSlave())
            {
                // Start bit
                spiConnection.Write(true);

                // Channel is single-ended
                spiConnection.Write(true);
                
                // Channel Id
                spiConnection.Write((byte)channel, 3);
                
                // Let one clock to sample
                spiConnection.Synchronize();

                // Read 10 bits
                var data = spiConnection.Read(10);
                
                return data*scale/1024m;
            }
        }

        public void Close()
        {
            spiConnection.Close();
        }

        #endregion
    }
}