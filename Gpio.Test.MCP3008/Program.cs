#region References

using System;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

#endregion

namespace Gpio.Test.MCP3008
{
    /// <summary>
    /// Freely adapted from http://learn.adafruit.com/reading-a-analog-in-and-controlling-audio-volume-with-the-raspberry-pi/overview
    /// Connected pins are the same as in the original sample.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var clock = ConnectorPin.P1Pin12.ToProcessor();
            var miso = ConnectorPin.P1Pin16.ToProcessor();
            var mosi = ConnectorPin.P1Pin18.ToProcessor();
            var cs = ConnectorPin.P1Pin22.ToProcessor();

            const decimal voltage = 3.3m;

            using (var connection = new SpiConnection(clock, cs, miso, mosi, voltage))
            {
                Console.CursorVisible = false;
                Console.WriteLine("MCP3008 Sample: Reading temperature on Channel 0 and light resistor on Channel 1");
                Console.WriteLine();

                while (!Console.KeyAvailable)
                {
                    var temperature = connection.Read(SpiChannel.Channel0).ToCelsius();
                    var lightResistor = connection.Read(SpiChannel.Channel1).ToOhms();

                    Console.WriteLine("Tc = {0,5:0.0} Celsius\t\tLr = {1,7:0} Ohms", temperature, lightResistor);
                    Console.CursorTop--;

                    Thread.Sleep(150);
                }
            }

            Console.CursorTop++;
            Console.CursorVisible = true;
        }
    }
}
