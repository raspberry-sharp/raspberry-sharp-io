using System;
using System.Linq;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

namespace Raspberry.IO.Console
{
    class Program
    {
        private class Status
        {
            public Connection Connection { get; set; }
            public int Current { get; set; }
            public bool Descending { get; set; }
        }

        static void Main(string[] args)
        {
            var driverName = args.SkipWhile(a => a != "-driver").Skip(1).DefaultIfEmpty("memory").First();
            
            IConnectionDriver driver;
            switch(driverName)
            {
                case "memory":
                    driver = new ConnectionMemoryDriver();
                    break;
                case "file":
                    driver = new ConnectionFileDriver();
                    break;
                default:
                    throw new InvalidOperationException("Unsupported driver");
            }

            var speed = args.SkipWhile(a => a != "-speed").Skip(1).Select(int.Parse).DefaultIfEmpty(250).First();

            var mainboard = Mainboard.Current;

            if (!mainboard.IsRaspberryPi)
            {
                System.Console.WriteLine("{0} is not a valid processor for a Raspberry Pi.");
                return;
            }

            System.Console.WriteLine("Running on Raspberry firmware rev{0}, board rev{1}, processor {2}", mainboard.FirmwareRevision, mainboard.BoardRevision, mainboard.Processor);
            System.Console.WriteLine("Using {0} driver, frequency {1:0.##}hz", driverName, 1000.0 / speed);

            var pins = new PinConfiguration[]
                           {
                               ConnectorPin.P1Pin26.Output().Name("Led1").Enable(),
                               ConnectorPin.P1Pin24.Output().Name("Led2"),
                               ConnectorPin.P1Pin22.Output().Name("Led3").Enable(),
                               ConnectorPin.P1Pin15.Output().Name("Led4"),
                               ConnectorPin.P1Pin13.Output().Name("Led5").Enable(),
                               ConnectorPin.P1Pin11.Output().Name("Led6"),
                               ConnectorPin.P1Pin3.Input().Name("Switch").Revert().Switch().Enable()
                           };

            using (var connection = new Connection(driver, pins))
            {
                var status = new Status { Connection = connection };

                connection.InputPinChanged += (sender, pinStatusEventArgs) =>
                                                  {
                                                      System.Console.WriteLine("[{0:HH:mm:ss}] Pin {1}: {2}", DateTime.UtcNow, pinStatusEventArgs.Pin.Name ?? Convert.ToString((int) pinStatusEventArgs.Pin.Pin), pinStatusEventArgs.Enabled ? "Enabled" : "Disabled");
                                                      status.Descending = !pinStatusEventArgs.Enabled;
                                                  };

                using (new Timer(Animate, status, 0, speed))
                    System.Console.ReadLine();
            }
        }

        private static void Animate(object state)
        {
            var status = (Status) state;

            var connection = status.Connection;
            var i = status.Current;

            connection["Led1"] = (i == 0);
            connection["Led2"] = (i == 1);
            connection["Led3"] = (i == 2);
            connection["Led4"] = (i == 3);
            connection["Led5"] = (i == 4);
            connection["Led6"] = (i == 5);

            if (!status.Descending)
                status.Current = (i + 1) % 6;
            else
                status.Current = (6 + (i - 1)) % 6;

            /*
            connection["Led1"] = (i & 0x1) == 0x1;
            connection["Led2"] = (i & 0x20) == 0x20;
            connection["Led3"] = (i & 0x4) == 0x4;
            connection["Led4"] = (i & 0x10) == 0x10;
            connection["Led5"] = (i & 0x8) == 0x8;
            connection["Led6"] = (i & 0x40) == 0x40;

            status.Current = (i + 1) % 0x40;
            */
        }
    }
}
