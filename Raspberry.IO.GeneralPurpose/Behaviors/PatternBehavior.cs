#region References

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public class PatternBehavior : PinsBehavior
    {
        #region Fields

        private bool wayOut;

        #endregion

        #region Instance Management

        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<int> patterns) : this(configurations, patterns.Select(i => (long) i)){}

        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<long> patterns) : base(configurations)
        {
            Patterns = patterns.ToArray();
        }

        #endregion

        #region Properties

        public bool Loop { get; set; }

        public bool RoundTrip { get; set; }

        #endregion

        #region Protected Methods

        protected override int FirstStep()
        {
            wayOut = true;
            return 0;
        }

        protected override void Step(int step)
        {
            var pattern = Patterns[step];

            for (var i = 0; i < Configurations.Length; i++)
                Connection[Configurations[i]] = ((pattern >> i) & 0x1) == 0x1;
        }

        protected override bool TryNextStep(ref int step)
        {
            if (wayOut)
            {
                if (step == Patterns.Length - 1)
                {
                    if (RoundTrip)
                    {
                        wayOut = false;
                        step--;
                    }
                    else if (Loop)
                        step = 0;
                    else
                        return false;
                }
                else
                    step++;
            }
            else
            {
                if (step == 0)
                {
                    if (Loop && RoundTrip)
                    {
                        wayOut = true;
                        step++;
                    }
                    else if (Loop)
                        step = Patterns.Length - 1;
                    else
                        return false;
                }
                else step--;
            }

            return true;
        }

        #endregion

        #region Private Helpers

        private long[] Patterns { get; set; }

        #endregion
    }
}