using System;

namespace Raspberry.IO.Components.Displays.Hd44780
{
    [Flags]
    internal enum EntryModeFlags
    {
        None = 0,

        EntryRight = 0,
        EntryShiftDecrement = 0,

        EntryShiftIncrement = 0x01,
        EntryLeft = 0x02
    }
}