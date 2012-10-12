namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides an interface for connection drivers.
    /// </summary>
    public interface IConnectionDriver
    {
        #region Methods

        /// <summary>
        /// Modified the status of a pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The pin status.</param>
        void Write(ProcessorPin pin, bool value);

        /// <summary>
        /// Reads the status of the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin status.</returns>
        bool Read(ProcessorPin pin);

        /// <summary>
        /// Exports the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Export(PinConfiguration pin);

        /// <summary>
        /// Unexports the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Unexport(PinConfiguration pin);

        #endregion
    }
}