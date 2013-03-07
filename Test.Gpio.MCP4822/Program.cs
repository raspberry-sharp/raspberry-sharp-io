#region Fields

using System;
using System.Threading;
using Raspberry.IO.Components.Converters.Mcp4822;
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
        static void Main()
        {
            const ConnectorPin dacClock = ConnectorPin.P1Pin11;
            const ConnectorPin dacCs = ConnectorPin.P1Pin13;
            const ConnectorPin dacMosi = ConnectorPin.P1Pin15;

            Console.WriteLine("MCP-4822 Sample: Write a changing value on Channel A");
            Console.WriteLine();
            Console.WriteLine("\tClock: {0}", dacClock);
            Console.WriteLine("\tCS: {0}", dacCs);
            Console.WriteLine("\tMOSI: {0}", dacMosi);
            Console.WriteLine();

            using (var dacConnection = new Mcp4822SpiConnection(dacClock.ToProcessor(), dacCs.ToProcessor(), dacMosi.ToProcessor(), 1))
            {
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
