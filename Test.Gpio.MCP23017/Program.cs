using System;
using System.Threading;
using Raspberry.IO.Components.Expanders.Mcp23017;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

namespace Test.Gpio.MCP23017
{
    class Program
    {
        static void Main(string[] args)
        {
            var sdaPin = ConnectorPin.P1Pin03.ToProcessor();
            var sclPin = ConnectorPin.P1Pin05.ToProcessor();

            using (var driver = new I2cDriver(sdaPin, sclPin))
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