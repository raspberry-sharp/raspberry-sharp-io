using System;

namespace Raspberry.IO.Components.Leds.GroveRgb
{
    public class RgbColor
    {
        #region Instance Management

        public RgbColor()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        public RgbColor(byte r, byte g, byte b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        #endregion

        #region Properties

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        #endregion

        #region Methods

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

        #endregion

        #region Private Helpers

        private static int Clamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return i;
        }

        #endregion
    }
}

