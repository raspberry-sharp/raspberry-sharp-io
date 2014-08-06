using System;

namespace Raspberry.IO.Components.Sensors
{
    public static class ResistiveDivider
    {
        /// <summary>
        /// Gets the conversion function for the upper resistor of a voltage divider.
        /// </summary>
        /// <param name="lowerResistorValue">The lower resistor value.</param>
        /// <returns>
        /// The function.
        /// </returns>
        public static Func<AnalogValue, decimal> ForUpperResistor(decimal lowerResistorValue)
        {
            return v => v.Relative != 0 
                ? lowerResistorValue * (1 - v.Relative) / v.Relative
                : decimal.MaxValue;
        }

        /// <summary>
        /// Gets the conversion function for the lower resistor of a voltage divider.
        /// </summary>
        /// <param name="upperResistorValue">The upper resistor value.</param>
        /// <returns>
        /// The function.
        /// </returns>
        public static Func<AnalogValue, decimal> ForLowerResistor(decimal upperResistorValue)
        {
            return v => v.Relative != 1 
                ? upperResistorValue * v.Relative / (1 - v.Relative)
                : decimal.MaxValue;
        }
    }
}