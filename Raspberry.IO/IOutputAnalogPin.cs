#region References

using System;

#endregion

namespace Raspberry.IO
{
    /// <summary>
    /// Provides an interface for output, analog pin.
    /// </summary>
    public interface IOutputAnalogPin : IDisposable
    {
        #region Methods

        /// <summary>
        /// Writes the specified value to the pin.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(AnalogValue value);

        #endregion
    }
}