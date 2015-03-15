#region References

using System;
using System.Threading;
using Raspberry.IO.Components.Converters.Mcp3208;
using Raspberry.IO.Components.Sensors;
using Raspberry.IO.Components.Sensors.Temperature.Dht;
using Raspberry.IO.Components.Sensors.Temperature.Tmp36;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO;

using System.IO;

#endregion

namespace Test.Spi.MCP3208
{
    /// <summary>
    /// Modified from Test.Gpio.MCP3208
    /// Connected with standars SPI pins in Raspberry
    /// </summary>
    class Program
    {
        static void Main()
        {
            const ConnectorPin adcClock = ConnectorPin.P1Pin23;
            const ConnectorPin adcMiso = ConnectorPin.P1Pin21;
            const ConnectorPin adcMosi = ConnectorPin.P1Pin19;
            const ConnectorPin adcCs = ConnectorPin.P1Pin24;

            Console.Clear();

            Console.WriteLine("MCP-3208 Sample: Reading ADC points in all channels");
            Console.WriteLine();
            Console.WriteLine("\tClock: {0}", adcClock);
            Console.WriteLine("\tCS: {0}", adcCs);
            Console.WriteLine("\tMOSI: {0}", adcMosi);
            Console.WriteLine("\tMISO: {0}", adcMiso);
            Console.WriteLine();

            var driver = new GpioConnectionDriver();

            {
                Console.CursorVisible = false;
                var adcConnection = new Mcp3208SpiConnection(
                    driver.Out(adcClock),
                    driver.Out(adcCs),
                    driver.In(adcMiso),
                    driver.Out(adcMosi));

                while (!Console.KeyAvailable)
                {
                    Console.CursorTop = 0;
                    Console.Clear();

                    Mcp3208Channel chan = Mcp3208Channel.Channel0;

                    for (int i = 0; i < 8; i++)
                    {
                        AnalogValue p = adcConnection.Read(chan);
                        decimal points = p.Value;
                        Console.WriteLine(i.ToString() + " ADC points " + points.ToString());
                        using (StreamWriter sw = File.AppendText(".\\prova.txt"))
                        {
                            sw.WriteLine(chan.ToString() + " ADC points " + points.ToString());
                        }
                        chan++; // enum increase sends to the next channel 
                    }
                    Thread.Sleep(500);
                }
            }
            Console.CursorTop++;
            Console.CursorVisible = true;
        }
    }
}