//Copyright (c) 2016 Logic Ethos Ltd
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using Common.Logging;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.Timers;
using UnitsNet;
using System.IO;

namespace Raspberry.IO.Components.Controllers.HT16K33
{

    /// <summary>
	/// Driver for Holtek HT16K33 LED Matrix driver
	/// As used by Adafruit devices
    /// </summary>
	public class HT16K33Connection //: IPwmDevice
    {


    public enum Flash : byte
    {
		Off 		= 0x00,
		On		 	= 0x01,
		TwoHZ 		= 0x02,
		OneHZ 		= 0x04,
		HalfHZ 		= 0x06,
    }

	public enum Command : byte
    {
    	DisplayDataAddress		= 0x00,
		System_Setup			= 0x20,
		KeyDataAddressPointer	= 0x40,
		INTFlagAddressPointer	= 0x60,
		Flash			 		= 0x80,
		RowIntSet				= 0xA0,
		DimmingSet				= 0xE0,
		TestMode				= 0xD9,
    }

		const byte DEFAULT_ADDRESS			= 0x70;
		const byte HT16K33_Oscillator		= 0x01;
		const byte HT16K33_DisplayOn		= 0x01;

        private readonly I2cDeviceConnection connection;       
		private static readonly ILog log = LogManager.GetLogger<HT16K33Connection>();

        public byte[] LEDBuffer {get; private set;}  //Max 16 rows, 8 bits (leds)


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Raspberry.IO.Components.Controllers.HT16K33.HT16K33Connection"/> class.
        /// </summary>
        /// <param name="connection">I2c connection.</param>
        /// <param name="RowCount">Rows in use (1 to 16) </param>
		public HT16K33Connection(I2cDeviceConnection connection, int RowCount)
        {
			LEDBuffer = new byte[RowCount];
            this.connection = connection;
			
			log.Info(m => m("Resetting HT16K33"));

			connection.Write((byte)Command.System_Setup | (byte)HT16K33_Oscillator); //Turn on the oscillator.
			connection.Write((byte)Command.Flash | (byte)HT16K33_DisplayOn | (byte)Flash.Off);
			connection.Write((byte)Command.DimmingSet | (byte)15);

		//	connection.Write(SetupSequence);
        }


		/// <summary>
		/// Flash display at specified frequency.
		/// </summary>
		/// <param name="">.</param>
		public void SetFlash(Flash frequency)
		{
			connection.WriteByte((byte)((byte)Command.Flash | HT16K33_DisplayOn | (byte)frequency));
		}

		/// <summary>
		/// Set brightness of entire display to specified value (0 to 15).
		/// </summary>
		/// <param name="">.</param>
		public void SetBrightness(uint brightness)
		{
			if (brightness > 15) brightness = 15;
					connection.WriteByte((byte)((byte)Command.DimmingSet | (byte)brightness));
		}

		/// <summary>
		/// Sets specified LED (0-[row-count] rows, 0 to 7 leds)
		/// </summary>
		/// <param name="">.</param>
		public void SetLed(uint row, uint led, bool OutputOn)
		{
			if (row >= LEDBuffer.Length) throw new Exception("Row out of range");
			if (led > 7) throw new Exception("LED out of range 0 to 7");

			if (OutputOn)
			{	
				LEDBuffer[row] |= (byte)(1 << (int)led); //Turn on the speciried LED (set bit to one).			
			}
			else
			{
				LEDBuffer[row] &= (byte)~(1 << (int)led);  //Turn off the specified LED (set bit to zero).
			}
			connection.Write(new byte[] {(byte)row, LEDBuffer[row]});
		}

			
		/// <summary>
		/// Write display buffer to display hardware.
		/// </summary>
		/// <param name="">.</param>
		public void WriteDisplayBuffer()
		{
			for (int i = 0; i < LEDBuffer.Length;i++)
			{
				connection.Write((byte)i,LEDBuffer[i]);
			}
		}

		/// <summary>
		/// Clear contents of display buffer.
		/// </summary>
		/// <param name="">.</param>
		public void Clear()
		{
			for (int i = 0; i < LEDBuffer.Length;i++)
			{
				LEDBuffer[i] = 0;
			}
			WriteDisplayBuffer();
		}

		/// <summary>
		/// Set all LEDs On.
		/// </summary>
		/// <param name="">.</param>
		public void SetAllOn()
		{
			for (int i = 0; i < LEDBuffer.Length;i++)
			{
				LEDBuffer[i] = 1;
			}
			WriteDisplayBuffer();
		}

    }
}
