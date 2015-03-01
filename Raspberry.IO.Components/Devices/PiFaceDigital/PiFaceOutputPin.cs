using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raspberry.IO.Components.Devices.PiFaceDigital
{
    /// <summary>
    /// Derivative of PiFacePin that allows setting of the internal state
    /// </summary>
    public class PiFaceOutputPin : PiFacePin
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="PiFaceOutputPin"/> class.
        /// </summary>
        /// <param name="pinNumber">Number of the pin in the range 0 to 7</param>
        internal PiFaceOutputPin(int pinNumber)
            : base(pinNumber)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the state of the pin in software but does not update the PiFaceDigital device
        /// Allows individual pins to be modified then everything updated with a call to UpdatePiFaceOutputPins
        /// </summary>
        public bool State
        {
            get { return state; }
            set { state = value; }
        }

        #endregion
    }
}
