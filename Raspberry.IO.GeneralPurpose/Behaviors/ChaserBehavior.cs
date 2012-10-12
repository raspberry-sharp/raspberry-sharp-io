#region References

using System.Collections.Generic;

#endregion

namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Represents a chaser behavior.
    /// </summary>
    public class ChaserBehavior : PinsBehavior
    {
        #region Fields

        private bool wayOut;
        private bool roundTrip;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaserBehavior"/> class.
        /// </summary>
        /// <param name="configurations">The configurations.</param>
        public ChaserBehavior(IEnumerable<PinConfiguration> configurations) : base(configurations)
        {
            Width = 1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to roundtrip.
        /// </summary>
        /// <value>
        ///   <c>true</c> if roundtrip is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool RoundTrip
        {
            get { return roundTrip; }
            set
            {
                roundTrip = value;
                wayOut = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChaserBehavior"/> must loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loop is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets the width of the enlightned leds.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

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
            return WidthBefore;
        }

        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        protected override void ProcessStep(int step)
        {
            var minEnabledStep = step - WidthBefore;
            var maxEnabledStep = step + WidthAfter;

            for (var i = 0; i < Configurations.Length; i++)
            {
                var configuration = Configurations[i];
                if (!Overflow)
                    Connection[configuration] = (i >= minEnabledStep && i <= maxEnabledStep);
                else
                    Connection[configuration] = (i >= minEnabledStep && i <= maxEnabledStep)
                                                || (maxEnabledStep >= Configurations.Length && i <= maxEnabledStep%Configurations.Length)
                                                || (minEnabledStep < 0 && i >= minEnabledStep + Configurations.Length);
            }
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

        #endregion

        #region Private Helpers

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

        #endregion
    }
}