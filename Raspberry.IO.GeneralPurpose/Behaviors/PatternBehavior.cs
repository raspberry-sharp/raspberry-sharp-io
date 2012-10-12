#region References

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Represents a pattern behavior.
    /// </summary>
    public class PatternBehavior : PinsBehavior
    {
        #region Fields

        private bool wayOut;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBehavior"/> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="patterns">The patterns.</param>
        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<int> patterns) : this(configurations, patterns.Select(i => (long) i)){}

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternBehavior"/> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        /// <param name="patterns">The patterns.</param>
        public PatternBehavior(IEnumerable<PinConfiguration> configurations, IEnumerable<long> patterns) : base(configurations)
        {
            Patterns = patterns.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PatternBehavior"/> must loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loop is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to roundtrip.
        /// </summary>
        /// <value>
        ///   <c>true</c> if round-trip is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool RoundTrip { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the first step.
        /// </summary>
        /// <returns>
        /// The first step.
        /// </returns>
        protected override int GetFirstStep()
        {
            wayOut = true;
            return 0;
        }

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected override void ProcessStep(int step)
        {
            var pattern = Patterns[step];

            for (var i = 0; i < Configurations.Length; i++)
                Connection[Configurations[i]] = ((pattern >> i) & 0x1) == 0x1;
        }

        /// <summary>
        /// Tries to get the next step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        ///   <c>true</c> if the behavior may continue; otherwise behavior will be stopped.
        /// </returns>
        protected override bool TryGetNextStep(ref int step)
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