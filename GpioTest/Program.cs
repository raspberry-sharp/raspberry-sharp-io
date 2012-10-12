using System;
using System.Linq;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;

namespace GpioTest
{
    /// <summary>
    /// This is a sample program. Must be modified to match your GPIO project.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var driver = args.GetDriver();
            var mainboard = Mainboard.Current;

            if (!mainboard.IsRaspberryPi)
            {
                Console.WriteLine("{0} is not a valid processor for a Raspberry Pi.");
                return;
            }

            // Declare outputs (leds)
            var leds = new PinConfiguration[]
                           {
                               ConnectorPin.P1Pin26.Output().Name("Led1").Enable(),
                               ConnectorPin.P1Pin24.Output().Name("Led2"),
                               ConnectorPin.P1Pin22.Output().Name("Led3").Enable(),
                               ConnectorPin.P1Pin15.Output().Name("Led4"),
                               ConnectorPin.P1Pin13.Output().Name("Led5").Enable(),
                               ConnectorPin.P1Pin11.Output().Name("Led6")
                           };
            
            // Assign a behavior to the leds
            var behavior = new ChaserBehavior(leds)
                               {
                                   Loop = args.GetLoop(),
                                   RoundTrip = args.GetRoundTrip(),
                                   Width = args.GetWidth(),
                                   Interval = args.GetSpeed()
                               };

            // Alternate behaviors...
            /*
            var random = new Random();
            var behavior = new PatternBehavior(leds, Enumerable.Range(0, 5).Select(i => random.Next(511)))
                               {
                                   Loop = Helpers.GetLoop(args),
                                   RoundTrip = Helpers.GetRoundTrip(args),
                                   Interval = Helpers.GetSpeed(args)
                               };*/

            /*
            var behavior = new BlinkBehavior(leds)
                               {
                                   Count = args.GetWidth(),
                                   Interval = args.GetSpeed()
                               };*/

            // Declare input (switchButton) interacting with the leds behavior
            var switchButton = ConnectorPin.P1Pin3.Input().Name("Switch").Revert().Switch().Enable().OnStatusChanged(b => behavior.RoundTrip = !behavior.RoundTrip);

            // Create connection
            using (var connection = new GpioConnection(driver))
            {
                connection.Add(switchButton);       

                Console.WriteLine("Running on Raspberry firmware rev{0}, board rev{1}, processor {2}", mainboard.FirmwareRevision, mainboard.BoardRevision, mainboard.Processor);
                Console.WriteLine("Using {0}, frequency {1:0.##}hz", connection.Driver.GetType().Name, 1000.0 / args.GetSpeed());

                connection.Start(behavior);     // Starting the behavior automatically registers the pins to the connection, if needed.

                Console.ReadKey(true);

                connection.Stop(behavior);
            }
        }
    }
}
