using System.Collections.Generic;

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public class BlinkBehavior : PinsBehavior
    {
        public BlinkBehavior(IEnumerable<PinConfiguration> configurations) : base(configurations){}

        public int Count { get; set; }

        protected override int FirstStep()
        {
            return 1;
        }

        protected override void Step(int step)
        {
            foreach (var configuration in Configurations)
                Connection.Toggle(configuration);
        }

        protected override bool TryNextStep(ref int step)
        {
            step++;
            return step <= Count*2;
        }
    }
}