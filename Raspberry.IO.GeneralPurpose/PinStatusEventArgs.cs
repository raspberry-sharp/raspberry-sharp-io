#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    public class PinStatusEventArgs : EventArgs
    {
        #region Properties

        public PinConfiguration Configuration { get; internal set; }
        public bool Enabled { get; internal set; }

        #endregion
    }
}