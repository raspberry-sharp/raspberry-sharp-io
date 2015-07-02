#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raspberry.IO;
using Raspberry.IO.Components.Displays.Hd44780;
using Raspberry.IO.Components.Expanders.Mcp23008;
using Raspberry.IO.Components.Expanders.Mcp23017;
using Raspberry.IO.Components.Expanders.Pcf8574;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

#endregion

namespace Test.Gpio.HD44780
{
    internal static class ConfigurationLoader
    {
        #region Methods

        public static Hd44780Configuration FromArguments(string[] args)
        {
            if (args.Contains("mcp23008", StringComparer.InvariantCultureIgnoreCase))
                return LoadMcp23008Configuration(args);

            if (args.Contains("mcp23017", StringComparer.InvariantCultureIgnoreCase))
                return LoadMcp23017Configuration(args);

            if (args.Contains("pcf8574", StringComparer.InvariantCultureIgnoreCase))
                return LoadPcf8574Configuration(args);

            return LoadGpioConfiguration();
        }

        #endregion
        
        #region Private Helpers

        private static Hd44780Configuration LoadMcp23008Configuration(IEnumerable<string> args)
        {
            var addressText = args.SkipWhile(s => !String.Equals(s, "mcp23008", StringComparison.InvariantCultureIgnoreCase)).Skip(1).DefaultIfEmpty("0x20").First();
            var address = addressText.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)
                ? Int32.Parse(addressText.Substring(2), NumberStyles.HexNumber)
                : Int32.Parse(addressText);

            const Mcp23008Pin registerSelectPin = Mcp23008Pin.Pin1;
            const Mcp23008Pin clockPin = Mcp23008Pin.Pin2;
            const Mcp23008Pin backlightPin = Mcp23008Pin.Pin7;

            var dataPins = new[]
            {
                Mcp23008Pin.Pin3,
                Mcp23008Pin.Pin4,
                Mcp23008Pin.Pin5,
                Mcp23008Pin.Pin6
            };

            Console.WriteLine();
            Console.WriteLine("Using I2C connection over MCP23008 Expander");
            Console.WriteLine("\tRegister Select: {0}", registerSelectPin);
            Console.WriteLine("\tClock: {0}", clockPin);
            Console.WriteLine("\tData 1: {0}", dataPins[0]);
            Console.WriteLine("\tData 2: {0}", dataPins[1]);
            Console.WriteLine("\tData 3: {0}", dataPins[2]);
            Console.WriteLine("\tData 4: {0}", dataPins[3]);
            Console.WriteLine("\tBacklight: {0}", backlightPin);
            Console.WriteLine("\tRead/write: GND");
            Console.WriteLine();

            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;

            var driver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor()) { ClockDivider = 512 };
            var connection = new Mcp23008I2cConnection(driver.Connect(address));

            var retVal = new Hd44780Configuration(driver)
            {
                Pins = new Hd44780Pins(
                    connection.Out(registerSelectPin),
                    connection.Out(clockPin),
                    dataPins.Select(pin => (IOutputBinaryPin)connection.Out(pin)))
            };

            retVal.Pins.Backlight = connection.Out(backlightPin);

