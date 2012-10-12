#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents event arguments related to pin status.
    /// </summary>
    public class PinStatusEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public PinConfiguration Configuration { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PinStatusEventArgs"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; internal set; }

        #endregion
    }
}