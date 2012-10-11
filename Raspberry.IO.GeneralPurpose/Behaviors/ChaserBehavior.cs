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
    
    public class ChaserBehavior : PinsBehavior
    {
        public ChaserBehavior(IEnumerable<PinConfiguration> configurations) : base(configurations)
        {
            Width = 1;
        }

        private bool wayOut;

        public bool RoundTrip { get; set; }
        public bool Descending { get; set; }
        public bool Loop { get; set; }
        public int Width { get; set; }

        protected override int FirstStep()
        {
            wayOut = true;
            return WidthBefore;
        }

        protected override void Step(int step)
        {
            var minEnabledStep = step - WidthBefore;
            var maxEnabledStep = step + WidthAfter;

            for (var i = 0; i < Configurations.Length; i++)
            {
                var configuration = Configurations[Descending ? Configurations.Length - 1 - i : i];
                if (!Overflow)
                    Connection[configuration] = (i >= minEnabledStep && i <= maxEnabledStep);
                else
                    Connection[configuration] = (i >= minEnabledStep && i <= maxEnabledStep)
                        || (maxEnabledStep >= Configurations.Length && i <= maxEnabledStep % Configurations.Length)
                        || (minEnabledStep < 0 && i >= minEnabledStep + Configurations.Length);
            }
        }

        protected override bool TryNextStep(ref int step)
        {
            if (wayOut)
            {
                if (step == MaximumStep)
                {
                    if (RoundTrip)
                    {
                        wayOut = false;
                        step--;
                    }
                    else if (Loop)
                        step = MinimumStep;
                    else
                        return false;
                }
                else
                    step++;
            }
            else
            {
                if (step == MinimumStep)
                {
                    if (Loop && RoundTrip)
                    {
                        wayOut = true;
                        step++;
                    }
                    else if (Loop)
                        step = MaximumStep;
                    else
                        return false;
                }
                else step--;
            }

            return true;
        }

        private bool Overflow
        {
            get { return Loop && !RoundTrip; }
        }

        private int MinimumStep
        {
            get { return Overflow ? 0 : WidthBefore; }
        }

        private int MaximumStep
        {
            get { return Configurations.Length - 1 - (Overflow ? 0 : WidthAfter); }
        }

        private int WidthBefore
        {
            get { return (Width%2) == 1 ? (Width - 1)/2 : Width/2; }
        }

        private int WidthAfter
        {
            get { return (Width%2) == 1 ? (Width - 1)/2 : Width/2 - 1; }
        }
    }
}