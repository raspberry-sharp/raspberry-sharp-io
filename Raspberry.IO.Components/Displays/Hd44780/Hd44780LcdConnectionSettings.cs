using System.Text;

namespace Raspberry.IO.Components.Displays.Hd44780
{
    public class Hd44780LcdConnectionSettings
    {
        public Hd44780LcdConnectionSettings()
        {
            ScreenWidth = 20;
            ScreenHeight = 2;
            PatternWidth = 5;
            PatternHeight = 8;

            Encoding = new Hd44780A00Encoding();
            //RightToLeft = false;
        }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public int PatternWidth { get; set; }
        public int PatternHeight { get; set; }

        public Encoding Encoding { get; set; }
        //public bool RightToLeft { get; set; }
    }
}