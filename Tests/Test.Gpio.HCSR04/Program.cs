#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raspberry.IO.Components.Sensors.Distance.HcSr04;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Test.Gpio.HCSR04
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.CursorVisible = false;

            const ConnectorPin triggerPin = ConnectorPin.P1Pin21;
            const ConnectorPin echoPin = ConnectorPin.P1Pin23;

            Console.WriteLine("HC-SR04 Sample: measure distance");
            Console.WriteLine();
            Console.WriteLine("\tTrigger: {0}", triggerPin);
            Console.WriteLine("\tEcho: {0}", echoPin);
            Console.WriteLine();

            var interval = GetInterval(args);
            var driver = GpioConnectionSettings.DefaultDriver;

            using (var connection = new HcSr04Connection(
                driver.Out(triggerPin.ToProcessor()), 
                driver.In(echoPin.ToProcessor())))
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        var distance = connection.GetDistance().Centimeters;
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.0}cm", distance).PadRight(16));
                        Console.CursorTop--;
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("(Timeout): " + e.Message);
                    }

                    Timer.Sleep(interval);
                }
            }

            Console.CursorVisible = true;
        }

        #region Private Helpers

        private static TimeSpan GetInterval(IEnumerable<string> args)
        {
            return TimeSpan.FromMilliseconds(args
                .SkipWhile(a => a != "-interval")
                .Skip(1)
                .Select(int.Parse)
                .DefaultIfEmpty(100)
                .First());
        }

        #endregion
    }
}
