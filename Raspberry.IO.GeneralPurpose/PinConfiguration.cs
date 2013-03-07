#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the configuration of a pin.
    /// </summary>
    public abstract class PinConfiguration
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="PinConfiguration"/> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        protected PinConfiguration(ProcessorPin pin)
        {
            Pin = pin;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pin.
        /// </summary>
        public ProcessorPin Pin { get; private set; }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public abstract PinDirection Direction { get; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PinConfiguration"/> is reversed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reversed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>If set to <c>true</c>, pin value will be enabled when no signal is present, and disabled when a signal is present.</remarks>
        public bool Reversed { get; set; }

        /// <summary>
        /// Gets or sets the status changed action.
        /// </summary>
        /// <value>
        /// The status changed action.
        /// </value>
        public Action<bool> StatusChangedAction { get; set; }

        #endregion

        #region Internal Methods

        internal bool GetEffective(bool value)
        {
            return Reversed ? !value : value;
        }

        #endregion
    }
}