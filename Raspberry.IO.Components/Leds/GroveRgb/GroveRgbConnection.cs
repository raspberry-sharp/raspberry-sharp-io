#region References

using System;
using Raspberry.IO;
using Raspberry.Timers;
using Raspberry.IO.GeneralPurpose;
using System.Collections.Generic;

#endregion

namespace Raspberry.IO.Components.Leds.GroveRgb
{
    /// <summary>
    /// Represents a connection with Grove Chainable RGB Led modules.
    /// @see http://www.seeedstudio.com/wiki/Grove_-_Chainable_RGB_LED
    /// </summary>
    public class GroveRgbConnection: IDisposable
    {
        #region Fields

        IOutputBinaryPin dataPin;
        IOutputBinaryPin clockPin;
        List<RgbColor> ledColors;

        #endregion

        #region Instance Management

        public GroveRgbConnection(IOutputBinaryPin dataPin, IOutputBinaryPin clockPin, int ledCount)
        {
            ledColors = new List<RgbColor>();
            for (int i = 0; i < ledCount; i++)
            {
                // Initialize all leds with white color
                ledColors.Add(new RgbColor());
            }
            this.dataPin = dataPin;
            this.clockPin = clockPin;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the color of a led.
        /// </summary>
        /// <param name="ledNumber">Led number (zero based index).</param>
        /// <param name="red">Red.</param>
        /// <param name="green">Green.</param>
        /// <param name="blue">Blue.</param>
        public void SetColor (int ledNumber, RgbColor color)
        {
            // Send data frame prefix (32x "0")
            SendByte(0x00);
            SendByte(0x00);
            SendByte(0x00);
            SendByte(0x00);

            // Send color data for each one of the leds
            for (int i = 0; i < ledColors.Count; i++)
            {
                if (i == ledNumber)
                {
                    ledColors [i].Red = color.Red;
                    ledColors [i].Green = color.Green;
                    ledColors [i].Blue = color.Blue;
                }

                // Start by sending a byte with the format "1 1 /B7 /B6 /G7 /G6 /R7 /R6"
                byte prefix = Convert.ToByte("11000000", 2);
                if ((color.Blue & 0x80) == 0)
                    prefix |= Convert.ToByte("00100000", 2);
                if ((color.Blue & 0x40) == 0)
                    prefix |= Convert.ToByte("00010000", 2);
                if ((color.Green & 0x80) == 0)
                    prefix |= Convert.ToByte("00001000", 2);
                if ((color.Green & 0x40) == 0)
                    prefix |= Convert.ToByte("00000100", 2);
                if ((color.Red & 0x80) == 0)
                    prefix |= Convert.ToByte("00000010", 2);
                if ((color.Red & 0x40) == 0)
                    prefix |= Convert.ToByte("00000001", 2);

                SendByte(prefix);

                // Now must send the 3 colors
                SendByte(ledColors [i].Blue);
                SendByte(ledColors [i].Green);
                SendByte(ledColors [i].Red);
            }

            // Terminate data frame (32x "0")
            SendByte(0x00);
            SendByte(0x00);
            SendByte(0x00);
            SendByte(0x00);
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            dataPin.Dispose();
            clockPin.Dispose();
        }

        #endregion

        #region Private Helpers

        private void SendByte(byte data)
        {
            // Send one bit at a time, starting with the MSB
            for (byte i = 0; i < 8; i++)
            {
                // If MSB is 1, write one and clock it, else write 0 and clock
                if ((data & 0x80) != 0)
                    dataPin.Write(true);
                else
                    dataPin.Write(false);

                // clk():
                clockPin.Write(false);
                HighResolutionTimer.Sleep(0.02m);
                clockPin.Write(true);
                HighResolutionTimer.Sleep(0.02m);

                // Advance to the next bit to send
                data <<= 1;
            }
        }

        #endregion
    }

    public class RgbColor
    {
        public byte Red;
        public byte Green;
        public byte Blue;

        /// <summary>
        /// Gets RgbColor instance from Hsv color space.
        /// </summary>
        /// <param name="hue">Hue.</param>
        /// <param name="sat">Saturation.</param>
        /// <param name="val">Value.</param>
        public static RgbColor FromHsv(double hue, double sat, double val)
        {
            byte r = 0, g = 0, b = 0;
            double H = hue * 360D;
            while (H < 0)
            {
                H += 360;
            }
            while (H >= 360)
            {
                H -= 360;
            }
            double R, G, B;
            if (val <= 0)
            {
                R = G = B = 0; 
            } 
            else if (sat <= 0)
            {
                R = G = B = val;
            } 
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = val * (1 - sat);
                double qv = val * (1 - sat * f);
                double tv = val * (1 - sat * (1 - f));
                switch(i)
                {
                // Red is the dominant color
                case 0:
                    R = val;
                    G = tv;
                    B = pv;
                    break;
                // Green is the dominant color
                case 1:
                    R = qv;
                    G = val;
                    B = pv;
                    break;
                case 2:
                    R = pv;
                    G = val;
                    B = tv;
                    break;
                // Blue is the dominant color
                case 3:
                    R = pv;
                    G = qv;
                    B = val;
                    break;
                case 4:
                    R = tv;
                    G = pv;
                    B = val;
                    break;
                // Red is the dominant color
                case 5:
                    R = val;
                    G = pv;
                    B = qv;
                    break;
                // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                case 6:
                    R = val;
                    G = tv;
                    B = pv;
                    break;
                case -1:
                    R = val;
                    G = pv;
                    B = qv;
                    break;
                // The color is not defined, we should throw an error.
                default:
                    R = G = B = val; // Just pretend its black/white
                    break;
                }
            }
            r = (byte)Clamp((int)(R * 255.0));
            g = (byte)Clamp((int)(G * 255.0));
            b = (byte)Clamp((int)(B * 255.0));

            return new RgbColor() { Red = r, Green = g, Blue = b };
        }

        private static int Clamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }
    }
}

