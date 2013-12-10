using System;
using System.Threading;
using Common.Logging;
using Raspberry.IO.Components.Controllers.Pca9685;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

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
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var options = new Pca9685Options(args);

            if (options.ShowHelp)
            {
                return;
            }

            Log.Info("-=Pca9685 Sample=-");
            Log.Info(m => m("Running {0}", Environment.OSVersion));
            Log.Info("Options:" + Environment.NewLine + options);

            int pulse = CalculatePulse(options.PwmFrequency, 50);
            Log.Info(m => m("Pulse={0}", pulse));

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                Log.Info("Windows detected. Exiting");
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
                    Log.Info(m => m("Set channel={0} to {1}", options.Channel, options.PwmOn));
                    device.SetPwm(options.Channel, 0, options.PwmOn);
                    Thread.Sleep(1000);
                    Log.Info(m => m("Set channel={0} to {1}", options.Channel, options.PwmOff));
                    device.SetPwm(options.Channel, 0, options.PwmOff);
                    Thread.Sleep(1000);
                }

            }
        }

        /// <summary>
        /// Ported but wasn't used in original? Ported from https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_PWM_Servo_Driver/Servo_Example.py
        /// Not entirely sure what the result is meant to mean.
        /// </summary>
        private static int CalculatePulse(int frequency, int pulse)
        {
            int microSeconds = 1000000;                   //# 1,000,000 us per second
            int pulseLengthMicroSeconds = microSeconds / frequency;                   //    # 60 Hz
            Log.Info(m => m("{0} uSecs per period", pulseLengthMicroSeconds));
            int microSecondsPerBit = pulseLengthMicroSeconds / 4096; //                     # 12 bits of resolution
            Log.Info(m => m("{0} uSecs per bit", microSecondsPerBit));
            pulse *= 1000;
            pulse /= pulseLengthMicroSeconds;
            return pulse;
        }
    }
}
