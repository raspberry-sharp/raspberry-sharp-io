using System;
using System.Threading;
using Raspberry.IO.Components.Expanders.Pca9685;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
using Test.Utils;


namespace Test.Gpio.PCA9685
{
    class Program
    {

        private static readonly Logger Log = new Logger();

        static void Main(string[] args)
        {
            var options = new PCA9685Options(args);

            if (options.ShowHelp)
            {
                return;
            }

            Log.Info("-=Pca9685 Sample=-");
            Log.Info("Running {0}", Environment.OSVersion);
            Log.Info("Options:");
            Log.Info(options.ToString());

            int pulse = CalculatePulse(options.PwmFrequency, 1000);
            Log.Info("Pulse={0}", pulse);

            if (Environment.OSVersion.ToString().Contains("Windows"))
            {
                Log.Info("Windows. Exiting");
                return;
            }
            
            Log.Info("Connecting...");
            
            using (var driver = new I2cDriver(options.SdaPin.ToProcessor(), options.SclPin.ToProcessor()))
            {
                Log.Info("Creating device...");
                var device = new PCA9685I2cConnection(driver.Connect(options.DeviceAddress));

                Log.Info("Setting frequency...");
                device.SetPWMUpdateRate(options.PwmFrequency);  //                        # Set frequency to 60 Hz
                while (!Console.KeyAvailable)
                {
                    Log.Info("Set channel={0} to {1}", options.Channel, options.PwmOn);
                    device.SetPWM(options.Channel, 0, options.PwmOn);
                    Thread.Sleep(1000);
                    Log.Info("Set channel={0} to {1}", options.Channel, options.PwmOff);
                    device.SetPWM(options.Channel, 0, options.PwmOff);
                    Thread.Sleep(1000);
                }

            }
        }

        private static int CalculatePulse(int frequency, int pulse)
        {
            int pulseLengthMicroSeconds = 1000000;                   //# 1,000,000 us per second
            pulseLengthMicroSeconds /= frequency;                   //    # 60 Hz
            Log.Info("{0} uSecs per period", pulseLengthMicroSeconds);
            pulseLengthMicroSeconds /= 4096; //                     # 12 bits of resolution
            Log.Info("{0} uSecs per bit", pulseLengthMicroSeconds);
            pulse *= 1000;
            pulse /= pulseLengthMicroSeconds;
            return pulse;
        }
    }
}
