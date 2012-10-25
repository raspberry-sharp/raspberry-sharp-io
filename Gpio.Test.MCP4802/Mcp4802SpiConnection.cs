using System;
using Raspberry.IO.GeneralPurpose;

namespace Gpio.Test.MCP4802
{
    public class Mcp4802SpiConnection : IDisposable
    {
        #region Fields

        private readonly MemoryGpioConnectionDriver driver;
        private readonly ProcessorPin clock;
        private readonly ProcessorPin cs;
        private readonly ProcessorPin mosi;
        private readonly decimal scale;

        #endregion

        public Mcp4802SpiConnection(ProcessorPin clock, ProcessorPin cs, ProcessorPin mosi, decimal scale)
        {
            this.clock = clock;
            this.cs = cs;
            this.mosi = mosi;
            this.scale = scale;

            driver = new MemoryGpioConnectionDriver();

            driver.Allocate(clock, PinDirection.Output);
            driver.Allocate(cs, PinDirection.Output);
            driver.Allocate(mosi, PinDirection.Output);

            driver.Write(clock, false);
            driver.Write(cs, true);
            driver.Write(mosi, false);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        private void Close()
        {
            driver.Release(clock);
            driver.Release(cs);
            driver.Release(mosi);
        }

        public void WriteData(SpiChannel channel, decimal data)
        {
            driver.Write(cs, false);
            try
            {
                var value = (int) (data*4096m/scale);
                if (value > 0xFFF)
                    value = 0xFFF;

                var command = ((int) channel << 15) | 0x3000 | value;

                for (var i = 0; i < 16; i++)
                {
                    driver.Write(mosi, (command & 0x8000) != 0);

                    driver.Write(clock, true);
                    driver.Write(clock, false);

                    command = command << 1;
                }
            }
            finally
            {
                driver.Write(cs, true);
            }
        }
    }
}