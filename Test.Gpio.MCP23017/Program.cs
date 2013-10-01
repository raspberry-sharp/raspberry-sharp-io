#region References

using System;
using System.Threading;
using Raspberry.IO.Components.Expanders.Mcp23017;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

#endregion

namespace Test.Gpio.MCP23017
{
    class Program
    {
        static void Main()
        {
            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;
            
            Console.WriteLine("MCP-23017 Sample: Switch LED on B0 pin");
            Console.WriteLine();
            Console.WriteLine("\tSDA: {0}", sdaPin);
            Console.WriteLine("\tSCL: {0}", sclPin);
            Console.WriteLine();

            using (var driver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor()))
            {
                var deviceConnection =  new Mcp23017I2cConnection(driver.Connect(0x20));
                deviceConnection.SetDirection(Mcp23017Pin.B0, Mcp23017PinDirection.Output);

                while (!Console.KeyAvailable)
                {
                    deviceConnection.Toogle(Mcp23017Pin.B0);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}