using System;
using System.Threading;

namespace Raspberry.IO.GeneralPurpose.Time
{
    internal class HighResolutionTimer : ITimer
    {
        private decimal delay;
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

        private Action action;
        public Action Action
        {
            get { return action; }
            set
            {
                if (value == null)
                    Stop();
                
                action = value;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="delay">The delay before the first occurence, in milliseconds.</param>
        public void Start(decimal delay)
        {
            if (delay > uint.MaxValue / 1000)
                throw new ArgumentOutOfRangeException("delay", delay, "Delay must be lower than or equal to uint.MaxValue / 1000");

            lock (this)
            {
                if (thread == null)
                {
                    this.delay = delay;
                    thread = new Thread(ThreadProcess);
                    thread.Start();
                }
            }
        }
        
        public void Stop()
        {
            lock (this)
            {
                if (thread != null)
                {
                    thread.Abort();
                    thread = null;
                }
            }
        }

        private void ThreadProcess()
        {
            var thisThread = thread;

            Sleep(delay);
            while (thread == thisThread)
            {
                (Action ?? NoOp)();
                Sleep(interval);
            }
        }

        private void NoOp(){}

        public static void Sleep(decimal delay)
        {
            Interop.bcm2835_delayMicroseconds((uint)(delay * 1000));
        }
    }
}