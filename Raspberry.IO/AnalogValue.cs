namespace Raspberry.IO
{
    /// <summary>
    /// Represents an analog value.
    /// </summary>
    public class AnalogValue
    {
        private decimal discrete;
        private decimal scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogValue"/> class.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <param name="discrete">The value.</param>
        public AnalogValue(decimal discrete, decimal scale = 1)
        {
            this.scale = scale;
            this.discrete = discrete;
        }

        /// <summary>
        /// Gets or sets the discrete value.
        /// </summary>
        /// <value>
        /// The discrete value.
        /// </value>
        public decimal Discrete
        {
            get { return discrete; }
            set { discrete = value; }
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public decimal Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets the relative value.
        /// </summary>
        /// <value>
        /// The relative value.
        /// </value>
        public decimal Relative
        {
            get { return (1.0m*discrete)/scale; }
        }
    }
}