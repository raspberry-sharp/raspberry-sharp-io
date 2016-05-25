using System;
using Raspberry.IO.Components.Leds.BiColor24Bargraph;
using Raspberry.IO.InterIntegratedCircuit;
using System.Threading;
using Raspberry.IO.GeneralPurpose;

namespace Test.Components.BiColor24Bargraph
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (var i2cDriver = new I2cDriver(ProcessorPin.Pin2,  ProcessorPin.Pin3))
			{
				I2cDeviceConnection i2c = i2cDriver.Connect(0x70);

				var bargraph = new BiColor24Bargraph(i2c);

				bargraph.Clear();

				while (true)
				{
					foreach (BiColor24Bargraph.LEDState state in Enum.GetValues(typeof(BiColor24Bargraph.LEDState)))
					{
						for (int i=0; i<24;i++)
						{
							bargraph.SetLed((uint)i, state);
							Thread.Sleep(50);
						}
					}
				}
			}
		}
	}
}
