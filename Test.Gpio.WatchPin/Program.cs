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
            Console.WriteLine("WatchPin Sample: Try to run it in parallel with i.e. Test.Gpio.HCSR501 or Test.Gpio.HCSR04");
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
            Console.WriteLine("Press CTRL-C to stop");

            var procPin = userPin.ToProcessor();

            IGpioConnectionDriver driver = GpioConnectionSettings.DefaultDriver;

            try
            {
                driver.Allocate(procPin, PinDirection.Input);

                bool isHigh = driver.Read(procPin);

                while (true)
                {
                    DateTime now = DateTime.Now;

                    Console.WriteLine(now + "." + now.Millisecond.ToString("000") + ": " + (isHigh?"HIGH":"LOW"));

                    driver.Wait(procPin, waitForUp: !isHigh, timeout: 7*24*60*60*1000); // 1 week - //todo infinite
                    isHigh = !isHigh;
                }

                //driver.Allocate(procPin, PinDirection.Output);
                //driver.Write(procPin, false);

                //driver.Allocate(procPin, PinDirection.Input);
            } finally
            {
                //driver.Release(procPin); //not sure if its a good idea

                // lets leave it without releasing so that other processes can keep reading from this pind

                
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Test.Gpio.WatchPin [pin]"); //todo allow watch multiple pins simultanously
            Console.WriteLine("//todo allow watch multiple pins simultanously");
            Console.WriteLine("Available pins:");
            Enum.GetNames(typeof(ConnectorPin)).ToList().ForEach(Console.WriteLine);
            Console.WriteLine("//todo allow to specify pin in diffrent ways, ie, by name, by wiringPi, etc"); //todo allow to specify pin in diffrent ways, ie, by name, by wiringPi, etc
            Console.WriteLine("I.e.: sudo mono Test.Gpio.WatchPin.exe P1Pin23");
            Console.WriteLine();
        }
    }
}
