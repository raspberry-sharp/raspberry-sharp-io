using System.Text;

namespace Raspberry.IO.Components.Displays.Hd44780
{
    public class Hd44780LcdConnectionSettings
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnectionSettings"/> class.
        /// </summary>
        public Hd44780LcdConnectionSettings()
        {
            ScreenWidth = 20;
            ScreenHeight = 2;
            PatternWidth = 5;
            PatternHeight = 8;

            Encoding = new Hd44780A00Encoding();
            //RightToLeft = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        /// <value>
        /// The width of the screen.
        /// </value>
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// </summary>
        /// <value>
        /// The height of the screen.
        /// </value>
        public int ScreenHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the pattern.
        /// </summary>
        /// <value>
        /// The width of the pattern.
        /// </value>
        public int PatternWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the pattern.
        /// </summary>
        /// <value>
        /// The height of the pattern.
        /// </value>
        public int PatternHeight { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding { get; set; }

        //public bool RightToLeft { get; set; }

        #endregion
    }
}