namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// A pulse-width modulation (PWM) device
    /// </summary>
    public interface IPwmDevice 
    {
        #region Properties

        /// <summary>
        /// The PWM channels
        /// </summary>
        IPwmChannels Channels { get; }

        #endregion
    }
}