#region References

using System;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    public class SpiSlaveSelection : IDisposable
    {
        #region Fields

        private readonly SpiConnection connection;

        #endregion

        #region Instance Management

        internal SpiSlaveSelection(SpiConnection connection)
        {
            this.connection = connection;
        }

        void IDisposable.Dispose()
        {
            connection.DeselectSlave();
        }

        #endregion
    }
}