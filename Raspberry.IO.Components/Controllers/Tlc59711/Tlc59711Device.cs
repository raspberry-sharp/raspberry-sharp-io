#region References

using System;
using Raspberry.IO.Interop;

#endregion

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// Adafruit 12-channel 16bit PWM/LED driver TLC59711
    /// </summary>
    public class Tlc59711Device : ITlc59711Device
    {
        #region Constants
        public const int COMMAND_SIZE = 28;
        private const byte MAGIC_WORD = 0x25 << 2;

        private const byte OUTTMG = 1 << 1;
        private const byte EXTGCK = 1;
        private const byte TMGRST = 1 << 7;
        private const byte DSPRPT = 1 << 6;
        private const byte BLANK = 1 << 5;
        private const byte OFF = 0;
        private const int DATA_OFFSET = 4;
        private const int DATA_LENGTH = COMMAND_SIZE - DATA_OFFSET;
        #endregion
        
        #region Fields

        private readonly IMemory memory;
        private readonly ITlc59711Settings initSettings;
        private readonly IPwmChannels channels;

        private byte referenceClockEdge = OUTTMG;
        private byte referenceClock = OFF;
        private byte displayTimingResetMode = TMGRST;
        private byte displayRepeatMode = DSPRPT;
        private byte blank = BLANK;
        private byte bcb = 127;
        private byte bcg = 127;
        private byte bcr = 127;

        #endregion

        #region Instance Management

        /// <summary>
        /// Creates a new instance of the <see cref="Tlc59711Device"/> class.
        /// </summary>
        /// <param name="memory">Memory to work with.</param>
        /// <param name="settings">Initial settings</param>
        public Tlc59711Device(IMemory memory, ITlc59711Settings settings = null) {
            if (ReferenceEquals(memory, null))
                throw new ArgumentNullException("memory");

            if (memory.Length < COMMAND_SIZE) {
                throw new ArgumentException(
                    string.Format("Need at least {0} bytes, got {1} bytes.", COMMAND_SIZE, memory.Length), "memory");
            }

            this.memory = memory;
            initSettings = settings;
            channels = new Tlc59711Channels(memory, DATA_OFFSET);

            Reset();
        }

        #endregion

        #region Properties

        /// <summary>
        /// BLANK: If set to <c>true</c> all outputs are forced off. Default value is <c>true</c>.
        /// </summary>
        public bool Blank {
            get { return blank == BLANK; }
            set {
                blank = (value) ? BLANK : OFF;
                WriteSecondByte();
            }
        }

        /// <summary>
        /// DSPRPT: Auto display repeat mode. If <c>false</c> the auto repeat function is disabled. 
        /// </summary>
        /// <remarks>
        /// Each constant-current output is only turned on once, according the GS data after 
        /// <see cref="ITlc59711Settings.Blank"/> is set to <c>false</c> or after the internal latch pulse is 
        /// generated with <see cref="ITlc59711Settings.DisplayTimingResetMode"/> set to <c>true</c>. If <c>true</c> 
        /// each output turns on and off according to the GS data every 65536 GS reference clocks.
        /// </remarks>
        public bool DisplayRepeatMode {
            get { return displayRepeatMode == DSPRPT; }
            set {
                displayRepeatMode = (value) ? DSPRPT : OFF;
                WriteSecondByte();
            }
        }

        /// <summary>
        /// TMGRST: Display timing reset mode. Set to <c>false</c> to disable.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the GS counter is reset to '0' and all constant-current 
        /// outputs are forced off when the internal latch pulse is generated for data latching. 
        /// This function is the same when <see cref="ITlc59711Settings.Blank"/> is set to <c>false</c>. 
        /// Therefore, <see cref="ITlc59711Settings.Blank"/> does not need to be controlled by an external controller 
        /// when this mode is enabled. If <c>false</c>, the GS counter is not reset and no output 
        /// is forced off even if the internal latch pulse is generated.
        /// </remarks>
        public bool DisplayTimingResetMode {
            get { return displayTimingResetMode == TMGRST; }
            set {
                displayTimingResetMode = (value) ? TMGRST : OFF;
                WriteSecondByte();
            }
        }

        /// <summary>
        /// EXTGCK: GS reference clock selection. (<c>false</c> = internal oscillator clock, <c>true</c> = SCKI clock.)
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, PWM timing refers to the SCKI clock. If <c>false</c>, PWM timing 
        /// refers to the internal oscillator clock.
        /// </remarks>
        public bool ReferenceClock {
            get { return referenceClock == EXTGCK; }
            set {
                referenceClock = (value) ? EXTGCK : OFF;
                WriteFirstByte();
            }
        }

        /// <summary>
        /// OUTTMG: GS reference clock edge selection for OUTXn on-off timing control. (<c>false</c> = falling edge, <c>true</c> = rising edge).
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, OUTXn are turned on or off at the rising edge of the selected GS reference clock.
        /// If <c>false</c>, OUTXn are turned on or off at the falling edge of the selected clock.
        /// </remarks>
        public bool ReferenceClockEdge {
            get { return referenceClockEdge == OUTTMG; }
            set {
                referenceClockEdge = (value) ? OUTTMG : OFF;
                WriteFirstByte();
            }
        }

        /// <summary>
        /// BCR: Global brightness control for OUTR0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output 
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current. 
        /// </remarks>
        public byte BrightnessControlR {
            get { return bcr; }
            set {
                value.ThrowOnInvalidBrightnessControl();
                
                bcr = value;
                WriteFourthByte();
            }
        }

        /// <summary>
        /// BCG: Global brightness control for OUTG0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output 
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current. 
        /// </remarks>
        public byte BrightnessControlG {
            get { return bcg; }
            set { 
                value.ThrowOnInvalidBrightnessControl();
                
                bcg = value;
                WriteThirdByte();
                WriteFourthByte();
            }
        }

        /// <summary>
        /// BCB: Global brightness control for OUTB0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output 
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current. 
        /// </remarks>
        public byte BrightnessControlB {
            get { return bcb; }
            set {
                value.ThrowOnInvalidBrightnessControl();

                bcb = value;
                WriteSecondByte();
                WriteThirdByte();
            }
        }

        /// <summary>
        /// The PWM channels
        /// </summary>
        public IPwmChannels Channels {
            get { return channels; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the device with default values.
        /// </summary>
        public void Reset() {
            Initialize(initSettings ?? new Tlc59711Settings());
        }

        #endregion

        #region Private Helpers

        private void Initialize(ITlc59711Settings settings) {
            referenceClockEdge = settings.ReferenceClockEdge ? OUTTMG : OFF;
            referenceClock = settings.ReferenceClock ? EXTGCK : OFF;
            displayTimingResetMode = settings.DisplayTimingResetMode ? TMGRST : OFF;
            displayRepeatMode = settings.DisplayRepeatMode ? DSPRPT : OFF;
            blank = settings.Blank ? BLANK : OFF;
            bcb = settings.BrightnessControlB;
            bcg = settings.BrightnessControlG;
            bcr = settings.BrightnessControlR;

            WriteFirstByte();
            WriteSecondByte();
            WriteThirdByte();
            WriteFourthByte();

            var zero = new byte[DATA_LENGTH];
            memory.Copy(zero, 0, DATA_OFFSET, DATA_LENGTH);
        }

        private void WriteFirstByte() {
            memory.Write(0, (byte) (MAGIC_WORD | referenceClockEdge | referenceClock));
        }

        private void WriteSecondByte() {
            memory.Write(1, (byte) (displayTimingResetMode | displayRepeatMode | blank | (bcb >> 2)));
        }

        private void WriteThirdByte() {
            memory.Write(2, (byte) ((bcb << 6) | (bcg >> 1)));
        }

        private void WriteFourthByte() {
            memory.Write(3, (byte) ((bcg << 7) | bcr));
        }

        #endregion
    }
}