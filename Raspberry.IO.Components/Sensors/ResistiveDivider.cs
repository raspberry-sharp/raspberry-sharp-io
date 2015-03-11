using System;
using UnitsNet;

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
        public static Func<AnalogValue, ElectricResistance> ForUpperResistor(ElectricResistance lowerResistorValue)
        {
            return v => v.Relative != 0
                ? lowerResistorValue * (double)((1 - v.Relative) / v.Relative)
                : ElectricResistance.FromOhms(double.MaxValue);
        }

        /// <summary>
        /// Gets the conversion function for the lower resistor of a voltage divider.
        /// </summary>
        /// <param name="upperResistorValue">The upper resistor value.</param>
        /// <returns>
        /// The function.
        /// </returns>
        public static Func<AnalogValue, ElectricResistance> ForLowerResistor(ElectricResistance upperResistorValue)
        {
            return v => v.Relative != 1
                ? upperResistorValue * (double)(v.Relative / (1 - v.Relative))
                : ElectricResistance.FromOhms(double.MaxValue);
        }
    }
}