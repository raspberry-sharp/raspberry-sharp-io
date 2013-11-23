using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Raspberry.IO.InterIntegratedCircuit;


namespace Raspberry.IO.Components.Expanders.Pca9685
{
    /*
     * 
  i2c = None

  # Registers/etc.
  __SUBADR1            = 0x02
  __SUBADR2            = 0x03
  __SUBADR3            = 0x04
  __MODE1              = 0x00
  __PRESCALE           = 0xFE
  __LED0_ON_L          = 0x06
  __LED0_ON_H          = 0x07
  __LED0_OFF_L         = 0x08
  __LED0_OFF_H         = 0x09
  __ALLLED_ON_L        = 0xFA
  __ALLLED_ON_H        = 0xFB
  __ALLLED_OFF_L       = 0xFC
  __ALLLED_OFF_H       = 0xFD

  def __init__(self, address=0x40, debug=False):
    self.i2c = Adafruit_I2C(address)
    self.address = address
    self.debug = debug
    if (self.debug):
      print "Reseting PCA9685"
    self.i2c.write8(self.__MODE1, 0x00)

  def setPWMFreq(self, freq):
    "Sets the PWM frequency"
    prescaleval = 25000000.0    # 25MHz
    prescaleval /= 4096.0       # 12-bit
    prescaleval /= float(freq)
    prescaleval -= 1.0
    if (self.debug):
      print "Setting PWM frequency to %d Hz" % freq
      print "Estimated pre-scale: %d" % prescaleval
    prescale = math.floor(prescaleval + 0.5)
    if (self.debug):
      print "Final pre-scale: %d" % prescale

    oldmode = self.i2c.readU8(self.__MODE1);
    newmode = (oldmode & 0x7F) | 0x10             # sleep
    self.i2c.write8(self.__MODE1, newmode)        # go to sleep
    self.i2c.write8(self.__PRESCALE, int(math.floor(prescale)))
    self.i2c.write8(self.__MODE1, oldmode)
    time.sleep(0.005)
    self.i2c.write8(self.__MODE1, oldmode | 0x80)

  def setPWM(self, channel, on, off):
    "Sets a single PWM channel"
    self.i2c.write8(self.__LED0_ON_L+4*channel, on & 0xFF)
    self.i2c.write8(self.__LED0_ON_H+4*channel, on >> 8)
    self.i2c.write8(self.__LED0_OFF_L+4*channel, off & 0xFF)
    self.i2c.write8(self.__LED0_OFF_H+4*channel, off >> 8)
     * 
     * */

    /// <summary>
    /// https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_PWM_Servo_Driver/Adafruit_PWM_Servo_Driver.py
    /// </summary>
    public class Pca9685I2cConnection
    {
        private readonly I2cDeviceConnection connection;

        private enum Register
        {
           SUBADR1 = 0x02,
           SUBADR2            = 0x03,
           SUBADR3            = 0x04,
           MODE1              = 0x00,
           PRESCALE           = 0xFE,
           LED0_ON_L          = 0x06,
           LED0_ON_H          = 0x07,
           LED0_OFF_L         = 0x08,
           LED0_OFF_H         = 0x09,
           ALLLED_ON_L        = 0xFA,
           ALLLED_ON_H        = 0xFB,
           ALLLED_OFF_L       = 0xFC,
           ALLLED_OFF_H       = 0xFD,

        }

        private void Log(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        public Pca9685I2cConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;

            Log("Reseting PCA9685");
            WriteRegister(Register.MODE1, 0x00);
        }

        public void SetPWMFreq(int freq)
        {
            Log("Sets the PWM frequency");
            decimal prescaleval = 25000000.0m; // 25MHz
            prescaleval /= 4096.0m;// 12-bit
            prescaleval /= freq;

            prescaleval -= 1.0m;

            Log("Setting PWM frequency to {0} Hz", freq);
            Log("Estimated pre-scale: {0}", prescaleval);

            var prescale = Math.Floor(prescaleval + 0.5m);

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
        public void SetPWM(int channel, int on, int off)
        {
            WriteRegister(Register.LED0_ON_L + 4*channel, on & 0xFF);
            WriteRegister(Register.LED0_ON_H + 4*channel, on >> 8);
            WriteRegister(Register.LED0_OFF_L + 4*channel, off & 0xFF);
            WriteRegister(Register.LED0_OFF_H + 4*channel, off >> 8);
        }

        private void WriteRegister(Register register, byte data)
        {
            Log("{0}=>{1}", register, data);
            connection.Write(new[] { (byte)register , data });
        }

        private void WriteRegister(Register register, int data)
        {
           WriteRegister(register, (byte) data);
        }

        private byte ReadRegister(Register register)
        {
            connection.Write((byte)register);
            var value = connection.ReadByte();
            return value;
        }


    }
}
