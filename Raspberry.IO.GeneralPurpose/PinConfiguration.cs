namespace Raspberry.IO.GeneralPurpose
{
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

        public bool Reversed { get; set; }

        internal bool GetEffective(bool value)
        {
            return Reversed ? !value : value;
        }
    }
}