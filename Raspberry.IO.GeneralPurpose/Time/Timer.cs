using System.Threading;

namespace Raspberry.IO.GeneralPurpose.Time
{
    internal static class Timer
    {
        public static ITimer Create()
        {
            return Host.Current.IsRaspberryPi
                       ? (ITimer)new HighResolutionTimer()
                       : new StandardTimer();
        }

        public static void Sleep(decimal delay)
        {
            if (Host.Current.IsRaspberryPi)
                HighResolutionTimer.Sleep(delay);
            else
                Thread.Sleep((int)delay);
        }
    }
}