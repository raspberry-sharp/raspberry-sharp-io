namespace Raspberry.IO.Components.Controllers.Pca9685
{
    /// <summary>
    /// Make it easier to IoC 
    /// </summary>
    public interface IPwmDevice
    {
        void SetPwmUpdateRate(int frequencyHz);

        /// <summary>
        /// Sets a single PWM channel with on / off values to control the duty cycle
        /// </summary>
        void SetPwm(PwmChannel channel, int on, int off);

        /// <summary>
        /// Set a channel to fully on or off
        /// </summary>
        void SetFull(PwmChannel channel, bool fullOn);
    }
}