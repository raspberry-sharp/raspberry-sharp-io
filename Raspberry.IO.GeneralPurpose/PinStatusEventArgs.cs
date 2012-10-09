using System;

namespace Raspberry.IO.GeneralPurpose
{
    public class PinStatusEventArgs : EventArgs
    {
        public PinConfiguration Pin { get; internal set; }
        public bool Enabled { get; internal set; }
    }
}