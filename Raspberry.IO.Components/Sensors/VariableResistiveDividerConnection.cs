#region References

using System;

#endregion

namespace Raspberry.IO.Components.Sensors
{
    /// <summary>
    /// Represents a connection to an analog value coming from a resistive voltage divider
    /// </summary>
    public class VariableResistiveDividerConnection : IDisposable
    {
        #region Fields

        private readonly IInputAnalogPin analogPin;
        private readonly Func<AnalogValue, decimal> resistorEvalFunc;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableResistiveDividerConnection"/> class.
        /// </summary>
        /// <param name="analogPin">The analog pin.</param>
        /// <param name="resistorEvalFunc">The resistor eval function.</param>
        /// <remarks>Methods from <see cref="ResistiveDivider"/> should be used.</remarks>
        public VariableResistiveDividerConnection(IInputAnalogPin analogPin, Func<AnalogValue, decimal> resistorEvalFunc)
        {
            this.analogPin = analogPin;
            this.resistorEvalFunc = resistorEvalFunc;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the resistor, in Ohms.
        /// </summary>
        /// <returns>The resistor value.</returns>
        public decimal GetResistor()
        {
            var value = analogPin.Read();
            return resistorEvalFunc(value);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            analogPin.Dispose();
        }

        #endregion
    }
}
