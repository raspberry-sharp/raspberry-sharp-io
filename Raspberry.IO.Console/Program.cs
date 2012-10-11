using System;
using System.Collections.Generic;
using System.Linq;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;

namespace Raspberry.IO.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var driver = GetDriver(args);
            var mainboard = Mainboard.Current;

            if (!mainboard.IsRaspberryPi)
            {
                System.Console.WriteLine("{0} is not a valid processor for a Raspberry Pi.");
                return;
            }

            var leds = new PinConfiguration[]
                           {
                               ConnectorPin.P1Pin26.Output().Name("Led1").Enable(),
                               ConnectorPin.P1Pin24.Output().Name("Led2"),
                               ConnectorPin.P1Pin22.Output().Name("Led3").Enable(),
                               ConnectorPin.P1Pin15.Output().Name("Led4"),
                               ConnectorPin.P1Pin13.Output().Name("Led5").Enable(),
                               ConnectorPin.P1Pin11.Output().Name("Led6")
                           };
            
            var behavior = new ChaserBehavior(leds)
                               {
                                   Loop = GetLoop(args),
                                   RoundTrip = GetRoundTrip(args),
                                   Width = GetWidth(args),
                                   Interval = GetSpeed(args)
                               };
            /*
            var random = new Random();
            var behavior = new PatternBehavior(leds, Enumerable.Range(0, 5).Select(i => random.Next(511)))
                               {
                                   Loop = GetLoop(args),
                                   RoundTrip = GetRoundTrip(args),
                                   Interval = GetSpeed(args)
                               };*/

            /*
            var behavior = new BlinkBehavior(leds)
                               {
                                   Count = GetWidth(args),
                                   Interval = GetSpeed(args)
                               };*/
            
            using (var connection = new GpioConnection(driver))
            {
                var switchButton = ConnectorPin.P1Pin3.Input().Name("Switch").Revert().Switch().Enable().OnStatusChanged(b => behavior.RoundTrip = !behavior.RoundTrip);
                connection.Add(switchButton);

                System.Console.WriteLine("Running on Raspberry firmware rev{0}, board rev{1}, processor {2}", mainboard.FirmwareRevision, mainboard.BoardRevision, mainboard.Processor);
                System.Console.WriteLine("Using {0}, frequency {1:0.##}hz", connection.Driver.GetType().Name, 1000.0 / GetSpeed(args));

                connection.Start(behavior);

                System.Console.ReadKey(true);

                connection.Stop(behavior);
            }
        }

        private static bool GetLoop(IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-loop").Any();
        }

        private static bool GetRoundTrip(IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-roundTrip").Any();
        }
        
        private static int GetWidth(IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-width").Skip(1).Select(int.Parse).DefaultIfEmpty(1).First();
        }

        private static int GetSpeed(IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-speed").Skip(1).Select(int.Parse).DefaultIfEmpty(250).First();
        }

        private static IConnectionDriver GetDriver(IEnumerable<string> args)
        {
            var driverName = args.SkipWhile(a => a != "-driver").Skip(1).DefaultIfEmpty("").First();

            switch (driverName)
            {
                case "memory":
                    return new MemoryConnectionDriver();
                case "file":
                    return new FileConnectionDriver();
                case "":
                    return null;

                default:
                    throw new InvalidOperationException("Unsupported driver");
            }
        }
    }
}
