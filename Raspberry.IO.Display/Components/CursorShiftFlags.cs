using System;

namespace Raspberry.IO.Display.Components
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