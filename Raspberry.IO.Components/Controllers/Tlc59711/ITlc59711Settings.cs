namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// TLC59711 settings
    /// </summary>
    public interface ITlc59711Settings
    {

        #region Properties

        /// <summary>
        /// BLANK: If set to <c>true</c> all outputs are forced off. Default value is <c>true</c>.
        /// </summary>
        bool Blank { get; set; }

        /// <summary>
        /// DSPRPT: Auto display repeat mode. If <c>false</c> the auto repeat function is disabled. 
        /// </summary>
        /// <remarks>
        /// Each constant-current output is only turned on once, according the GS data after 
        /// <see cref="Blank"/> is set to <c>false</c> or after the internal latch pulse is 
        /// generated with <see cref="DisplayTimingResetMode"/> set to <c>true</c>. If <c>true</c> 
        /// each output turns on and off according to the GS data every 65536 GS reference clocks.
        /// </remarks>
        bool DisplayRepeatMode { get; set; }

        /// <summary>
        /// TMGRST: Display timing reset mode. Set to <c>false</c> to disable.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the GS counter is reset to '0' and all constant-current 
        /// outputs are forced off when the internal latch pulse is generated for data latching. 
        /// This function is the same when <see cref="Blank"/> is set to <c>false</c>. 
        /// Therefore, <see cref="Blank"/> does not need to be controlled by an external controller 
        /// when this mode is enabled. If <c>false</c>, the GS counter is not reset and no output 
        /// is forced off even if the internal latch pulse is generated.
        /// </remarks>
        bool DisplayTimingResetMode { get; set; }

        /// <summary>
        /// EXTGCK: GS reference clock selection. (<c>false</c> = internal oscillator clock, <c>true</c> = SCKI clock.)
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, PWM timing refers to the SCKI clock. If <c>false</c>, PWM timing 
        /// refers to the internal oscillator clock.
        /// </remarks>
        bool ReferenceClock { get; set; }

        /// <summary>
        /// OUTTMG: GS reference clock edge selection for OUTXn on-off timing control. (<c>false</c> = falling edge, <c>true</c> = rising edge).
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, OUTXn are turned on or off at the rising edge of the selected GS reference clock.
        /// If <c>false</c>, OUTXn are turned on or off at the falling edge of the selected clock.
        /// </remarks>
        bool ReferenceClockEdge { get; set; }

        /// <summary>
        /// BCR: Global brightness control for OUTR0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output 
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current. 
        /// </remarks>
        byte BrightnessControlR { get; set; }

        /// <summary>
        /// BCG: Global brightness control for OUTG0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output 
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current. 
        /// </remarks>
        byte BrightnessControlG { get; set; }

        /// <summary>
        /// BCB: Global brightness control for OUTB0-3. Default value is <c>127</c>.
        /// </summary>
        /// <remarks>
        /// The BC data are seven bits long, which allows each color group output 
        /// current to be adjusted in 128 steps (0-127) from 0% to 100% of the
        /// maximum output current. 
        /// </remarks>
        byte BrightnessControlB { get; set; }

        #endregion
    }
}