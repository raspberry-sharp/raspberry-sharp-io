namespace Raspberry.IO.GeneralPurpose
{
    public class InputPinConfiguration : PinConfiguration
    {
        public InputPinConfiguration(ProcessorPin pin) : base(pin){}

        public override PinDirection Direction
        {
            get { return PinDirection.Input; }
        }
    }
}