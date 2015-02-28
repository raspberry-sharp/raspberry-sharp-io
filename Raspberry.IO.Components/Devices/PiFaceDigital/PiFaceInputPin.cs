using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raspberry.IO.Components.Devices.PiFaceDigital
{
    public class PiFaceInputPin : PiFacePin
    {

        #region events
        /// <summary>
        /// Event fired when the state of a pin changes
        /// </summary>
        public event InputPinChangedHandler OnStateChanged;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="PiFaceInputPin"/> class.
        /// </summary>
        /// <param name="pinNumber">Number of the pin in the range 0 to 7</param>
        internal PiFaceInputPin(int pinNumber)
            : base(pinNumber)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates that sensor grounding the input pin is closed
        /// </summary>
        public bool IsGrounded { get { return !state; } }

        /// <summary>
        /// Gets the state of the pin, note true indicates the pin is high i.e. open.
        /// if this throws off your logic use the IsGrounded property instead.
        /// </summary>
        public bool State
        {
            get { return state; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the state of this pin based on a byte that contains the state of every pin
        /// </summary>
        /// <param name="allPinState">byte with all pin values</param>
        internal override void Update(byte allPinState)
        {
            var oldState = state;
            state = (allPinState & mask) > 0;
            if (oldState != state && OnStateChanged != null)
            {
                OnStateChanged(this, new InputPinChangedArgs { pin = this });
            }
        }

        /// <summary>
        /// helper to set the state of every pin in a collection
        /// </summary>
        /// <param name="inputPins"></param>
        /// <param name="allPinState"></param>
        internal static void SetAllPinStates(IEnumerable<PiFaceInputPin> inputPins, byte allPinState)
        {
            foreach (var inputPin in inputPins)
            {
                inputPin.Update(allPinState);
            }
        }

        #endregion
    }
}
