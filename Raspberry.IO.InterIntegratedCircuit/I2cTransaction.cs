namespace Raspberry.IO.InterIntegratedCircuit
{
    using System;

    /// <summary>
    /// Defines an I2C data transaction.
    /// </summary>
    public class I2cTransaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2cTransaction"/> class.
        /// </summary>
        /// <param name="actions">The actions which should be performed within the transaction.</param>
        public I2cTransaction(params I2cAction[] actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException("actions");    
            }

            Actions = actions;
        }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        public I2cAction[] Actions { get; private set; }
    }
}
