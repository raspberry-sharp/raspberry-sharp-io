using System;
using System.Threading;
using Raspberry.IO.Components.Controllers.Pca9685;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
using Test.Utils;


namespace Test.Gpio.Pca9685
{
    /// <summary>
    /// Demonstrates a connection to the Pca9685 LED controller - used by the Adafruit 16-channel PWM/Servo Shield
    /// </summary>
    /// <remarks>
    /// Ported from https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_PWM_Servo_Driver/Servo_Example.py
    /// </remarks>
    class Program
    {

        private static readonly Logger Log = new Logger();

        static void Main(string[] args)
        {
            var options = new Pca9685Options(args);

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

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                Log.Info("Windows?! Exiting");
                return;
            }
            
            Log.Info("Connecting...");
            
            using (var driver = new I2cDriver(options.SdaPin.ToProcessor(), options.SclPin.ToProcessor()))
            {
                Log.Info("Creating device...");
                var device = Pca9685Connection.Create(driver.Connect(options.DeviceAddress));

                Log.Info("Setting frequency...");
                device.SetPwmUpdateRate(options.PwmFrequency);
                while (!Console.KeyAvailable)
                {
                    Log.Info("Set channel={0} to {1}", options.Channel, options.PwmOn);
                    device.SetPwm(options.Channel, 0, options.PwmOn);
                    Thread.Sleep(1000);
                    Log.Info("Set channel={0} to {1}", options.Channel, options.PwmOff);
                    device.SetPwm(options.Channel, 0, options.PwmOff);
                    Thread.Sleep(1000);
                }

            }
        }

        /// <summary>
        /// Ported but wasn't used in original? Ported from https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_PWM_Servo_Driver/Servo_Example.py
        /// </summary>
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
