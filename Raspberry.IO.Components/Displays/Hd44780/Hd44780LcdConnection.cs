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
    /// </summary>
    public class Hd44780LcdConnection : IDisposable
    {
        #region Fields

        private readonly Hd44780Pins pins;

        private readonly int width;
        private readonly int height;

        private readonly Functions functions;
        private readonly Encoding encoding;
        private readonly EntryModeFlags entryModeFlags;
        
        private DisplayFlags displayFlags = DisplayFlags.DisplayOn | DisplayFlags.BlinkOff | DisplayFlags.CursorOff;
        private int currentRow;
        private int currentColumn;

        private bool backlightEnabled;

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
        /// settings;ScreenHeight must be either 1 or 2 rows
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
            if (height < 1 || height > 2)
                throw new ArgumentOutOfRangeException("settings", height, "ScreenHeight must be either 1 or 2 rows");
            if (width * height > 80)
                throw new ArgumentException("At most 80 characters are allowed");

            if (settings.PatternWidth != 5)
                throw new ArgumentOutOfRangeException("settings", settings.PatternWidth, "PatternWidth must be 5 pixels");
            if (settings.PatternHeight != 8 && settings.PatternHeight != 10)
                throw new ArgumentOutOfRangeException("settings", settings.PatternWidth, "PatternWidth must be either 7 or 10 pixels height");
            if (settings.PatternHeight == 10 && height == 2)
                throw new ArgumentException("10 pixels height pattern cannot be used with 2 rows");

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
            currentRow = 0;
            currentColumn = 0;

            Sleep(3);
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void Clear()
        {
            WriteCommand(Command.ClearDisplay);
            currentRow = 0;
            currentColumn = 0;

            Sleep(3); // Clearing the display takes a long time
        }

        /// <summary>
        /// Moves the cursor of the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Move(int offset)
        {
            var count = offset > 0 ? offset : -offset;
            for (var i = 0; i < count; i++)
                WriteCommand(Command.MoveCursor, (int)(CursorShiftFlags.DisplayMove | (offset < 0 ? CursorShiftFlags.MoveLeft : CursorShiftFlags.MoveRight)));
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
        public void WriteLine(object value, decimal animationDelay = 0m)
        {
            WriteLine("{0}", value, animationDelay);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void WriteLine(string text, decimal animationDelay = 0m)
        {
            Write(text + Environment.NewLine, animationDelay);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void Write(object value, decimal animationDelay = 0m)
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
        public void WriteLine(string format, decimal animationDelay, params object[] values)
        {
            WriteLine(string.Format(format, values), animationDelay);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="animationDelay">The animation delay.</param>
        /// <param name="values">The values.</param>
        public void Write(string format, decimal animationDelay, params object[] values)
        {
            Write(string.Format(format, values), animationDelay);
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="animationDelay">The animation delay.</param>
        public void Write(string text, decimal animationDelay = 0m)
        {
            var lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                Console.WriteLine(line);
                var bytes = encoding.GetBytes(line);
                foreach (var b in bytes)
                {
                    if (currentColumn < width)
                        WriteByte(b, true);

                    if (animationDelay > 0m)
                        Timer.Sleep(animationDelay);

                    currentColumn++;
                }

                if (currentRow == 0 && height > 1)
                {
                    WriteByte(0xC0, false);
                    currentColumn = 0;
                    currentRow++;
                }
                else
                    break;
            }
        }

        #endregion

        #region Private Helpers

        private void Sleep(decimal delay)
        {
            Timer.Sleep(delay);
        }

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

        private void Synchronize()
        {
            pins.Clock.Write(true);
            Sleep(0.001m); // 1 microsecond pause - enable pulse must be > 450ns 	

            pins.Clock.Write(false);
            Sleep(0.001m); // commands need > 37us to settle
        }

        #endregion
    }
}