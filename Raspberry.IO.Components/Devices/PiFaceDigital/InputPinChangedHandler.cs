using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raspberry.IO.Components.Devices.PiFaceDigital
{
    /// <summary>
    /// delgate for pin state changed events. The pin that changed is in the args 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InputPinChangedHandler (object sender, InputPinChangedArgs e);

}
