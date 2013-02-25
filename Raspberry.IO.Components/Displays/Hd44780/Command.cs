namespace Raspberry.IO.Components.Displays.Hd44780
{
    internal enum Command
    {
        ClearDisplay = 0x01,
        ReturnHome = 0x02,
        SetEntryModeFlags = 0x04,
        SetDisplayFlags = 0x08,
        MoveCursor = 0x10,
        SetFunctions = 0x20,
        SetCGRamAddr = 0x40,
        //SetDDRamAddr = 0x80
    }
}