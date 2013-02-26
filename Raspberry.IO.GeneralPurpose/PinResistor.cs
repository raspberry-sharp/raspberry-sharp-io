namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the resistor enabled on an input.
    /// </summary>
    public enum PinResistor
    {
        /// <summary>
        /// No resistor is enabled on the input.
        /// </summary>
        None,

        /// <summary>
        /// A pull-down resistor is enabled.
        /// </summary>
        PullDown,

        /// <summary>
        /// A pull-up resistor is enabled.
        /// </summary>
        PullUp
    }
}