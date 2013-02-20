#region References

using System;
using System.Collections.Generic;
using System.Linq;
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

            var driver = new MemoryGpioConnectionDriver();

            var triggerPin = ConnectorPin.P1Pin03.ToProcessor();
            var echoPin = ConnectorPin.P1Pin07.ToProcessor();

            driver.Allocate(triggerPin, PinDirection.Output);
            driver.Allocate(echoPin, PinDirection.Input);

            while (!Console.KeyAvailable)
            {
                var distance = GetDistance(driver, triggerPin, echoPin);
                if (distance != decimal.MinValue)
                    Console.WriteLine("{0:0.0}cm", distance*100);
                else
                    Console.WriteLine("(Timeout)");

                Timer.Sleep(interval);
            }

            driver.Release(triggerPin);
            driver.Release(echoPin);
        }

        #region Private Helpers

        private static decimal GetDistance(MemoryGpioConnectionDriver driver, ProcessorPin triggerPin, ProcessorPin echoPin)
        {
            driver.Write(triggerPin, true);
            Timer.Sleep(0.01m);
            driver.Write(triggerPin, false);

            return driver.Wait(echoPin)
                       ? Units.Velocity.Sound.ToDistance(driver.Time(echoPin))
                       : decimal.MinValue;
        }

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
