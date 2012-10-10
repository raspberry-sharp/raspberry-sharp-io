using System.Collections.Generic;

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public class ChaserBehavior : PinsBehavior
    {
        public ChaserBehavior(IEnumerable<PinConfiguration> configurations) : base(configurations) { }

        private bool wayOut;

        public bool RoundTrip { get; set; }
        public bool Descending { get; set; }
        public bool Loop { get; set; }

        protected override int FirstStep()
        {
            wayOut = true;
            return Descending ? Configurations.Length - 1 : 0;
        }

        protected override void Step(int step)
        {
            for (var i = 0; i < Configurations.Length; i++)
            {
                var configuration = Configurations[i];
                Connection[configuration] = (step == i);
            }
        }

        protected override bool TryNextStep(ref int step)
        {
            if (wayOut != Descending)
            {
                if (step == Configurations.Length - 1)
                {
                    if (RoundTrip && (Loop || !Descending))
                    {
                        wayOut = Descending;
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
                    if (RoundTrip && (Loop || Descending))
                    {
                        wayOut = !Descending;
                        step++;
                    }
                    else if (Loop)
                        step = Configurations.Length;
                    else
                        return false;
                }
                else step--;
            }

            return true;
        }
    }
}