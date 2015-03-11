#region References

using System;
using System.Threading;
using Raspberry.IO.Components.Converters.Mcp3008;
using Raspberry.IO.Components.Sensors;
using Raspberry.IO.Components.Sensors.Temperature.Dht;
using Raspberry.IO.Components.Sensors.Temperature.Tmp36;
using Raspberry.IO.GeneralPurpose;
using UnitsNet;

#endregion

namespace Test.Gpio.MCP3008
{
    /// <summary>
    /// Freely adapted from http://learn.adafruit.com/reading-a-analog-in-and-controlling-audio-volume-with-the-raspberry-pi/overview
    /// Connected pins are the same as in the original sample.
    /// </summary>
    class Program
    {
        static void Main()
        {
            const ConnectorPin adcClock = ConnectorPin.P1Pin12;
            const ConnectorPin adcMiso = ConnectorPin.P1Pin16;
            const ConnectorPin adcMosi = ConnectorPin.P1Pin18;
            const ConnectorPin adcCs = ConnectorPin.P1Pin22;

            Console.WriteLine("MCP-3008 Sample: Reading temperature on Channel 0 and luminosity on Channel 1");
            Console.WriteLine();
            Console.WriteLine("\tClock: {0}", adcClock);
            Console.WriteLine("\tCS: {0}", adcCs);
            Console.WriteLine("\tMOSI: {0}", adcMosi);
            Console.WriteLine("\tMISO: {0}", adcMiso);
            Console.WriteLine();

            ElectricPotential voltage = ElectricPotential.FromVolts(3.3);

            var driver = new MemoryGpioConnectionDriver(); //GpioConnectionSettings.DefaultDriver;

            using (var adcConnection = new Mcp3008SpiConnection( 
                driver.Out(adcClock), 
                driver.Out(adcCs), 
                driver.In(adcMiso), 
                driver.Out(adcMosi)))
            using (var temperatureConnection = new Tmp36Connection(
                adcConnection.In(Mcp3008Channel.Channel0),
                voltage))
            using (var lightConnection = new VariableResistiveDividerConnection(
                adcConnection.In(Mcp3008Channel.Channel1), 
                ResistiveDivider.ForLowerResistor(ElectricResistance.FromKiloohms(10))))
            {
                Console.CursorVisible = false;

                while (!Console.KeyAvailable)
                {
                    var temperature = temperatureConnection.GetTemperature();
                    var resistor = lightConnection.GetResistance();
                    var lux = resistor.ToLux();

                    Console.WriteLine("Temperature = {0,5:0.0} °C\tLight = {1,5:0.0} Lux ({2} ohms)", temperature, lux, (int)resistor.Ohms);

                    Console.CursorTop--;

                    Thread.Sleep(1000);
                }
            }

            Console.CursorTop++;
            Console.CursorVisible = true;
        }
    }
}
