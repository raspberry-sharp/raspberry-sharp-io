using System;
using Raspberry.IO.InterIntegratedCircuit;

namespace Raspberry.IO.Components.Controllers.Pca9685
{
    /// <summary>
    /// Driver for Adafruit 16-channel PWM/Servo Shield which uses the
    /// NXP PCA9685 16-channel, 12-bit PWM Fm+ I2C-bus LED controller
    /// Ported from
    /// https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_PWM_Servo_Driver/Adafruit_PWM_Servo_Driver.py
    /// </summary>
    public class Pca9685Connection : IPwmDevice
    {
        private readonly I2cDeviceConnection _connection;

        private enum Register
        {
           SUBADR1      = 0x02,
           SUBADR2      = 0x03,
           SUBADR3      = 0x04,
           MODE1        = 0x00,
           PRESCALE     = 0xFE,
           LED0_ON_L    = 0x06,
           LED0_ON_H    = 0x07,
           LED0_OFF_L   = 0x08,
           LED0_OFF_H   = 0x09,
           ALLLED_ON_L  = 0xFA,
           ALLLED_ON_H  = 0xFB,
           ALLLED_OFF_L = 0xFC,
           ALLLED_OFF_H = 0xFD,
        }

        private void Log(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        #region Ctor

        public static IPwmDevice Create(I2cDeviceConnection connection)
        {
            var pwmDevice = new Pca9685Connection(connection);
            return pwmDevice;
        }

        private Pca9685Connection(I2cDeviceConnection connection)
        {
            _connection = connection;
            Log("Reseting PCA9685");
            WriteRegister(Register.MODE1, 0x00);
        }

        #endregion


        /// <summary>
        /// Datasheet: 7.3.5 PWM frequency PRE_SCALE
        /// </summary>
        public void SetPwmUpdateRate(int frequencyHz)
        {
            var preScale = 25000000.0m; // 25MHz
            preScale /= 4096m;// 12-bit
            preScale /= frequencyHz;

            preScale -= 1.0m;

            Log("Setting PWM frequency to {0} Hz", frequencyHz);
            Log("Estimated pre-scale: {0}", preScale);

            var prescale = Math.Floor(preScale + 0.5m);

            Log("Final pre-scale: {0}", prescale);

            var oldmode = ReadRegister(Register.MODE1);
            var newmode = (byte) ((oldmode & 0x7F) | 0x10);      // sleep


            WriteRegister(Register.MODE1, newmode);         // go to sleep

            WriteRegister(Register.PRESCALE, (byte) Math.Floor(prescale));
            WriteRegister(Register.MODE1, oldmode);
            
            Timers.Timer.Sleep(5);
            
            WriteRegister(Register.MODE1, oldmode| 0x80);
        }

        /// <summary>
        /// Sets a single PWM channel
        /// </summary>
        public void SetPwm(PwmChannel channel, int on, int off)
        {
            WriteRegister(Register.LED0_ON_L + 4*(int)channel, on & 0xFF);
            WriteRegister(Register.LED0_ON_H + 4 * (int)channel, on >> 8);
            WriteRegister(Register.LED0_OFF_L + 4 * (int)channel, off & 0xFF);
            WriteRegister(Register.LED0_OFF_H + 4 * (int)channel, off >> 8);
        }

        public void SetFull(PwmChannel channel, bool fullOn)
        {
            if (fullOn)
            {
                SetFullOn(channel);
            }
            else
            {
                SetFullOff(channel);
            }
        }

        private void SetFullOn(PwmChannel channel)
        {
            WriteRegister(Register.LED0_ON_H + 4 * (int)channel, 0x10);
            WriteRegister(Register.LED0_OFF_H + 4 * (int)channel, 0x00);
        }

        private void SetFullOff(PwmChannel channel)
        {
            WriteRegister(Register.LED0_ON_H + 4 * (int)channel, 0x00);
            WriteRegister(Register.LED0_OFF_H + 4 * (int)channel, 0x10);
        }

        private void WriteRegister(Register register, byte data)
        {
           // Log("{0}=>{1}", register, data);
            _connection.Write(new[] { (byte)register , data });
        }

        private void WriteRegister(Register register, int data)
        {
           WriteRegister(register, (byte) data);
        }

        private byte ReadRegister(Register register)
        {
            _connection.Write((byte)register);
            var value = _connection.ReadByte();
            return value;
        }
    }
}