            return retVal;
        }

        private static Hd44780Configuration LoadMcp23017Configuration(IEnumerable<string> args)
        {
            var addressText = args.SkipWhile(s => !String.Equals(s, "mcp23017", StringComparison.InvariantCultureIgnoreCase)).Skip(1).DefaultIfEmpty("0x20").First();
            var address = addressText.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)
                ? Int32.Parse(addressText.Substring(2), NumberStyles.HexNumber)
                : Int32.Parse(addressText);

            const Mcp23017Pin registerSelectPin = Mcp23017Pin.B7;
            const Mcp23017Pin clockPin = Mcp23017Pin.B5;
            var dataPins = new[]
            {
                Mcp23017Pin.B1,
                Mcp23017Pin.B2,
                Mcp23017Pin.B3,
                Mcp23017Pin.B4
            };

            Console.WriteLine();
            Console.WriteLine("Using I2C connection over MCP23017 Expander");
            Console.WriteLine("\tRegister Select: {0}", registerSelectPin);
            Console.WriteLine("\tClock: {0}", clockPin);
            Console.WriteLine("\tData 1: {0}", dataPins[0]);
            Console.WriteLine("\tData 2: {0}", dataPins[1]);
            Console.WriteLine("\tData 3: {0}", dataPins[2]);
            Console.WriteLine("\tData 4: {0}", dataPins[3]);
            Console.WriteLine("\tBacklight: VCC");
            Console.WriteLine("\tRead/write: GND");
            Console.WriteLine();

            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;

            var driver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor()) { ClockDivider = 512 };
            var connection = new Mcp23017I2cConnection(driver.Connect(address));

            return new Hd44780Configuration(driver)
            {
                Pins = new Hd44780Pins(
                    connection.Out(registerSelectPin),
                    connection.Out(clockPin),
                    dataPins.Select(pin => (IOutputBinaryPin)connection.Out(pin)))
            };
        }

        private static Hd44780Configuration LoadPcf8574Configuration(IEnumerable<string> args)
        {
            var addressText = args.SkipWhile(s => !String.Equals(s, "pcf8574", StringComparison.InvariantCultureIgnoreCase)).Skip(1).DefaultIfEmpty("0x20").First();
            var address = addressText.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)
                ? Int32.Parse(addressText.Substring(2), NumberStyles.HexNumber)
                : Int32.Parse(addressText);

            const Pcf8574Pin clockPin = Pcf8574Pin.P2;
            const Pcf8574Pin readWritePin = Pcf8574Pin.P1;
            const Pcf8574Pin registerSelectPin = Pcf8574Pin.P0;
            const Pcf8574Pin backlightPin = Pcf8574Pin.P3;
            var dataPins = new[]
            {
                Pcf8574Pin.P4,
                Pcf8574Pin.P5,
                Pcf8574Pin.P6,
                Pcf8574Pin.P7
            };

            Console.WriteLine();
            Console.WriteLine("Using I2C connection over PCF8574 expander");
            Console.WriteLine("\tRegister Select: {0}", registerSelectPin);
            Console.WriteLine("\tClock: {0}", clockPin);
            Console.WriteLine("\tData 1: {0}", dataPins[0]);
            Console.WriteLine("\tData 2: {0}", dataPins[1]);
            Console.WriteLine("\tData 3: {0}", dataPins[2]);
            Console.WriteLine("\tData 4: {0}", dataPins[3]);
            Console.WriteLine("\tBacklight: {0}", backlightPin);
            Console.WriteLine("\tRead/write: {0}", readWritePin);
            Console.WriteLine();

            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;

            var driver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor()) { ClockDivider = 512 };
            var connection = new Pcf8574I2cConnection(driver.Connect(address));

            return new Hd44780Configuration(driver)
            {
                Pins = new Hd44780Pins(
                    connection.Out(registerSelectPin),
                    connection.Out(clockPin),
                    dataPins.Select(p => (IOutputBinaryPin)connection.Out(p)))
                {
                    Backlight = connection.Out(backlightPin),
                    ReadWrite = connection.Out(readWritePin),
                }
            };
        }

        private static Hd44780Configuration LoadGpioConfiguration()
        {
            const ConnectorPin registerSelectPin = ConnectorPin.P1Pin22;
            const ConnectorPin clockPin = ConnectorPin.P1Pin18;
            var dataPins = new[]
            {
                ConnectorPin.P1Pin16,
                ConnectorPin.P1Pin15,
                ConnectorPin.P1Pin13,
                ConnectorPin.P1Pin11
            };

            Console.WriteLine();
            Console.WriteLine("Using GPIO connection");
            Console.WriteLine("\tRegister Select: {0}", registerSelectPin);
            Console.WriteLine("\tClock: {0}", clockPin);
            Console.WriteLine("\tData 1: {0}", dataPins[0]);
            Console.WriteLine("\tData 2: {0}", dataPins[1]);
            Console.WriteLine("\tData 3: {0}", dataPins[2]);
            Console.WriteLine("\tData 4: {0}", dataPins[3]);
            Console.WriteLine("\tBacklight: VCC");
            Console.WriteLine("\tRead/write: GND");
            Console.WriteLine();

            var driver = GpioConnectionSettings.DefaultDriver;
            return new Hd44780Configuration
            {
                Pins = new Hd44780Pins(
                    driver.Out(registerSelectPin),
                    driver.Out(clockPin),
                    dataPins.Select(p => (IOutputBinaryPin)driver.Out(p)))
            };
        }

        #endregion
    }
}