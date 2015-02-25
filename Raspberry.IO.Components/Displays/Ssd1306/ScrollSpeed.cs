using System;

namespace Raspberry.IO.Components.Displays.Ssd1306
{
    [Flags]
    public enum ScrollSpeed : byte
    {
        F2 = 0x7,
        F3 = 0x4,
        F4 = 0x5,
        F5 = 0x0,
        F25 = 0x6,
        F64 = 0x1,
        F128 = 0x2,
        F256 = 0x3
    }
}