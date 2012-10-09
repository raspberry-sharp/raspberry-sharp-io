namespace Raspberry.IO.GeneralPurpose
{
    public class SwitchInputPinConfiguration : InputPinConfiguration
    {
        public SwitchInputPinConfiguration(ProcessorPin pin) : base(pin){}

        internal SwitchInputPinConfiguration(InputPinConfiguration pin) : base(pin.Pin)
        {
            Reversed = pin.Reversed;
            Name = pin.Name;
        }

        public override PinDirection Direction
        {
            get { return PinDirection.Input; }
        }

        public bool Enabled { get; set; }
    }
}