#region References

using System;
using Raspberry.IO.GeneralPurpose;

#endregion

namespace Gpio.Test.MCP3008
{
    public class Mcp3008SpiConnection : IDisposable
    {
        #region Fields

        private readonly MemoryGpioConnectionDriver driver;
        private readonly ProcessorPin clock;
        private readonly ProcessorPin cs;
        private readonly ProcessorPin miso;
        private readonly ProcessorPin mosi;
        private readonly decimal scale;

        #endregion

        #region Instance Management

        public Mcp3008SpiConnection(ProcessorPin clock, ProcessorPin cs, ProcessorPin miso, ProcessorPin mosi, decimal scale)
        {
            this.clock = clock;
            this.cs = cs;
            this.miso = miso;
            this.mosi = mosi;
            this.scale = scale;

            driver = new MemoryGpioConnectionDriver();

            driver.Allocate(clock, PinDirection.Output);
            driver.Allocate(cs, PinDirection.Output);
            driver.Allocate(mosi, PinDirection.Output);
            driver.Allocate(miso, PinDirection.Input);

            driver.Write(clock, false);
            driver.Write(cs, true);
            driver.Write(mosi, false);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        public decimal Read(SpiChannel channel)
        {
            driver.Write(cs, false);
            try
            {
                var command = (int) channel;
                command |= 0x18; // start bit + single-ended bit
                command = command << 3; // we only need to send 5 bits here

                for (var i = 0; i < 5; i++)
                {
                    driver.Write(mosi, (command & 0x80) != 0);

                    driver.Write(clock, true);
                    driver.Write(clock, false);

                    command = command << 1;
                }

                var data = 0;
                // read in one empty bit, one null bit and 10 ADC bits 
                for (var i = 0; i < 12; i++)
                {
                    driver.Write(clock, true);
                    driver.Write(clock, false);

                    data = data << 1;
                    if (driver.Read(miso))
                        data |= 0x1;
                }

                // first bit is 'null', drop it    
                data = data >> 1;

                return data*scale/1024m;
            }
            finally
            {
                driver.Write(cs, true);
            }
        }

        public void Close()
        {
            driver.Release(clock);
            driver.Release(cs);
            driver.Release(mosi);
            driver.Release(miso);
        }

        #endregion
    }
}