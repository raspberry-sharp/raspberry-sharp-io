<<<<<<< HEAD
using System;

namespace Raspberry.IO.GeneralPurpose
{
    public class PinStatusEventArgs : EventArgs
    {
        public PinConfiguration Pin { get; internal set; }
        public bool IsActive { get; internal set; }
    }
=======
using System;

namespace Raspberry.IO.GeneralPurpose
{
    public class PinStatusEventArgs : EventArgs
    {
        public PinConfiguration Pin { get; internal set; }
        public bool IsActive { get; internal set; }
    }
>>>>>>> Initial Import
}