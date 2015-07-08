#region References

using System;
using System.Threading;
using Raspberry.IO.Components.Expanders.Mcp23008;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

#endregion

namespace Test.Gpio.MCP23008
{
    class Program
    {
        static void Main()
        {
            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;

            Console.WriteLine("MCP-23008 Sample: Switch LED on Pin0 pin");
            Console.WriteLine();
            Console.WriteLine("\tSDA: {0}", sdaPin);
            Console.WriteLine("\tSCL: {0}", sclPin);
            Console.WriteLine();

            using (var driver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor()))
            {
                var deviceConnection = new Mcp23008I2cConnection(driver.Connect(0x20));
                deviceConnection.SetDirection(Mcp23008Pin.Pin0, Mcp23008PinDirection.Output);

                while (!Console.KeyAvailable)
                {
                    deviceConnection.Toogle(Mcp23008Pin.Pin0);
                    Thread.Sleep(1000);
                }
            }

        }
    }
}
