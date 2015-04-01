using System;
using System.Threading;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;

namespace Test.Gpio.Chaser
{
    /// <summary>
    /// This is a sample program. Must be modified to match your GPIO project.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            const ConnectorPin led1Pin = ConnectorPin.P1Pin26;
            const ConnectorPin led2Pin = ConnectorPin.P1Pin24;
            const ConnectorPin led3Pin = ConnectorPin.P1Pin22;
            const ConnectorPin led4Pin = ConnectorPin.P1Pin15;
            const ConnectorPin led5Pin = ConnectorPin.P1Pin13;
            const ConnectorPin led6Pin = ConnectorPin.P1Pin11;
            const ConnectorPin buttonPin = ConnectorPin.P1Pin03;
            
            Console.WriteLine("Chaser Sample: Sample a LED chaser with a switch to change behavior");
            Console.WriteLine();
            Console.WriteLine("\tLed 1: {0}", led1Pin);
            Console.WriteLine("\tLed 2: {0}", led2Pin);
            Console.WriteLine("\tLed 3: {0}", led3Pin);
            Console.WriteLine("\tLed 4: {0}", led4Pin);
            Console.WriteLine("\tLed 5: {0}", led5Pin);
            Console.WriteLine("\tLed 6: {0}", led6Pin);
            Console.WriteLine("\tSwitch: {0}", buttonPin);
            Console.WriteLine();

            var driver = args.GetDriver();

            // Declare outputs (leds)
            var leds = new PinConfiguration[]
                           {
                               led1Pin.Output().Name("Led1").Enable(),
                               led2Pin.Output().Name("Led2"),
                               led3Pin.Output().Name("Led3").Enable(),
                               led4Pin.Output().Name("Led4"),
                               led5Pin.Output().Name("Led5").Enable(),
                               led6Pin.Output().Name("Led6")
                           };

            // Assign a behavior to the leds
            var behavior = new ChaserBehavior(leds)
                               {
                                   Loop = args.GetLoop(),
                                   RoundTrip = args.GetRoundTrip(),
                                   Width = args.GetWidth(),
                                   Interval = TimeSpan.FromMilliseconds(args.GetSpeed())
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
            var switchButton = buttonPin.Input()
                .Name("Switch")
                .Revert()
                .Switch()
                .Enable()
                .OnStatusChanged(b =>
                                     {
                                         behavior.RoundTrip = !behavior.RoundTrip;
                                         Console.WriteLine("Button switched {0}", b ? "on" : "off");
                                     });

            // Create connection
            var settings = new GpioConnectionSettings {Driver = driver};

            using (var connection = new GpioConnection(settings, leds))
            {
                Console.WriteLine("Using {0}, frequency {1:0.##}hz", settings.Driver.GetType().Name, 1000.0/args.GetSpeed());

                Thread.Sleep(1000);

                connection.Add(switchButton);
                connection.Start(behavior); // Starting the behavior automatically registers the pins to the connection, if needed.

                Console.ReadKey(true);

                connection.Stop(behavior);
            }
        }
    }
}
