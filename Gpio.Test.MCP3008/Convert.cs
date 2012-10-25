namespace Gpio.Test.MCP3008
{
    internal static class Convert
    {
        public static decimal ToCelsius(this decimal volts)
        {
            // See http://learn.adafruit.com/send-raspberry-pi-data-to-cosm
            return 100 * volts - 50;
        }

        public static decimal ToOhms(this decimal volts, decimal referenceVoltage)
        {
            // See http://learn.adafruit.com/photocells/using-a-photocell
            // and http://www.emant.com/316002.page

            const decimal resistor = 10000;
            const decimal luxRatio = 500000;

            return volts != 0
                       ? luxRatio * volts / (resistor * (referenceVoltage - volts))
                       : 0;
        }
    }
}