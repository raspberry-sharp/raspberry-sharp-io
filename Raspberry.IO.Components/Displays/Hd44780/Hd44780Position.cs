namespace Raspberry.IO.Components.Displays.Hd44780
{
    /// <summary>
    /// Represents the position of the cursor on a Hd44780 display.
    /// </summary>
    public struct Hd44780Position
    {
        public int Row;
        public int Column;

        public static Hd44780Position Zero = new Hd44780Position {Row = 0, Column = 0};
    }
}