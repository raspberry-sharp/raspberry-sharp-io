namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// Adafruit 12-Channel 16-bit PWM LED Driver TLC59711
    /// </summary>
    public interface ITlc59711Device : IPwmDevice, ITlc59711Settings
    {
        #region Methods

        /// <summary>
        /// Initializes the device with default values.
        /// </summary>
        void Reset();

        #endregion
    }
}