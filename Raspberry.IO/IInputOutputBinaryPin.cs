namespace Raspberry.IO
{
    /// <summary>
    /// Provides an interface for bidirectional binary pins.
    /// </summary>
    public interface IInputOutputBinaryPin : IInputBinaryPin, IOutputBinaryPin
    {
        /// <summary>
        /// Prepares the pin to act as an input.
        /// </summary>
        void AsInput();

        /// <summary>
        /// Prepares the pin to act as an output.
        /// </summary>
        void AsOutput();
    }
}