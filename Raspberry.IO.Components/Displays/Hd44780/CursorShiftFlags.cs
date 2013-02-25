using System;

namespace Raspberry.IO.Components.Displays.Hd44780
{
    [Flags]
    internal enum CursorShiftFlags
    {
        None = 0,

        CursorMove = 0,
        MoveLeft = 0,

        MoveRight = 0x04,
        DisplayMove = 0x08
    }
}