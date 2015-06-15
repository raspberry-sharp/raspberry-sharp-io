namespace Raspberry.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Defines an I2C write action.
    /// </summary>
    public class I2cWriteAction : I2cAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2cWriteAction"/> class.
        /// </summary>
        /// <param name="buffer">The buffer with data which should be written.</param>
        public I2cWriteAction(params byte[] buffer)
            : base(buffer)
        {
        }
    }
}
