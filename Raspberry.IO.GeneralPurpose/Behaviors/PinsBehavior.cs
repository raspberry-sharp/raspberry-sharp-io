#region References

using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public abstract class PinsBehavior
    {
        #region Fields

        private readonly Timer timer;
        private int currentStep;

        #endregion

        #region Instance Management

        protected PinsBehavior(IEnumerable<PinConfiguration> configurations)
        {
            Configurations = configurations.ToArray();
            Interval = 250;

            timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region Properties

        public PinConfiguration[] Configurations { get; private set; }

        public int Interval { get; set; }

        #endregion

        #region Protected Methods

        protected GpioConnection Connection { get; private set; }

        protected abstract int FirstStep();

        protected abstract void Step(int step);

        protected abstract bool TryNextStep(ref int step);

        #endregion

        #region Internal Methods

        internal void Start(GpioConnection connection)
        {
            Connection = connection;
            foreach (var pinConfiguration in Configurations)
                connection[pinConfiguration] = false;

            currentStep = FirstStep();
            timer.Change(0, Interval);
        }

        internal void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);

            foreach (var pinConfiguration in Configurations)
                Connection[pinConfiguration] = false;
        }

        #endregion

        #region Private Helpers

        private void OnTimer(object state)
        {
            Step(currentStep);
            if (!TryNextStep(ref currentStep))
            {
                Thread.Sleep(Interval);
                Stop();
            }
        }

        #endregion
    }
}
