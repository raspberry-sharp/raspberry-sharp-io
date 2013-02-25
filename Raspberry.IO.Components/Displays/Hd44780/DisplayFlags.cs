using System;

namespace Raspberry.IO.Components.Displays.Hd44780
{
    [Flags]
    internal enum DisplayFlags
    {
        None = 0,

        BlinkOff = 0,
        CursorOff = 0,
        DisplayOff = 0,

        BlinkOn = 0x01,
        CursorOn = 0x02,
        DisplayOn = 0x04
    }
}