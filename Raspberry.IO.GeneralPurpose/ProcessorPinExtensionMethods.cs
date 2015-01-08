#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extension methods for <see cref="ProcessorPin"/> and <see cref="ProcessorPins"/> objects.
    /// </summary>
    public static class ProcessorPinExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Enumerates the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>The pins.</returns>
        public static IEnumerable<ProcessorPin> Enumerate(this ProcessorPins pins)
        {
            return ((Enum.GetValues(typeof (ProcessorPin)) as ProcessorPin[]) ?? new ProcessorPin[0])
                .Distinct()
                .Where(p => (pins & (ProcessorPins) ((uint) 1 << (int) p)) != ProcessorPins.None)
                .ToArray();
        }

        public static string ToGpioName(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(
                    typeof(GpioName),
                    false);

                if (attrs != null && attrs.Length > 0)
                    return ((GpioName)attrs[0]).Name;
            }
            return en.ToString();
        }

        #endregion
    }
}