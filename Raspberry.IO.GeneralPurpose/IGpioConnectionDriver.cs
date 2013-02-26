namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides an interface for connection drivers.
    /// </summary>
    public interface IGpioConnectionDriver
    {
        #region Methods

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        void Allocate(ProcessorPin pin, PinDirection direction);
        
        /// <summary>
        /// Sets the pin resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        void SetPinResistor(ProcessorPin pin, PinResistor resistor);

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        void Release(ProcessorPin pin);

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
        /// Reads the status of the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>The pins status.</returns>
        ProcessorPins Read(ProcessorPins pins);
        
        #endregion
    }
}