#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides extension methods for byte and byte arrays.
    /// </summary>
    public static class ByteExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Converts a byte array/enumerable to a bit string.
        /// </summary>
        /// <param name="bytes">bytes to be converted.</param>
        /// <returns>A bit string</returns>
        public static string ToBitString(this IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder(255);
            foreach (var value in bytes.Select(@byte => Convert.ToString(@byte, 2)))
            {
                if (value.Length < 8)
                {
                    sb.Append(new string('0', 8 - value.Length));
                }
                sb.Append(value);
            }
            return sb.ToString();
        }

        #endregion
    }
}