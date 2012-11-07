#region References

using System;
using System.Linq;
using System.Text;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Test.Gpio.HD44780
{
    /// <summary>
    /// Based on https://github.com/adafruit/Adafruit-Raspberry-Pi-Python-Code/blob/master/Adafruit_CharLCD/Adafruit_CharLCD.py
    /// and http://lcd-linux.sourceforge.net/pdfdocs/hd44780.pdf
    /// and http://www.quinapalus.com/hd44780udg.html
    /// and http://robo.fe.uni-lj.si/~kamnikr/sola/urac/vaja3_display/How%20to%20control%20HD44780%20display.pdf 
    /// </summary>
    public class HD44780LcdConnection : IDisposable
    {
        #region Fields

        private readonly IGpioConnectionDriver connectionDriver;

        private readonly ProcessorPin registerSelect;
        private readonly ProcessorPin clock;
        private readonly ProcessorPin data1;
        private readonly ProcessorPin data2;
        private readonly ProcessorPin data3;
        private readonly ProcessorPin data4;
        private readonly int width;
        private readonly int height;

        private readonly Functions functions;

        private readonly Encoding encoding;

        private DisplayFlags displayFlags = DisplayFlags.DisplayOn | DisplayFlags.BlinkOff | DisplayFlags.CursorOff;
        private EntryModeFlags entryModeFlags = EntryModeFlags.EntryLeft | EntryModeFlags.EntryShiftDecrement;

        private int currentRow;
        private int currentColumn;

        #endregion

        #region Instance Management
        
        public HD44780LcdConnection(
            ProcessorPin registerSelect, ProcessorPin clock,
            ProcessorPin data1, ProcessorPin data2, ProcessorPin data3, ProcessorPin data4,
            int width, int height)
        {
            this.registerSelect = registerSelect;
            this.clock = clock;
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
            this.data4 = data4;

            this.width = width;
            this.height = height;
            functions = Functions.Matrix5x7 | (height == 1 ? Functions.OneLine : Functions.TwoLines);

            encoding = new HD44780LcdJapaneseEncoding();

            connectionDriver = new MemoryGpioConnectionDriver();

            connectionDriver.Allocate(registerSelect, PinDirection.Output);
            connectionDriver.Allocate(clock, PinDirection.Output);
            connectionDriver.Allocate(data1, PinDirection.Output);
            connectionDriver.Allocate(data2, PinDirection.Output);
            connectionDriver.Allocate(data3, PinDirection.Output);
            connectionDriver.Allocate(data4, PinDirection.Output);

            connectionDriver.Write(registerSelect, false);
            connectionDriver.Write(clock, false);
            connectionDriver.Write(data1, false);
            connectionDriver.Write(data2, false);
            connectionDriver.Write(data3, false);
            connectionDriver.Write(data4, false);

            Write4Bits(0x33, false); // Initialize
            Write4Bits(0x32, false);

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

        public bool BlinkEnabled
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

            connectionDriver.Release(registerSelect);
            connectionDriver.Release(clock);
            connectionDriver.Release(data1);
            connectionDriver.Release(data2);
            connectionDriver.Release(data3);
            connectionDriver.Release(data4);
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

        public void WriteLine(object value)
        {
            WriteLine("{0}", value);
        }

        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        public void WriteLine(string format, params object[] values)
        {
            WriteLine(string.Format(format, values));
        }

        public void SetCustomCharacter(byte character, byte[] pattern)
        {
            if ((functions & Functions.Matrix5x7) == Functions.Matrix5x7)
                Set5x7CustomCharacter(character, pattern);
            else
                Set5x10CustomCharacter(character, pattern);
        }

        public void Write(string text)
        {
            var lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var bytes = encoding.GetBytes(line);
                foreach (var b in bytes)
                {
                    if (currentColumn < width)
                        Write4Bits(b, true);

                    currentColumn++;
                }

                if (currentRow == 0 && height > 1)
                {
                    Write4Bits(0xC0, false);
                    currentColumn = 0;
                    currentRow++;
                }
                else
                    break;
            }
        }

        public void Write(object value)
        {
            Write("{0}", value);
        }

        public void Write(string format, params object[] values)
        {
            Write(string.Format(format, values));
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
            Write4Bits(bits, false);
        }

        private void Set5x10CustomCharacter(byte character, byte[] pattern)
        {
            if (character > 7 || (character & 0x1) != 0x1)
                throw new ArgumentOutOfRangeException("character", character, "character must be lower or equal to 7, and not an odd number");
            if (pattern.Length != 10)
                throw new ArgumentOutOfRangeException("pattern", pattern, "pattern must be 10 rows long");

            WriteCommand(Command.SetCGRamAddr, character << 3);
            for (var i = 0; i < 7; i++)
                Write4Bits(pattern[i], true);
            Write4Bits(0, true);
        }

        private void Set5x7CustomCharacter(byte character, byte[] pattern)
        {
            if (character > 7)
                throw new ArgumentOutOfRangeException("character", character, "character must be lower or equal to 7");
            if (pattern.Length != 7)
                throw new ArgumentOutOfRangeException("pattern", pattern, "pattern must be 7 rows long");

            WriteCommand(Command.SetCGRamAddr, character << 3);
            for (var i = 0; i < 7; i++)
                Write4Bits(pattern[i], true);
            Write4Bits(0, true);
        }

        private void Write4Bits(int bits, bool charMode)
        {
            connectionDriver.Write(registerSelect, charMode);

            connectionDriver.Write(data1, (bits & 0x10) != 0);
            connectionDriver.Write(data2, (bits & 0x20) != 0);
            connectionDriver.Write(data3, (bits & 0x40) != 0);
            connectionDriver.Write(data4, (bits & 0x80) != 0);

            Synchronize();

            connectionDriver.Write(data1, (bits & 0x01) != 0);
            connectionDriver.Write(data2, (bits & 0x02) != 0);
            connectionDriver.Write(data3, (bits & 0x04) != 0);
            connectionDriver.Write(data4, (bits & 0x08) != 0);

            Synchronize();

        }

        private void Synchronize()
        {
            Sleep(0.001m); // 1 microsecond pause - enable pulse must be > 450ns 	

            connectionDriver.Write(clock, true);
            Sleep(0.001m); // 1 microsecond pause - enable pulse must be > 450ns 	

            connectionDriver.Write(clock, false);
            Sleep(0.001m); // commands need > 37us to settle
        }

        #endregion
    }
}