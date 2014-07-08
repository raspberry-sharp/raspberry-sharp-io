#region References

using System;
using System.Linq;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Converts a bit string in MSBF order (most significant bit first) to a byte array.
        /// </summary>
        /// <param name="bitString">A bit string (e.g. "00101111").</param>
        /// <param name="prefixWithZero">If <c>true</c> the bit string will be prefixed with '0' if it is not divisible by 8.</param>
        /// <returns>An array starting with the most significant byte.</returns>
        public static byte[] BitStringToArray(this string bitString, bool prefixWithZero)
        {
            var requiredPrefixBits = bitString.Length%8;
            var @string = (requiredPrefixBits > 0 && prefixWithZero)
                ? new string('0', requiredPrefixBits) + bitString
                : bitString;

            return Enumerable
                .Range(0, @string.Length/8)
                .Select(pos => Convert.ToByte(@string.Substring(pos*8, 8), 2))
                .ToArray();
        }

        #endregion
    }
}