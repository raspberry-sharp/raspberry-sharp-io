using System;
using System.Collections.Generic;
using System.Linq;

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extension methods for <see cref="ProcessorPin"/> and <see cref="ProcessorPins"/> objects.
    /// </summary>
    public static class ProcessorPinExtensionMethods
    {
        /// <summary>
        /// Enumerates the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>The pins.</returns>
        public static IEnumerable<ProcessorPin> Enumerate(this ProcessorPins pins)
        {
            return (Enum.GetValues(typeof (ProcessorPin)) as ProcessorPin[])
                .Distinct()
                .Where(p => (pins & (ProcessorPins) ((uint) 1 << (int) p)) != ProcessorPins.None)
                .ToArray();
        }
    }
}