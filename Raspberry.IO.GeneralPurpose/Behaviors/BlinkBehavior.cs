#region References

using System.Collections.Generic;

#endregion

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Represents a simple blink behavior.
    /// </summary>
    public class BlinkBehavior : PinsBehavior
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkBehavior"/> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        public BlinkBehavior(IEnumerable<PinConfiguration> configurations) : base(configurations){}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of times the behavior may blink.
        /// </summary>
        /// <value>
        /// The number of times the behavior may blink.
        /// </value>
        public int Count { get; set; }

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
            return 1;
        }

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected override void ProcessStep(int step)
        {
            foreach (var configuration in Configurations)
                Connection.Toggle(configuration);
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
            step++;
            return step <= Count*2;
        }

        #endregion
    }
}