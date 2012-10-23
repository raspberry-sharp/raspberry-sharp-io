using System;

namespace Raspberry.IO.GeneralPurpose.Time
{
    internal class StandardTimer : ITimer
    {
        private decimal interval;

        private bool isStarted;
        private System.Threading.Timer timer;

        public decimal Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                if (isStarted)
                    Start(0);
            }
        }

        public void Start(decimal delay)
        {
            lock (this)
            {
                if (!isStarted && interval >= 1.0m)
                {
                    isStarted = true;
                    timer = new System.Threading.Timer(OnElapsed, null, (int)delay, (int)interval);
                }
                else
                    Stop();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (isStarted)
                {
                    isStarted = false;
                    timer.Dispose();
                    timer = null;
                }
            }
        }

        public event EventHandler Elapsed;

        private void OnElapsed(object state)
        {
            var elapsed = Elapsed;
            elapsed(this, EventArgs.Empty);
        }
    }
}