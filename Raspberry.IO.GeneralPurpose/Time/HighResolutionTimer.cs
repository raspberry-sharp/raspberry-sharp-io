using System;
using System.Threading;

namespace Raspberry.IO.GeneralPurpose.Time
{
    internal class HighResolutionTimer : ITimer
    {
        private decimal interval;
        private Thread thread;

        public HighResolutionTimer()
        {
            if (!Host.Current.IsRaspberryPi)
                throw new NotSupportedException("Cannot use HighResolutionTimer on a platform different than Raspberry Pi");
        }

        public decimal Interval
        {
            get { return interval; }
            set
            {
                if (value > uint.MaxValue / 1000)
                    throw new ArgumentOutOfRangeException("Interval", interval, "Interval must be lower than or equal to uint.MaxValue / 1000");

                interval = value;
            }
        }

        public void Start(decimal delay)
        {
            thread = new Thread(ThreadProcess);

            Sleep(delay);
            thread.Start();
        }
        
        public void Stop()
        {
            thread.Abort();
            thread = null;
        }

        public event EventHandler Elapsed;

        private void ThreadProcess()
        {
            var currentThread = thread;
            while (thread == currentThread)
            {
                Interop.bcm2835_delayMicroseconds((uint) interval*1000);

                var elapsed = Elapsed;
                elapsed(this, EventArgs.Empty);
            }
        }

        public static void Sleep(decimal delay)
        {
            if (delay > uint.MaxValue / 1000)
                throw new ArgumentOutOfRangeException("delay", delay, "Delay must be lower than or equal to uint.MaxValue / 1000");

            Interop.bcm2835_delayMicroseconds((uint)(delay * 1000));
        }
    }
}