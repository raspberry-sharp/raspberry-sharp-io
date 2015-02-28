using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raspberry.IO.Components.Devices.PiFaceDigital
{
    /// <summary>
    /// Represents the pins on a PiFace Digital board
    /// </summary>
    public abstract class PiFacePin
    {


        #region Fields

        /// <summary>
        /// bit mask that allows this pin to get / set it's value from a byte
        /// </summary>
        protected byte mask;

        /// <summary>
        /// state of this pin
        /// </summary>
        protected bool state;

        #endregion



        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="PiFacePin"/> class.
        /// </summary>
        /// <param name="pinNumber">Number of the pin in the range 0 to 7</param>
        internal PiFacePin(int pinNumber)
        {
            if (pinNumber < 0 || pinNumber > 7)
            {
                throw new ArgumentOutOfRangeException("pin numbers must be in the range 0 to 7");
            }
            mask = (byte)(1 << pinNumber);
            Id = pinNumber;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of the pin 0..7
        /// </summary>
        public int Id { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Update this pin based on a byte that contains the data for every pin
        /// </summary>
        /// <param name="allPinState">byte with all pin values</param>
        internal virtual void Update(byte allPinState)
        {
            state = (allPinState & mask) > 0;
        }

        /// <summary>
        /// Returns a byte representing the state of all pins
        /// </summary>
        /// <param name="pins">collection of pins to aggregate over</param>
        /// <returns>byte of all pin state</returns>
        internal static byte AllPinState(IEnumerable<PiFacePin> pins)
        {
            byte allPinState = 0;
            foreach (var pin in pins)
            {
                if (pin.state)
                {
                    allPinState = (byte)(allPinState | pin.mask);
                }
            }
            return allPinState;
        }


        #endregion
    }
}
