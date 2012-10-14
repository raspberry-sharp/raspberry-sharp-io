#region References

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents connected pins.
    /// </summary>
    public class ConnectedPins : IEnumerable<ConnectedPin>
    {
        #region Fields

        private readonly GpioConnection connection;

        #endregion

        #region Instance Management

        internal ConnectedPins(GpioConnection connection)
        {
            this.connection = connection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        public ConnectedPin this[ProcessorPin pin]
        {
            get { return new ConnectedPin(connection, connection.GetConfiguration(pin)); }
        }

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        public ConnectedPin this[string name]
        {
            get { return new ConnectedPin(connection, connection.GetConfiguration(name)); }
        }

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        public ConnectedPin this[ConnectorPin pin]
        {
            get { return this[pin.ToProcessor()]; }
        }

        /// <summary>
        /// Gets the status of the specified pin.
        /// </summary>
        public ConnectedPin this[PinConfiguration pin]
        {
            get { return new ConnectedPin(connection, pin); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ConnectedPin> GetEnumerator()
        {
            return connection.Configurations.Select(c => new ConnectedPin(connection, c)).GetEnumerator();
        }

        #endregion
    }
}