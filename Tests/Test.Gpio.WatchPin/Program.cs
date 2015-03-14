using System;
using System.Linq;
using Raspberry.IO.GeneralPurpose;

namespace Test.Gpio.WatchPin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WatchPin Sample: log the state changes on input pin. \nTry to run it in parallel with i.e. Test.Gpio.HCSR501 or to test HCSR04 (IR motion detector) or SW-18020P (shake detector)");
            Console.WriteLine();

            if (args.Length!=1)
            {
                PrintUsage();
                return;
            }

            var pinname = args[0];

            //TODO: Allow watch multiple pins simultaneously
            //TODO: Allow to specify pin in different ways, ie, by name, by wiringPi, etc.
            ConnectorPin userPin;
            if (!Enum.TryParse(pinname, true, out userPin))
            {
                Console.WriteLine("Could not find pin: "+pinname);
                PrintUsage();
                return;
            }

            Console.WriteLine("\tWatching Pin: {0}", userPin);
            Console.WriteLine();
            Console.WriteLine("Press CTRL-C to stop");

            var procPin = userPin.ToProcessor();

            var driver = GpioConnectionSettings.DefaultDriver;

            try
            {
                driver.Allocate(procPin, PinDirection.Input);

                var isHigh = driver.Read(procPin);

                while (true)
                {
                    var now = DateTime.Now;

                    Console.WriteLine(now + "." + now.Millisecond.ToString("000") + ": " + (isHigh?"HIGH":"LOW"));

                    driver.Wait(procPin, !isHigh, TimeSpan.FromDays(7)); //TODO: infinite
                    isHigh = !isHigh;
                }
            } 
            finally
            {
                // Leaves the pin unreleased so that other processes can keep reading
                //driver.Release(procPin);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Test.Gpio.WatchPin [pin]");
            Console.WriteLine("Available pins:");
            Enum.GetNames(typeof(ConnectorPin)).ToList().ForEach(Console.WriteLine);

            Console.WriteLine("//todo allow watch multiple pins simultanously");
            Console.WriteLine("//todo allow to specify pin in diffrent ways, ie, by name, by wiringPi, etc");
            Console.WriteLine("I.e.: sudo mono Test.Gpio.WatchPin.exe P1Pin23");
            Console.WriteLine();
        }
    }
}
