using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raspberry.IO.Components.Devices.PiFaceDigital
{
    public class InputPinChangedArgs : EventArgs
    {
        public PiFaceInputPin pin;
    }
}
