using System;
using System.Collections.Generic;
using System.Linq;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

namespace Test.Gpio.HCSR04
{
    class Program
    {
        static void Main(string[] args)
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

                Timer.Sleep(interval);
            }

            driver.Release(triggerPin);
            driver.Release(echoPin);
        }

        private static decimal GetDistance(MemoryGpioConnectionDriver driver, ProcessorPin triggerPin, ProcessorPin echoPin)
        {
            driver.Write(triggerPin, true);
            Timer.Sleep(0.01m);
            var waitStart = DateTime.Now;
            driver.Write(triggerPin, false);

            while(DateTime.Now.Ticks - waitStart.Ticks < 10*1000*1000*5) // 5s max
            {
                if (driver.Read(echoPin))
                {
                    var upStart = DateTime.Now;
                    while (driver.Read(echoPin)){}
                    var upTime = (DateTime.Now.Ticks - upStart.Ticks) / 10000000m;

                    return upTime*340/2;
                    //(high level time * sound velocity (340M/S) / 2.
                }
            }

            Console.WriteLine("Timeout");
            return decimal.MinValue;
        }

        private static int GetInterval(IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-interval").Skip(1).Select(int.Parse).DefaultIfEmpty(1000).First();
        }
    }
}
