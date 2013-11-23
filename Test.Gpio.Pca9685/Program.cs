using System;
using System.Threading;
using Raspberry.IO.Components.Expanders.Pca9685;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;


namespace Test.Gpio.Pca9685
{
    class Program
    {
        static void Main()
        {
            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;

            Console.WriteLine("Pca9685 Sample");
            Console.WriteLine();
            Console.WriteLine("\tSDA: {0}", sdaPin);
            Console.WriteLine("\tSCL: {0}", sclPin);
            Console.WriteLine();

            using (var driver = new I2cDriver(sdaPin.ToProcessor(), sclPin.ToProcessor()))
            {
                var device = new Pca9685I2cConnection(driver.Connect(0x40));


                int servoMin = 150;  // Min pulse length out of 4096
                int servoMax = 600;  // Max pulse length out of 4096

//def setServoPulse(channel, pulse):
//  pulseLength = 1000000                   # 1,000,000 us per second
//  pulseLength /= 60                       # 60 Hz
//  print "%d us per period" % pulseLength
//  pulseLength /= 4096                     # 12 bits of resolution
//  print "%d us per bit" % pulseLength
//  pulse *= 1000
//  pulse /= pulseLength
//  pwm.setPWM(channel, 0, pulse)

                device.SetPWMFreq(60);  //                        # Set frequency to 60 Hz
                while (!Console.KeyAvailable)
                {
                    //# Change speed of continuous servo on channel O
                    device.SetPWM(0, 0, servoMin);
                    Thread.Sleep(1000);
                    device.SetPWM(0, 0, servoMax);
                    Thread.Sleep(1000);
                }

            }
        }
    }
}
