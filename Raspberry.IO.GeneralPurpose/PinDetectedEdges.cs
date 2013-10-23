#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents detected edges.
    /// </summary>
    [Flags]
    public enum PinDetectedEdges
    {
        /// <summary>
        /// No changes are detected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Rising edge changes are detected.
        /// </summary>
        Rising = 1,

        /// <summary>
        /// Falling edge changes are detected.
        /// </summary>
        Falling = 2,

        /// <summary>
        /// Both changes are detected.
        /// </summary>
        Both = Rising | Falling
    }
}