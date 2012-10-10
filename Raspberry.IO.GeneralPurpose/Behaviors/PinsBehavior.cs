using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public abstract class PinsBehavior
    {
        private readonly Timer timer;
        private int currentStep;

        protected PinsBehavior(IEnumerable<PinConfiguration> configurations)
        {
            Configurations = configurations.ToArray();
            Interval = 250;

            timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        public PinConfiguration[] Configurations { get; private set; }

        public int Interval { get; set; }

        private void OnTimer(object state)
        {
            Step(currentStep);
            if (!TryNextStep(ref currentStep))
            {
                Thread.Sleep(Interval);
                Stop();
            }
        }

        protected Connection Connection { get; private set; }

        protected abstract int FirstStep();

        protected abstract void Step(int step);

        protected abstract bool TryNextStep(ref int step);

        internal void Start(Connection connection)
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
    }
}
