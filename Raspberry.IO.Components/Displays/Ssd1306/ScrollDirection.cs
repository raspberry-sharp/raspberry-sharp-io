using System;

namespace Raspberry.IO.Components.Displays.Ssd1306
{
    [Flags]
    public enum ScrollDirection : byte
    {
        HorizontalRight = 0x01,
        HorizontalLeft = 0x02,
        VerticalAndHorizontalRight = 0x04,
        VerticalAndHorizontalLeft = 0x05
    }
}