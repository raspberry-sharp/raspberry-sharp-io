#region References

using System;
using Common.Logging;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.Timers;
using UnitsNet;

#endregion

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
        #region Fields

        private readonly I2cDeviceConnection connection;
        
        private static readonly ILog log = LogManager.GetLogger<Pca9685Connection>();
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(5);
        
        #endregion
        
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Pca9685Connection"/> class.
        /// </summary>
        /// <param name="connection">The I2C connection.</param>
        public Pca9685Connection(I2cDeviceConnection connection)
        {
            this.connection = connection;

            log.Info(m => m("Resetting PCA9685"));
            WriteRegister(Register.MODE1, 0x00);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the PWM update rate.
        /// </summary>
        /// <param name="frequency">The frequency, in hz.</param>
        /// <remarks>Datasheet: 7.3.5 PWM frequency PRE_SCALE</remarks>
        public void SetPwmUpdateRate(Frequency frequency)
        {
            var preScale = 25000000.0m; // 25MHz
            preScale /= 4096m; // 12-bit
            preScale /= (int)frequency.Hertz;

            preScale -= 1.0m;

            log.Trace(m => m("Setting PWM frequency to {0} Hz", frequency));
            log.Trace(m => m("Estimated pre-maximum: {0}", preScale));

            var prescale = Math.Floor(preScale + 0.5m);

            log.Trace(m => m("Final pre-maximum: {0}", prescale));

            var oldmode = ReadRegister(Register.MODE1);
            var newmode = (byte) ((oldmode & 0x7F) | 0x10); // sleep


            WriteRegister(Register.MODE1, newmode); // go to sleep

            WriteRegister(Register.PRESCALE, (byte) Math.Floor(prescale));
            WriteRegister(Register.MODE1, oldmode);

            Timer.Sleep(delay);

            WriteRegister(Register.MODE1, oldmode | 0x80);
        }

        /// <summary>
        /// Sets a single PWM channel
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="on">The on values.</param>
        /// <param name="off">The off values.</param>
        public void SetPwm(PwmChannel channel, int on, int off)
        {
            WriteRegister(Register.LED0_ON_L + 4*(int) channel, on & 0xFF);
            WriteRegister(Register.LED0_ON_H + 4*(int) channel, on >> 8);
            WriteRegister(Register.LED0_OFF_L + 4*(int) channel, off & 0xFF);
            WriteRegister(Register.LED0_OFF_H + 4*(int) channel, off >> 8);
        }

        /// <summary>
        /// Set a channel to fully on or off
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="fullOn">if set to <c>true</c>, all values are on; otherwise they are all off.</param>
        public void SetFull(PwmChannel channel, bool fullOn)
        {
            if (fullOn)
                SetFullOn(channel);
            else
                SetFullOff(channel);
        }

        #endregion

        #region Private Helpers

        private enum Register
        {
            //SUBADR1 = 0x02,
            //SUBADR2 = 0x03,
            //SUBADR3 = 0x04,
            MODE1 = 0x00,
            PRESCALE = 0xFE,
            LED0_ON_L = 0x06,
            LED0_ON_H = 0x07,
            LED0_OFF_L = 0x08,
            LED0_OFF_H = 0x09,
            //ALLLED_ON_L = 0xFA,
            //ALLLED_ON_H = 0xFB,
            //ALLLED_OFF_L = 0xFC,
            //ALLLED_OFF_H = 0xFD,
        }

        private void SetFullOn(PwmChannel channel)
        {
            WriteRegister(Register.LED0_ON_H + 4*(int) channel, 0x10);
            WriteRegister(Register.LED0_OFF_H + 4*(int) channel, 0x00);
        }

        private void SetFullOff(PwmChannel channel)
        {
            WriteRegister(Register.LED0_ON_H + 4*(int) channel, 0x00);
            WriteRegister(Register.LED0_OFF_H + 4*(int) channel, 0x10);
        }

        private void WriteRegister(Register register, byte data)
        {
            connection.Write(new[] {(byte) register, data});
        }

        private void WriteRegister(Register register, int data)
        {
            WriteRegister(register, (byte) data);
        }

        private byte ReadRegister(Register register)
        {
            connection.Write((byte) register);
            var value = connection.ReadByte();
            return value;
        }

        #endregion
    }
}
