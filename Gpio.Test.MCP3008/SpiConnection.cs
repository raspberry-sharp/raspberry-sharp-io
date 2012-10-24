#region References

using System;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

#endregion

namespace Gpio.Test.MCP3008
{
    public class SpiConnection : IDisposable
    {
        #region Fields

        private readonly GpioConnection connection;
        private readonly decimal referenceVoltage;

        #endregion

        #region Instance Management

        public SpiConnection(ProcessorPin clock, ProcessorPin cs, ProcessorPin miso, ProcessorPin mosi, decimal referenceVoltage)
        {
            this.referenceVoltage = referenceVoltage;

            var settings = new GpioConnectionSettings {PollInterval = 10, BlinkDuration = 0};
            var pins = new PinConfiguration[]
                           {
                               clock.Output().Name("Clock"),
                               cs.Output().Name("Cs").Enable(),
                               mosi.Output().Name("Mosi"),
                               miso.Input().Name("Miso")
                           };

            connection = new GpioConnection(settings, pins);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        public decimal Read(SpiChannel channel)
        {
            connection["Cs"] = false;
            try
            {
                var command = (int) channel;
                command |= 0x18; // start bit + single-ended bit
                command = command << 3; // we only need to send 5 bits here

                for (var i = 0; i < 5; i++)
                {
                    connection["Mosi"] = (command & 0x80) != 0;

                    connection.Blink("Clock");
                    command = command << 1;
                }

                var data = 0;
                // read in one empty bit, one null bit and 10 ADC bits 
                for (var i = 0; i < 12; i++)
                {
                    connection.Blink("Clock");

                    Thread.Sleep(25);

                    data = data << 1;
                    if (connection["Miso"])
                        data |= 0x1;
                }

                // first bit is 'null', drop it    
                data = data >> 1;

                return data*referenceVoltage/1023m;
            }
            finally
            {
                connection["Cs"] = true;
            }
        }

        public void Close()
        {
            connection.Close();
        }

        #endregion
    }
}