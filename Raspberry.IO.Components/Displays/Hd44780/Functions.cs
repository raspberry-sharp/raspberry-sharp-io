using System;

namespace Raspberry.IO.Components.Displays.Hd44780
{
    [Flags]
    internal enum Functions
    {
        None = 0,

        Matrix5x8 = 0,
        Matrix5x10 = 0x04,

        OneLine = 0,
        TwoLines = 0x08,

        Data4bits = 0x0,
        Data8bits = 0x10,
    }
}