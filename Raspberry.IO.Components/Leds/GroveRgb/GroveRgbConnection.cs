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
	public class GroveRgbConnection
	{
		#region Fields

		IGpioConnectionDriver driver;
		ProcessorPin dataPin;
		ProcessorPin clockPin;
		List<RgbColor> ledColors;

		#endregion

		#region Instance Management

		public GroveRgbConnection(ConnectorPin dataPin, ConnectorPin clockPin, int ledCount)
		{
			ledColors = new List<RgbColor>();
			for(int i = 0; i < ledCount; i++)
			{
				// Initialize all leds with white color
				ledColors.Add(new RgbColor());
			}
			this.dataPin = dataPin.ToProcessor();
			this.clockPin = clockPin.ToProcessor();
			if (Raspberry.Board.Current.IsRaspberryPi) 
			{
				driver = new GpioConnectionDriver();
			}
			else 
			{
				driver = new FileGpioConnectionDriver();
			}
			driver.Allocate(this.dataPin, PinDirection.Output);
			driver.Allocate(this.clockPin, PinDirection.Output);
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
		public void SetColorRgb(int ledNumber, byte red, byte green, byte blue)
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
					ledColors[i].Red = red;
					ledColors[i].Green = green;
					ledColors[i].Blue = blue;
				}

				// Start by sending a byte with the format "1 1 /B7 /B6 /G7 /G6 /R7 /R6"
				byte prefix = Convert.ToByte ("11000000", 2);
				if ((blue & 0x80) == 0)
					prefix |= Convert.ToByte ("00100000", 2);
				if ((blue & 0x40) == 0)
					prefix |= Convert.ToByte ("00010000", 2);
				if ((green & 0x80) == 0)
					prefix |= Convert.ToByte ("00001000", 2);
				if ((green & 0x40) == 0)
					prefix |= Convert.ToByte ("00000100", 2);
				if ((red & 0x80) == 0)
					prefix |= Convert.ToByte ("00000010", 2);
				if ((red & 0x40) == 0)
					prefix |= Convert.ToByte ("00000001", 2);

				SendByte(prefix);

				// Now must send the 3 colors
				SendByte(ledColors[i].Blue);
				SendByte(ledColors[i].Green);
				SendByte(ledColors[i].Red);
			}

			// Terminate data frame (32x "0")
			SendByte(0x00);
			SendByte(0x00);
			SendByte(0x00);
			SendByte(0x00);
		}

		/// <summary>
		/// Sets the color of a led.
		/// </summary>
		/// <param name="ledNumber">Led number (zero based index).</param>
		/// <param name="h">Hue.</param>
		/// <param name="s">Saturation.</param>
		/// <param name="v">Value.</param>
		public void SetColorHsv(int ledNumber, double h, double s, double v)
		{
			RgbColor color = HsvToRgb(h, s, v);
			SetColorRgb(ledNumber, color.Red, color.Green, color.Blue);
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
					driver.Write(dataPin, true);
				else
					driver.Write(dataPin, false);

				// clk():
				driver.Write(clockPin, false);
				HighResolutionTimer.Sleep(0.02m);
				driver.Write(clockPin, true);
				HighResolutionTimer.Sleep(0.02m);

				// Advance to the next bit to send
				data <<= 1;
			}
		}

		private int Clamp(int i)
		{
			if (i < 0) i = 0;
			if (i > 255) i = 255;
			return i;
		}

		private RgbColor HsvToRgb(double hue, double sat, double val)
		{
			byte r = 0, g = 0, b = 0;
			double H = hue * 360D;
			while (H < 0) { H += 360; };
			while (H >= 360) { H -= 360; };
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
				switch (i)
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

			return new RgbColor(){ Red = r, Green = g, Blue = b };
		}

		#endregion

	}

	public class RgbColor
	{
		public byte Red;
		public byte Green;
		public byte Blue;
	}

}

