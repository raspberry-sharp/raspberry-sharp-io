#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Raspberry.IO.Components.Sensors.HcSr04;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Test.Gpio.HCSR04
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var interval = GetInterval(args);

            var triggerPin = ConnectorPin.P1Pin21.ToProcessor();
            var echoPin = ConnectorPin.P1Pin23.ToProcessor();

            using (var connection = new HcSr04Connection(triggerPin, echoPin))
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        var distance = connection.GetDistance();
                        Console.WriteLine("{0:0.0}cm", distance * 100);
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("(Timeout): " + e.Message);
                    }

                    Timer.Sleep(interval);
                }
            }
        }

        #region Private Helpers

        private static int GetInterval(IEnumerable<string> args)
        {
            return args
                .SkipWhile(a => a != "-interval")
                .Skip(1)
                .Select(int.Parse)
                .DefaultIfEmpty(1000)
                .First();
        }

        #endregion
    }
}
