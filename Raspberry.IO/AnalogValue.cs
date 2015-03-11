namespace Raspberry.IO
{
    /// <summary>
    /// Represents an analog value.
    /// </summary>
    public class AnalogValue
    {
        private decimal value;
        private decimal range;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogValue"/> class.
        /// </summary>
        /// <param name="range">The total range (ie. maximum value).</param>
        /// <param name="value">The value.</param>
        public AnalogValue(decimal value, decimal range = 1)
        {
            this.range = range;
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
        /// Gets or sets the total range.
        /// </summary>
        /// <value>
        /// The total range, ie. the maximum value.
        /// </value>
        public decimal Range
        {
            get { return range; }
            set { range = value; }
        }

        /// <summary>
        /// Gets the relative value.
        /// </summary>
        /// <value>
        /// The relative value.
        /// </value>
        public decimal Relative
        {
            get { return value / range; }
        }
    }
}