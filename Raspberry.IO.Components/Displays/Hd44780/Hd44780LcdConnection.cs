#region References

using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IOutputPin registerSelectPin;
        private readonly IOutputPin clockPin;
        private readonly IOutputPin[] dataPins;

        private readonly int width;
        private readonly int height;

        private readonly Functions functions;
        private readonly Encoding encoding;
        private readonly EntryModeFlags entryModeFlags;
        
        private DisplayFlags displayFlags = DisplayFlags.DisplayOn | DisplayFlags.BlinkOff | DisplayFlags.CursorOff;
        private int currentRow;
        private int currentColumn;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection"/> class.
        /// </summary>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="dataPins">The data pins.</param>
        public Hd44780LcdConnection(IOutputPin registerSelectPin, IOutputPin clockPin, params IOutputPin[] dataPins) : this(null, registerSelectPin, clockPin, (IEnumerable<IOutputPin>)dataPins) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection"/> class.
        /// </summary>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="dataPins">The data pins.</param>
        public Hd44780LcdConnection(IOutputPin registerSelectPin, IOutputPin clockPin, IEnumerable<IOutputPin> dataPins) : this(null, registerSelectPin, clockPin, dataPins) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="dataPins">The data pins.</param>
        public Hd44780LcdConnection(Hd44780LcdConnectionSettings settings, IOutputPin registerSelectPin, IOutputPin clockPin, params IOutputPin[] dataPins) : this(settings, registerSelectPin, clockPin, (IEnumerable<IOutputPin>)dataPins) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780LcdConnection"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="registerSelectPin">The register select pin.</param>
        /// <param name="clockPin">The clock pin.</param>
        /// <param name="dataPins">The data pins.</param>
        public Hd44780LcdConnection(Hd44780LcdConnectionSettings settings, IOutputPin registerSelectPin, IOutputPin clockPin, IEnumerable<IOutputPin> dataPins)
        {
            settings = settings ?? new Hd44780LcdConnectionSettings();

            this.registerSelectPin = registerSelectPin;
            this.clockPin = clockPin;
            this.dataPins = dataPins.ToArray();

            if (this.dataPins.Length != 4 && this.dataPins.Length != 8)
                throw new ArgumentOutOfRangeException("dataPins", this.dataPins.Length, "There must be either 4 or 8 data pins");
            
            width = settings.ScreenWidth;
            height = settings.ScreenHeight;
            if (height < 1 || height > 2)
                throw new ArgumentOutOfRangeException("ScreenHeight", height, "Screen must have either 1 or 2 rows");
            if (width * height > 80)
                throw new ArgumentException("At most 80 characters are allowed");

            if (settings.PatternWidth != 5)
                throw new ArgumentOutOfRangeException("PatternWidth", settings.PatternWidth, "Pattern must be 5 pixels width");
            if (settings.PatternHeight != 8 && settings.PatternHeight != 10)
                throw new ArgumentOutOfRangeException("PatternHeight", settings.PatternWidth, "Pattern must be either 7 or 10 pixels height");
            if (settings.PatternHeight == 10 && height == 2)
                throw new ArgumentException("10 pixels height pattern cannot be used with 2 rows");

            functions = (settings.PatternHeight == 8 ? Functions.Matrix5x8 : Functions.Matrix5x10) 
                | (height == 1 ? Functions.OneLine : Functions.TwoLines)
                | (this.dataPins.Length == 4 ? Functions.Data4bits : Functions.Data8bits);

            entryModeFlags = /*settings.RightToLeft 
                ? EntryModeFlags.EntryRight | EntryModeFlags.EntryShiftDecrement
                :*/ EntryModeFlags.EntryLeft | EntryModeFlags.EntryShiftDecrement;

            encoding = settings.Encoding;

            registerSelectPin.Write(false);
            clockPin.Write(false);
            foreach (var dataPin in this.dataPins)
                dataPin.Write(false);

            WriteByte(0x33, false); // Initialize
            WriteByte(0x32, false);

            WriteCommand(Command.SetFunctions, (int) functions);
            WriteCommand(Command.SetDisplayFlags, (int) displayFlags);
            WriteCommand(Command.SetEntryModeFlags, (int) entryModeFlags);

            Clear();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Properties

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

        public void Close()
        {
            Clear();

            registerSelectPin.Dispose();
            clockPin.Dispose();
            foreach (var dataPin in dataPins)
                dataPin.Dispose();
        }

        public void Home()
        {
            WriteCommand(Command.ReturnHome);
            currentRow = 0;
            currentColumn = 0;

            Sleep(3);
        }

        public void Clear()
        {
            WriteCommand(Command.ClearDisplay);
            currentRow = 0;
            currentColumn = 0;

            Sleep(3); // Clearing the display takes a long time
        }

        public void Move(int offset)
        {
            var count = offset > 0 ? offset : -offset;
            for (var i = 0; i < count; i++)
                WriteCommand(Command.MoveCursor, (int)(CursorShiftFlags.DisplayMove | (offset < 0 ? CursorShiftFlags.MoveLeft : CursorShiftFlags.MoveRight)));
        }

        public void SetCustomCharacter(byte character, byte[] pattern)
        {
            if ((functions & Functions.Matrix5x8) == Functions.Matrix5x8)
                Set5x8CustomCharacter(character, pattern);
            else
                Set5x10CustomCharacter(character, pattern);
        }

        public void WriteLine(object value, decimal animationDelay = 0m)
        {
            WriteLine("{0}", value, animationDelay);
        }

        public void WriteLine(string text, decimal animationDelay = 0m)
        {
            Write(text + Environment.NewLine, animationDelay);
        }

        public void Write(object value, decimal animationDelay = 0m)
        {
            Write("{0}", value, animationDelay);
        }

        public void WriteLine(string format, params object[] values)
        {
            WriteLine(string.Format(format, values));
        }

        public void Write(string format, params object[] values)
        {
            Write(string.Format(format, values));
        }

        public void WriteLine(string format, decimal animationDelay, params object[] values)
        {
            WriteLine(string.Format(format, values), animationDelay);
        }

        public void Write(string format, decimal animationDelay, params object[] values)
        {
            Write(string.Format(format, values), animationDelay);
        }

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
            if (dataPins.Length == 4)
                WriteByte4Pins(bits, charMode);
            else
                throw new NotImplementedException("8 bits mode is currently not implemented");
        }

        private void WriteByte4Pins(int bits, bool charMode)
        {
            registerSelectPin.Write(charMode);

            dataPins[0].Write((bits & 0x10) != 0);
            dataPins[1].Write((bits & 0x20) != 0);
            dataPins[2].Write((bits & 0x40) != 0);
            dataPins[3].Write((bits & 0x80) != 0);

            Synchronize();

            dataPins[0].Write((bits & 0x01) != 0);
            dataPins[1].Write((bits & 0x02) != 0);
            dataPins[2].Write((bits & 0x04) != 0);
            dataPins[3].Write((bits & 0x08) != 0);

            Synchronize();
        }

        private void Synchronize()
        {
            clockPin.Write(true);
            Sleep(0.001m); // 1 microsecond pause - enable pulse must be > 450ns 	

            clockPin.Write(false);
            Sleep(0.001m); // commands need > 37us to settle
        }

        #endregion
    }
}