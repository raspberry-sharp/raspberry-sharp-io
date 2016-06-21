#region References

using System;
using System.Collections.Generic;
using System.Text;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.Components.Displays.Hd44780
{
    /// <summary>
    /// Based on https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_CharLCD/Adafruit_CharLCD.py
    ///      and http://lcd-linux.sourceforge.net/pdfdocs/hd44780.pdf
    ///      and http://www.quinapalus.com/hd44780udg.html
    ///      and http://robo.fe.uni-lj.si/~kamnikr/sola/urac/vaja3_display/How%20to%20control%20HD44780%20display.pdf 
    ///      and http://web.stanford.edu/class/ee281/handouts/lcd_tutorial.pdf
    ///      and http://www.systronix.com/access/Systronix_20x4_lcd_brief_data.pdf
    /// </summary>
    public class Hd44780LcdConnection : IDisposable
    {
        #region Fields

        private const int MAX_HEIGHT = 4;   // Allow for larger displays
        private const int MAX_CHAR = 80;    // This allows for setups such as 40x2 or a 20x4
        
        private readonly Hd44780Pins pins;

        private readonly int width;
        private readonly int height;

        private readonly Functions functions;
        private readonly Encoding encoding;
        private readonly EntryModeFlags entryModeFlags;
        
        private DisplayFlags displayFlags = DisplayFlags.DisplayOn | DisplayFlags.BlinkOff | DisplayFlags.CursorOff;
        private Hd44780Position currentPosition;
        
        private bool backlightEnabled;

        private static readonly TimeSpan syncDelay = TimeSpanUtility.FromMicroseconds(1);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection" /> class.
        /// </summary>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="dataPins">The data pins.</param>
        public Hd44780LcdConnection(IOutputBinaryPin registerSelectPin, IOutputBinaryPin clockPin, params IOutputBinaryPin[] dataPins) : this(null, new Hd44780Pins(registerSelectPin, clockPin, dataPins)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection" /> class.
        /// </summary>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="dataPins">The data pins.</param>
        public Hd44780LcdConnection(IOutputBinaryPin registerSelectPin, IOutputBinaryPin clockPin, IEnumerable<IOutputBinaryPin> dataPins) : this(null, new Hd44780Pins(registerSelectPin, clockPin, dataPins)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pins">The pins.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// dataPins;There must be either 4 or 8 data pins
        /// or
        /// settings;ScreenHeight must be between 1 and 4 rows
        /// or
        /// settings;PatternWidth must be 5 pixels
        /// or
        /// settings;PatternWidth must be either 7 or 10 pixels height
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// At most 80 characters are allowed
        /// or
        /// 10 pixels height pattern cannot be used with 2 rows
        /// </exception>
        public Hd44780LcdConnection(Hd44780LcdConnectionSettings settings, Hd44780Pins pins)
        {
            settings = settings ?? new Hd44780LcdConnectionSettings();
            this.pins = pins;

            if (pins.Data.Length != 4 && pins.Data.Length != 8)
                throw new ArgumentOutOfRangeException("pins", pins.Data.Length, "There must be either 4 or 8 data pins");
            
            width = settings.ScreenWidth;
            height = settings.ScreenHeight;
            if (height < 1 || height > MAX_HEIGHT)
                throw new ArgumentOutOfRangeException("settings", height, "ScreenHeight must be between 1 and 4 rows");
            if (width * height > MAX_CHAR)
                throw new ArgumentException("At most 80 characters are allowed");

            if (settings.PatternWidth != 5)
                throw new ArgumentOutOfRangeException("settings", settings.PatternWidth, "PatternWidth must be 5 pixels");
            if (settings.PatternHeight != 8 && settings.PatternHeight != 10)
                throw new ArgumentOutOfRangeException("settings", settings.PatternWidth, "PatternWidth must be either 7 or 10 pixels height");
            if (settings.PatternHeight == 10 && (height % 2) == 0)
                throw new ArgumentException("10 pixels height pattern cannot be used with an even number of rows");

            functions = (settings.PatternHeight == 8 ? Functions.Matrix5x8 : Functions.Matrix5x10) 
                | (height == 1 ? Functions.OneLine : Functions.TwoLines)
                | (pins.Data.Length == 4 ? Functions.Data4bits : Functions.Data8bits);

            entryModeFlags = /*settings.RightToLeft 
                ? EntryModeFlags.EntryRight | EntryModeFlags.EntryShiftDecrement
                :*/ EntryModeFlags.EntryLeft | EntryModeFlags.EntryShiftDecrement;

            encoding = settings.Encoding;

            BacklightEnabled = false;

            if (pins.ReadWrite != null)
                pins.ReadWrite.Write(false);

            pins.RegisterSelect.Write(false);
            pins.Clock.Write(false);
            foreach (var dataPin in pins.Data)
                dataPin.Write(false);

            WriteByte(0x33, false); // Initialize
            WriteByte(0x32, false);

            WriteCommand(Command.SetFunctions, (int) functions);
            WriteCommand(Command.SetDisplayFlags, (int) displayFlags);
            WriteCommand(Command.SetEntryModeFlags, (int) entryModeFlags);

            Clear();
            BacklightEnabled = true;
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether display is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if display is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayEnabled
        {
            get { return (displayFlags & DisplayFlags.DisplayOn) == DisplayFlags.DisplayOn; }
            set
            {
                if (value)
                    displayFlags |= DisplayFlags.DisplayOn;
                else
                    displayFlags &= ~DisplayFlags.DisplayOn;

                WriteCommand(Command.SetDisplayFlags, (int) displayFlags);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether backlight is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if backlight is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool BacklightEnabled
        {
            get { return backlightEnabled; }
            set
            {
                if (pins.Backlight == null) 
                    return;

                pins.Backlight.Write(value);
                backlightEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cursor is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cursor is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CursorEnabled
        {
            get { return (displayFlags & DisplayFlags.CursorOn) == DisplayFlags.CursorOn; }
            set
            {
                if (value)
                    displayFlags |= DisplayFlags.CursorOn;
                else
                    displayFlags &= ~DisplayFlags.CursorOn;

                WriteCommand(Command.SetDisplayFlags, (int) displayFlags);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cursor is blinking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cursor is blinking; otherwise, <c>false</c>.
        /// </value>
        public bool CursorBlinking
        {
            get { return (displayFlags & DisplayFlags.BlinkOn) == DisplayFlags.BlinkOn; }
            set
            {
                if (value)
                    displayFlags |= DisplayFlags.BlinkOn;
                else
                    displayFlags &= ~DisplayFlags.BlinkOn;

                WriteCommand(Command.SetDisplayFlags, (int) displayFlags);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            Clear();
            pins.Close();
        }

        /// <summary>
        /// Set cursor to top left corner.
        /// </summary>
        public void Home()
        {
            WriteCommand(Command.ReturnHome);
            currentPosition = Hd44780Position.Zero;
            
            Timer.Sleep(TimeSpan.FromMilliseconds(3));
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void Clear()
        {
            WriteCommand(Command.ClearDisplay);
            currentPosition = Hd44780Position.Zero;
            
            Timer.Sleep(TimeSpan.FromMilliseconds(3)); // Clearing the display takes a long time
        }

        /// <summary>
        /// Moves the cursor of the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Move(int offset)
        {
            var column = currentPosition.Column += offset;
            var row = currentPosition.Row;

            if (column >= width)
            {
                column = 0;
                row++;
            }
            else if (column < 0)
            {
                column = width - 1;
                row--;
            }

            if (row >= height)
                row = 0;
            if (row < 0)
                row = height - 1;

            SetCursorPosition(new Hd44780Position{Row = row, Column = column});
        }

        /// <summary>
        /// Moves the cursor to the specified row and column
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetCursorPosition(Hd44780Position position)
        {
            var row = position.Row;
            if (row < 0 || height <= row)
                row = height - 1;

            var column = position.Column;
            if (column < 0 || width <= column)
                column = width - 1;

            var address = column + GetLcdAddressLocation(row);
            
            WriteByte(address, false);

            currentPosition = new Hd44780Position { Row = row, Column = column };
        }

        /// <summary>
        /// Sets the custom character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="pattern">The pattern.</param>
        public void SetCustomCharacter(byte character, byte[] pattern)
        {
            if ((functions & Functions.Matrix5x8) == Functions.Matrix5x8)
                Set5x8CustomCharacter(character, pattern);
            else
                Set5x10CustomCharacter(character, pattern);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void WriteLine(object value, TimeSpan animationDelay = new TimeSpan())
        {
            WriteLine("{0}", value, animationDelay);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void WriteLine(string text, TimeSpan animationDelay = new TimeSpan())
        {
            Write(text + Environment.NewLine, animationDelay);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void Write(object value, TimeSpan animationDelay = new TimeSpan())
        {
            Write("{0}", value, animationDelay);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        public void WriteLine(string format, params object[] values)
        {
            WriteLine(string.Format(format, values));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        public void Write(string format, params object[] values)
        {
            Write(string.Format(format, values));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="animationDelay">The animation delay.</param>
        /// <param name="values">The values.</param>
        public void WriteLine(string format, TimeSpan animationDelay, params object[] values)
        {
            WriteLine(string.Format(format, values), animationDelay);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="animationDelay">The animation delay.</param>
        /// <param name="values">The values.</param>
        public void Write(string format, TimeSpan animationDelay, params object[] values)
        {
            Write(string.Format(format, values), animationDelay);
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void Write(string text, TimeSpan animationDelay = new TimeSpan())
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                //Console.WriteLine(line);
                var bytes = encoding.GetBytes(line);
                foreach (var b in bytes)
                {
                    if (currentPosition.Column < width)
                        WriteByte(b, true);

                    if (animationDelay > TimeSpan.Zero)
                        Timer.Sleep(animationDelay);

                    currentPosition.Column++;
                }

                if ((currentPosition.Row == 0 || (currentPosition.Row + 1) % height != 0) && height > 1)
                {
                    var addressLocation = GetLcdAddressLocation(currentPosition.Row + 1);
                    
                    WriteByte(addressLocation, false);
                    currentPosition.Column = 0;
                    currentPosition.Row++;
                }
                else
                {
                    Home(); // This was added to return home when the maximum number of row's has been achieved.
                    break;
                }
            }
        }

        #endregion

        #region Private Helpers

        private void WriteCommand(Command command, int parameter = 0)
        {
            var bits = (int) command | parameter;
            WriteByte(bits, false);
        }

        private void Set5x10CustomCharacter(byte character, byte[] pattern)
        {
            if (character > 7 || (character & 0x1) != 0x1)
                throw new ArgumentOutOfRangeException("character", character, "character must be lower or equal to 7, and not an odd number");
            if (pattern.Length != 10)
                throw new ArgumentOutOfRangeException("pattern", pattern, "pattern must be 10 rows long");

            WriteCommand(Command.SetCGRamAddr, character << 3);
            for (var i = 0; i < 10; i++)
                WriteByte(pattern[i], true);
            WriteByte(0, true);
        }

        private void Set5x8CustomCharacter(byte character, byte[] pattern)
        {
            if (character > 7)
                throw new ArgumentOutOfRangeException("character", character, "character must be lower or equal to 7");
            if (pattern.Length != 7)
                throw new ArgumentOutOfRangeException("pattern", pattern, "pattern must be 7 rows long");

            WriteCommand(Command.SetCGRamAddr, character << 3);
            for (var i = 0; i < 7; i++)
                WriteByte(pattern[i], true);
            WriteByte(0, true);
        }

        private void WriteByte(int bits, bool charMode)
        {
            if (pins.Data.Length == 4)
                WriteByte4Pins(bits, charMode);
            else
                throw new NotImplementedException("8 bits mode is currently not implemented");
        }

        private void WriteByte4Pins(int bits, bool charMode)
        {
            pins.RegisterSelect.Write(charMode);

            pins.Data[0].Write((bits & 0x10) != 0);
            pins.Data[1].Write((bits & 0x20) != 0);
            pins.Data[2].Write((bits & 0x40) != 0);
            pins.Data[3].Write((bits & 0x80) != 0);

            Synchronize();

            pins.Data[0].Write((bits & 0x01) != 0);
            pins.Data[1].Write((bits & 0x02) != 0);
            pins.Data[2].Write((bits & 0x04) != 0);
            pins.Data[3].Write((bits & 0x08) != 0);

            Synchronize();
        }

        /// <summary>
        /// Returns the Lcd Address for the given row
        /// </summary>
        /// <param name="row">A zero based row position</param>
        /// <returns>The Lcd Address as an int</returns>
        /// <remarks>http://www.mikroe.com/forum/viewtopic.php?t=5149</remarks>
        private int GetLcdAddressLocation(int row)
        {
            const int baseAddress = 128;

            switch (row)
            {
                case 0: return baseAddress;
                case 1: return (baseAddress + 64);
                case 2: return (baseAddress + width);
                case 3: return (baseAddress + 64 + width);
                default: return baseAddress;
            }
        }

        private void Synchronize()
        {
            pins.Clock.Write(true);
            Timer.Sleep(syncDelay); // 1 microsecond pause - enable pulse must be > 450ns 	

            pins.Clock.Write(false);
            Timer.Sleep(syncDelay); // commands need > 37us to settle
        }

        #endregion
    }
}