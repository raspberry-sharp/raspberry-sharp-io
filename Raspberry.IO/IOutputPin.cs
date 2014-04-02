#region References

using System;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides an interface for output pins.
    /// </summary>
    public interface IOutputPin : IDisposable
    {
        #region Methods

        /// <summary>
        /// Writes the specified state.
        /// </summary>
        /// <param name="state">The pin state.</param>
        void Write(bool state);

        #endregion
    }
}