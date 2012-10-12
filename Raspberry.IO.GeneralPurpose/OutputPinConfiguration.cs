namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the configuration of an output pin.
    /// </summary>
    public class OutputPinConfiguration : PinConfiguration
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputPinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public OutputPinConfiguration(ProcessorPin pin) : base(pin){}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OutputPinConfiguration"/> is enabled on connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public override PinDirection Direction
        {
            get { return PinDirection.Output; }
        }

        #endregion
    }
}