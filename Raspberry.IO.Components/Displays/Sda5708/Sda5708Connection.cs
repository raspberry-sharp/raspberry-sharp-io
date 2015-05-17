/* Sda5708Connection
 * Parts of this code are ported from https://github.com/pimium/sda5708
 * which in turn used the information from http://www.sbprojects.com/knowledge/footprints/sda5708.php.
 * The font is taken from http://sunge.awardspace.com/glcd-sd/node4.html
 * with additional german characters added. */
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using Raspberry.IO;
using Raspberry.IO.GeneralPurpose;
using System.Collections.Generic;

namespace Raspberry.IO.Components.Displays.Sda5708
{
	public sealed class Sda5708Connection : IDisposable
	{
		private Sda5708Brightness _brightness = Sda5708Brightness.Level100;

		private readonly ProcessorPin _load;
		private readonly ProcessorPin _data;
		private readonly ProcessorPin _sdclk;
		private readonly ProcessorPin _reset;
		private readonly GpioConnection _baseConnection;

		public Sda5708Connection () : this(
			ProcessorPin.Pin7, 
			ProcessorPin.Pin8,
			ProcessorPin.Pin18,
			ProcessorPin.Pin23)
		{
		}

		public Sda5708Connection (ProcessorPin load, ProcessorPin data, ProcessorPin sdclk, ProcessorPin reset)
		{
			this._load = load;
			this._data = data;
			this._sdclk = sdclk;
			this._reset = reset;

			this._baseConnection = new GpioConnection (
				load.Output (),
				data.Output (),
				sdclk.Output (),
				reset.Output ());

			this._baseConnection [reset] = false;
			this._baseConnection [reset] = false;
			Thread.Sleep (50);
			this._baseConnection [reset] = true;

			this.Clear();
		}

		public void SetBrightness(Sda5708Brightness brightness)
		{
			this._brightness = brightness;
			this.Write(0xe0 | (int)brightness);
		}

		public void Clear()
		{
			this.Write(0xc0 | (int)this._brightness);
		}

		public void WriteString(string str)
		{
			var chars = str
				.PadRight (8, ' ')
				.Substring (0, 8);

			for(var i = 0; i < chars.Length; i++)
			{
				this.WriteChar (i, chars [i]);
			}
		}

		private void WriteChar(int position, char value)
		{
			this.Write(0xa0 + position);

			string[] pattern;
			if (!Sda5708Font.Patterns.TryGetValue(value, out pattern))
                pattern = Sda5708Font.Patterns['?'];

			for(var i = 0; i < 7; i++)
			{
				this.Write (Convert.ToInt32 (pattern[i].Replace (' ', '0'), 2));
			}
		}

		private void Write(int value)
		{
			this._baseConnection [this._sdclk] = false;
			this._baseConnection [this._load] = false;

			for(var i = 8; i > 0; i--)
			{
				this._baseConnection[this._data] = (value & 0x1) == 0x1;

				this._baseConnection [this._sdclk] = true;
				this._baseConnection [this._sdclk] = false;

				value = value >> 1;
			}

			this._baseConnection [this._sdclk] = false;
			this._baseConnection [this._load] = true;
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			this._baseConnection [this._reset] = false;
			((IDisposable)this._baseConnection).Dispose ();
		}
		#endregion
	}
}
