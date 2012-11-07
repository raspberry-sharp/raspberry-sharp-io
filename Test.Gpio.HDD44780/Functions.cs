using System;

namespace Test.Gpio.HD44780
{
    [Flags]
    internal enum Functions
    {
        None = 0,

        Matrix5x7 = 0,
        OneLine = 0,

        Matrix5x10 = 0x04,
        TwoLines = 0x08,

        //LCD_8BITMODE = 0x10,
    }
}