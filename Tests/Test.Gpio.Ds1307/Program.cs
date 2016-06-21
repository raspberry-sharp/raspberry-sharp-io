using System;
using System.Threading;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.Components.Clocks.Ds1307;

namespace Test.Gpio.Ds1307
{
    class Program
    {
        private static I2cDriver driver;
        private static I2cDeviceConnection i2cConnection;
        private static Ds1307Connection clock;

        static void Main(string[] args)
        {
            Console.WriteLine("Ds1307 Test");
            Console.WriteLine("===========");

            Console.WriteLine("Opening Connection...");

            try
            {
                driver = new I2cDriver(ProcessorPin.Pin02, ProcessorPin.Pin03);
                i2cConnection = driver.Connect(0x68);
                clock = new Ds1307Connection(i2cConnection);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to open connection.");
                Console.WriteLine(ex);
                Console.WriteLine("Press any key to close.");
                Console.Read();
                Environment.Exit(1);
            }

            Console.WriteLine("Connection open!");

            AskForKey();
        }

        public static void AskForKey()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("A = EnableRtc");
            Console.WriteLine("B = DisableRtc");
            Console.WriteLine("C = GetDate (reads and shows date from the RTC every second)");
            Console.WriteLine("D = SetDate");
            Console.WriteLine("E = ResetToFactoryDefaults");
            Console.WriteLine("F = IsRtcEnabled");
            Console.WriteLine("X = Exit");
            Console.ForegroundColor = ConsoleColor.Cyan;

            while (!Console.KeyAvailable)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.A:
                        clock.EnableRtc();
                        Console.WriteLine("Clock Enabled");
                        break;
                    case ConsoleKey.B:
                        clock.DisableRtc();
                        Console.WriteLine("Clock Disabled");
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine("Press any key to stop");
                        while (!Console.KeyAvailable)
                        {
                            Console.WriteLine(clock.GetDate());
                            Thread.Sleep(1000);
                        }
                        Console.Read();
                        break;
                    case ConsoleKey.D:
                        Console.WriteLine("Enter Year ");
                        int year = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Month ");
                        int month = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Day ");
                        int day = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Hour ");
                        int hour = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Minutes ");
                        int minutes = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Seconds ");
                        int seconds = int.Parse(Console.ReadLine());

                        DateTime newDateTime = new DateTime(year, month, day, hour, minutes, seconds);

                        clock.SetDate(newDateTime);

                        Console.WriteLine("Clock set to {0}", newDateTime);

                        break;
                    case ConsoleKey.E:
                        clock.ResetToFactoryDefaults();
                        Console.WriteLine("Clock reset to factory defaults");
                        break;
                    case ConsoleKey.F:
                        Console.WriteLine("Clock is ticking: {0}", clock.IsRtcEnabled());
                        break;
                    case ConsoleKey.X:
                        driver.Dispose();
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }

                AskForKey();
            }
        }
    }
}
