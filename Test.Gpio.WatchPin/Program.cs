using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var pin = args[0];

            Console.WriteLine("\tPin: {0}", pin);
            Console.WriteLine();

            Console.WriteLine("//todo actually watch the pin"); //todo
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Test.Gpio.WatchPin [pin]"); //todo watch multiple pins
            Console.WriteLine("//todo watch multiple pins"); 
            Console.WriteLine();
        }
    }
}
