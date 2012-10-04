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

    public class OutputPinConfiguration : PinConfiguration
    {
        public OutputPinConfiguration(ProcessorPin pin) : base(pin){}

        public bool IsActive { get; set; }

        public override PinDirection Direction
        {
            get { return PinDirection.Output; }
        }
    }

    public abstract class PinConfiguration
    {
        private readonly ProcessorPin pin;

        protected PinConfiguration(ProcessorPin pin)
        {
            this.pin = pin;
        }
        
        public ProcessorPin Pin
        {
            get { return pin; }
        }

        public abstract PinDirection Direction { get; }

        public string Name { get; set; }

        public bool IsReversed { get; set; }

        internal bool GetEffective(bool value)
        {
            return IsReversed ? !value : value;
        }
    }
}