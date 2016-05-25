//Copyright (c) 2016 Logic Ethos Ltd
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Raspberry.Timers;
using Raspberry.IO.Components.Controllers.HT16K33;
using Raspberry.IO.InterIntegratedCircuit;


namespace Raspberry.IO.Components.Leds.BiColor24Bargraph
{
	public class BiColor24Bargraph : HT16K33Connection
	{
		public enum LEDState
		{
			Off,
			Red,
			Green,
			Yellow
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Raspberry.IO.Components.Leds.BiColor24Bargraph.BiColor24Bargraph"/> class.
		/// </summary>
		/// <param name="connection">I2c Connection.</param>
		public BiColor24Bargraph (I2cDeviceConnection connection) : base (connection,6)
		{
		}

		/// <summary>
		/// Sets the led (0 to 23)
		/// </summary>
		/// <param name="ledNo">Led no.</param>
		/// <param name="state">State.</param>
		public void SetLed(uint ledNo, LEDState state)
		{

			if (ledNo > 23) throw new Exception("led must be between 0 and 23");

			long r,c;
			r = Math.DivRem(ledNo,4,out c) * 2;
			if (ledNo >= 12) c += 4;

			if (r > 4) r -= 6;

			switch (state)
			{
				case LEDState.Off:
					base.SetLed((uint)r,(uint)c, false);
					base.SetLed((uint)r + 1,(uint)c, false);
					break;
				case LEDState.Red:
					base.SetLed((uint)r,(uint)c, true);
					base.SetLed((uint)r + 1,(uint)c, false);
					break;
				case LEDState.Yellow:
					base.SetLed((uint)r,(uint)c, true);
					base.SetLed((uint)r + 1,(uint)c, true);
					break;
				case LEDState.Green:
					base.SetLed((uint)r,(uint)c, false);
					base.SetLed((uint)r + 1,(uint)c, true);
					break;
			}

		}
	}
}




