using System.Collections.Generic;
using System.Linq;

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public class PatternBehavior : PinsBehavior
    {
        private bool wayOut;

        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<int> patterns) : this(configurations, (IEnumerable<long>) patterns.Select(i => (long)i)){}

        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<long> patterns) : base(configurations)
        {
            Patterns = patterns.ToArray();
        }

        public bool Loop { get; set; }

        public bool RoundTrip { get; set; }

        private long[] Patterns { get; set; }

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
    }
}