using System;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

namespace Gpio.Test.MCP4802
{
    /// <summary>
    /// Freely adapted from http://www.skpang.co.uk/blog/archives/689
    /// Connected pins are custom.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var dacClock = ConnectorPin.P1Pin11.ToProcessor();
            var dacCs = ConnectorPin.P1Pin13.ToProcessor();
            var dacMosi = ConnectorPin.P1Pin15.ToProcessor();

            using (var dacConnection = new Mcp4802SpiConnection(dacClock, dacCs, dacMosi, 1))
            {
                Console.WriteLine("MC4802 Sample: Write a changing value on Channel 1");

                const decimal minimum = 0.0001m;
                var ticks = minimum;
                var up = true;

                while (!Console.KeyAvailable)
                {
                    dacConnection.WriteData(Mcp4802Channel.Channel0, ticks);

                    if (up)
                    {
                        ticks *= 2;
                        if (ticks >= 1)
                            up = false;
                    }
                    else
                    {
                        ticks /= 2;
                        if (ticks <= minimum)
                            up = true;
                    }

                    Thread.Sleep(100);
                }
            }
        }
    }
}
