using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspberry.IO.GeneralPurpose;

namespace Test.Gpio.WatchPin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WatchPin Sample: Try to run it in parallel with i.e. Test.Gpio.HCSR04");
            Console.WriteLine();

            if (args.Length!=1)
            {
                PrintUsage();
                return;
            }

            var pinname = args[0];

            ConnectorPin userPin;
            if (!Enum.TryParse(pinname, true, out userPin))
            {
                Console.WriteLine("Could not find pin: "+pinname);
                PrintUsage();
                return;
            }

            Console.WriteLine("\tWatching Pin: {0}", userPin);
            Console.WriteLine();

            var procPin = userPin.ToProcessor();

            IGpioConnectionDriver driver = GpioConnectionSettings.DefaultDriver;

            try
            {
                driver.Allocate(procPin, PinDirection.Input);

                bool val;

                while (true)
                {
                    val = driver.Read(procPin);

                    Console.WriteLine(DateTime.Now + ": " + val);

                    driver.Wait(procPin, waitForUp: !val, timeout: 60000);
                }

                //driver.Allocate(procPin, PinDirection.Output);
                //driver.Write(procPin, false);

                //driver.Allocate(procPin, PinDirection.Input);
            } finally
            {
                //driver.Release(procPin); //not sure if its a good idea
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Test.Gpio.WatchPin [pin]"); //todo allow watch multiple pins
            Console.WriteLine("//todo allow watch multiple pins");
            Console.WriteLine("Available pins:");
            Enum.GetNames(typeof(ConnectorPin)).ToList().ForEach(Console.WriteLine);
            Console.WriteLine("//todo allow to specify pin in diffrent ways, ie, by name, by wiringPi, etc"); //todo allow to specify pin in diffrent ways, ie, by name, by wiringPi, etc
            Console.WriteLine("I.e.: sudo mono Test.Gpio.WatchPin.exe P1Pin23");
            Console.WriteLine();
        }
    }
}
