namespace Raspberry.IO.GeneralPurpose
{
    public class SwitchInputPinConfiguration : InputPinConfiguration
    {
        #region Instance Management

        public SwitchInputPinConfiguration(ProcessorPin pin) : base(pin){}

        internal SwitchInputPinConfiguration(InputPinConfiguration pin) : base(pin.Pin)
        {
            Reversed = pin.Reversed;
            Name = pin.Name;
        }

        #endregion

        #region Properties

        public override PinDirection Direction
        {
            get { return PinDirection.Input; }
        }

        public bool Enabled { get; set; }

        #endregion
    }
}