namespace Raspberry.IO.GeneralPurpose
{
    public class InputPinConfiguration : PinConfiguration
    {
        #region Instance Management

        public InputPinConfiguration(ProcessorPin pin) : base(pin){}

        #endregion

        #region Properties

        public override PinDirection Direction
        {
            get { return PinDirection.Input; }
        }

        #endregion
    }
}