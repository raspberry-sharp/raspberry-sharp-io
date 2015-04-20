namespace Raspberry.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Defines an I2C read action.
    /// </summary>
    public class I2cReadAction : I2cAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2cReadAction"/> class.
        /// </summary>
        /// <param name="buffer">The buffer which should be used to store the received data.</param>
        public I2cReadAction(params byte[] buffer)
            : base(buffer)
        {
        }
    }
}
