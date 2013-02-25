#region Fields

using System;
using System.Threading;
using Raspberry.IO.Components.Converter.Mcp4822;
using Raspberry.IO.GeneralPurpose;

#endregion

namespace Test.Gpio.MCP4822
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

            using (var dacConnection = new Mcp4822SpiConnection(dacClock, dacCs, dacMosi, 1))
            {
                Console.WriteLine("MC4822 Sample: Write a changing value on Channel A");

                const decimal minimum = 0.0001m;
                var ticks = minimum;
                var up = true;

                while (!Console.KeyAvailable)
                {
                    dacConnection.Write(Mcp4822Channel.ChannelA, ticks);

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
