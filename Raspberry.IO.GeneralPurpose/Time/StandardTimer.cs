#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose.Time
{
    internal class StandardTimer : ITimer
    {
        #region Fields

        private decimal interval;
        private Action action;

        private bool isStarted;
        private System.Threading.Timer timer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interval, in milliseconds.
        /// </summary>
        /// <value>
        /// The interval, in milliseconds.
        /// </value>
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

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
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

        #endregion

        #region Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="delay">The delay before the first occurence, in milliseconds.</param>
        public void Start(decimal delay)
        {
            lock (this)
            {
                if (!isStarted && interval >= 1.0m)
                {
                    isStarted = true;
                    timer = new System.Threading.Timer(OnElapsed, null, (int) delay, (int) interval);
                }
                else
                    Stop();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
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

        #endregion

        #region Private Helpers

        private void NoOp(){}

        private void OnElapsed(object state)
        {
            (Action ?? NoOp)();
        }

        #endregion
    }
}