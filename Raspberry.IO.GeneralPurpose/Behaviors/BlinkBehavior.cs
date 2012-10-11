#region References

using System.Collections.Generic;

#endregion

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public class BlinkBehavior : PinsBehavior
    {
        #region Instance Management

        public BlinkBehavior(IEnumerable<PinConfiguration> configurations) : base(configurations){}

        #endregion

        #region Properties

        public int Count { get; set; }

        #endregion

        #region Protected Methods

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

        #endregion
    }
}