namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents configuration of an input pin.
    /// </summary>
    public class InputPinConfiguration : PinConfiguration
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="InputPinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public InputPinConfiguration(ProcessorPin pin) : base(pin){}

        #endregion

        #region Properties

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public override PinDirection Direction
        {
            get { return PinDirection.Input; }
        }

        #endregion
    }
}