using System;

namespace Raspberry.IO.GeneralPurpose
{
    public class PinStatusEventArgs : EventArgs
    {
        public PinConfiguration Configuration { get; internal set; }
        public bool Enabled { get; internal set; }
    }
}