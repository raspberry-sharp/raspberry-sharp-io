namespace Raspberry.IO
{
    /// <summary>
    /// Represents an analog value.
    /// </summary>
    public class AnalogValue
    {
        private decimal value;
        private decimal maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogValue"/> class.
        /// </summary>
        /// <param name="maximum">The maximum.</param>
        /// <param name="value">The value.</param>
        public AnalogValue(decimal value, decimal maximum = 1)
        {
            this.maximum = maximum;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the discrete value.
        /// </summary>
        /// <value>
        /// The discrete value.
        /// </value>
        public decimal Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public decimal Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        /// <summary>
        /// Gets the relative value.
        /// </summary>
        /// <value>
        /// The relative value.
        /// </value>
        public decimal Relative
        {
            get { return value / maximum; }
        }
    }
}