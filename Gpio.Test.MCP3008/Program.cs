using System;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

namespace Gpio.Test.MCP3008
{
    /// <summary>
    /// Freely adapted from http://learn.adafruit.com/reading-a-analog-in-and-controlling-audio-volume-with-the-raspberry-pi/overview
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            const ProcessorPin spiClock = ProcessorPin.Pin18;
            const ProcessorPin spiMiso = ProcessorPin.Pin23;
            const ProcessorPin spiMosi = ProcessorPin.Pin24;
            const ProcessorPin spiCs = ProcessorPin.Pin25;

            var pins = new PinConfiguration[]
                           {
                               spiClock.Output().Name("Clock"),
                               spiCs.Output().Name("Cs").Enable(),
                               spiMosi.Output().Name("Mosi"),
                               spiMiso.Input().Name("Miso")
                           };

            Console.CursorVisible = false;
            Console.WriteLine();
            Console.WriteLine();

            using (var connection = new GpioConnection(pins))
            {
                while (!Console.KeyAvailable)
                {
                    var volts = ReadSpiData(connection, 0, 3.3m);
                    var temperature = 100*volts - 50;

                    volts = ReadSpiData(connection, 1, 3.3m);
                    // See http://learn.adafruit.com/photocells/using-a-photocell
                    var photo = volts != 0 
                        ? 10000*(3.3m - volts)/volts 
                        : 0;

                    Console.CursorTop--;
                    Console.WriteLine("Tc = {0,5:0.0} Celsius\t\tLr = {1,7:0} Ohms", temperature, photo);
                    Thread.Sleep(150);
                }
            }

            Console.CursorVisible = true;
        }

        private static decimal ReadSpiData(GpioConnection connection, int channel, decimal tension)
        {
            connection["Cs"] = false;
            try
            {
                var command = channel;
                command |= 0x18; // start bit + single-ended bit
                command = command << 3; // we only need to send 5 bits here

                for (var i = 0; i < 5; i++)
                {
                    connection["Mosi"] = (command & 0x80) != 0;

                    connection.Blink("Clock", 1);
                    command = command << 1;
                }

                var data = 0;
                // read in one empty bit, one null bit and 10 ADC bits 
                for (var i = 0; i < 12; i++)
                {
                    connection.Blink("Clock", 1);

                    Thread.Sleep(25);

                    data = data << 1;
                    if (connection["Miso"])
                        data |= 0x1;
                }

                // first bit is 'null', drop it    
                data = data >> 1;

                return data * tension / 1023m;
            }
            finally
            {
                connection["Cs"] = true;
            }
        }
    }
}
