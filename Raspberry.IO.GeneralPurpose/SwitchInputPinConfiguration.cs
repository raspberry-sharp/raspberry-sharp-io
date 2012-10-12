namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the configuration of an input pin acting as a switch.
    /// </summary>
    public class SwitchInputPinConfiguration : InputPinConfiguration
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchInputPinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public SwitchInputPinConfiguration(ProcessorPin pin) : base(pin){}

        internal SwitchInputPinConfiguration(InputPinConfiguration pin) : base(pin.Pin)
        {
            Reversed = pin.Reversed;
            Name = pin.Name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public override PinDirection Direction
        {
            get { return PinDirection.Input; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SwitchInputPinConfiguration"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        #endregion
    }
}