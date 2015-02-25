namespace Raspberry.IO.Components.Displays.Ssd1306
{
    public class Command
    {
        public const byte SetContrast = 0x81;
        //
        public const byte DisplayAllOnResume = 0xA4;
        public const byte DisplayAllOn = 0xA5;
        public const byte DisplayNormal = 0xA6;
        public const byte DisplayInvert = 0xA7;
        public const byte DisplayOff = 0xAE;
        public const byte DisplayOn = 0xAF;
        //
        public const byte SetDisplayOffset = 0xD3;
        public const byte SetComPins = 0xDA;
        public const byte SetVComDetect = 0xDB;
        public const byte SetDisplayClockDivider = 0xD5;
        public const byte SetPreCharge = 0xD9;
        public const byte SetMultiplex = 0xA8;
        public const byte SetLowColumn = 0x00;
        public const byte SetHighColumn = 0x10;
        public const byte SetStartLine = 0x40;
        public const byte MemoryMode = 0x20;
        public const byte ColumnAddress = 0x21;
        public const byte PageAddress = 0x22;
        //
        public const byte ActivateScroll = 0x2F;
        public const byte DeactivateScroll = 0x2E;
        public const byte SetVerticalScrollArea = 0xA3;
        public const byte SetScrollDirection = 0x25;
        //
        public const byte ComScanIncrement = 0xC0;
        public const byte ComScanDecrement = 0xC8;
        public const byte SegRemap = 0xA0;
        public const byte ChargePump = 0x8D;
        public const byte ExternalVcc = 0x1;
        public const byte SwitchCapVcc = 0x2;
    }
}

