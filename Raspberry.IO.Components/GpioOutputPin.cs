using Raspberry.IO.GeneralPurpose;

namespace Raspberry.IO.Components
{
    public class GpioOutputPin : IOutputPin
    {
        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;

        public GpioOutputPin(IGpioConnectionDriver driver, ProcessorPin pin)
        {
            this.driver = driver;
            this.pin = pin;

            driver.Allocate(pin, PinDirection.Output);
        }

        public void Dispose()
        {
            driver.Release(pin);
        }

        public void Write(bool state)
        {
            driver.Write(pin, state);
        }
    }
}