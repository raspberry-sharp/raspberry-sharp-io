namespace Raspberry.IO.GeneralPurpose
{
    public class OutputPinConfiguration : PinConfiguration
    {
        #region Instance Management

        public OutputPinConfiguration(ProcessorPin pin) : base(pin){}

        #endregion

        #region Properties

        public bool Enabled { get; set; }

        public override PinDirection Direction
        {
            get { return PinDirection.Output; }
        }

        #endregion
    }
}