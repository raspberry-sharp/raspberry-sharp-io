#region References

using System.Threading;

#endregion

namespace Raspberry.IO.GeneralPurpose.Time
{
    internal static class Timer
    {
        #region Methods

        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <returns>The timer.</returns>
        /// <remarks>
        /// The created timer is the most suitable for the current platform.
        /// </remarks>
        public static ITimer Create()
        {
            return Host.Current.IsRaspberryPi
                       ? (ITimer) new HighResolutionTimer()
                       : new StandardTimer();
        }

        /// <summary>
        /// Sleeps during the specified time.
        /// </summary>
        /// <param name="time">The time, in milliseconds.</param>
        public static void Sleep(decimal time)
        {
            if (Host.Current.IsRaspberryPi)
                HighResolutionTimer.Sleep(time);
            else
                Thread.Sleep((int) time);
        }

        #endregion
    }
}