using Raspberry.IO.GeneralPurpose;

namespace Raspberry.IO.Components
{
    public static class GpioPinExtensionMethods
    {
        public static GpioOutputPin Out(this IGpioConnectionDriver driver, ConnectorPin pin)
        {
            return driver.Out(pin.ToProcessor());
        }

        public static GpioOutputPin Out(this IGpioConnectionDriver driver, ProcessorPin pin)
        {
            return new GpioOutputPin(driver, pin);
        }
    }
}