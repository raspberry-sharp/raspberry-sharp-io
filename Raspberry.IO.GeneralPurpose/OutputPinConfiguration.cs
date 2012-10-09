namespace Raspberry.IO.GeneralPurpose
{
    public class OutputPinConfiguration : PinConfiguration
    {
        public OutputPinConfiguration(ProcessorPin pin) : base(pin){}

        public bool Enabled { get; set; }

        public override PinDirection Direction
        {
            get { return PinDirection.Output; }
        }
    }
}