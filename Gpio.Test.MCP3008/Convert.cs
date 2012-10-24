namespace Gpio.Test.MCP3008
{
    internal static class Convert
    {
        public static decimal ToCelsius(this decimal volts)
        {
            // See http://learn.adafruit.com/send-raspberry-pi-data-to-cosm
            return 100 * volts - 50;
        }

        public static decimal ToOhms(this decimal volts)
        {
            // See http://learn.adafruit.com/photocells/using-a-photocell
            return volts != 0
                       ? 10000 * (3.3m - volts) / volts
                       : 0;
        }
    }
}