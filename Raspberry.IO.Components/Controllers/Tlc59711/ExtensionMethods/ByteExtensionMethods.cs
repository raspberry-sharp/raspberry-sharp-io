#region References

using System;

#endregion

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    internal static class ByteExtensionMethods
    {
        #region Constants
        public const byte BRIGHTNESS_CONTROL_MAX = 127;
        #endregion

        #region Methods

        public static void ThrowOnInvalidBrightnessControl(this byte value) {
            if (value <= BRIGHTNESS_CONTROL_MAX)
                return;

            var message = String.Format("The maximum value for brightness control is {0}. You set a value of {1}.", BRIGHTNESS_CONTROL_MAX, value);

            throw new ArgumentException(message, "value");
        }

        #endregion
    }
}