namespace Raspberry.IO.GeneralPurpose
{
    public abstract class PinConfiguration
    {
        #region Instance Management

        protected PinConfiguration(ProcessorPin pin)
        {
            Pin = pin;
        }

        #endregion

        #region Properties

        public ProcessorPin Pin { get; private set; }

        public abstract PinDirection Direction { get; }

        public string Name { get; set; }

        public bool Reversed { get; set; }

        #endregion

        #region Internal Methods

        internal bool GetEffective(bool value)
        {
            return Reversed ? !value : value;
        }

        #endregion
    }
}