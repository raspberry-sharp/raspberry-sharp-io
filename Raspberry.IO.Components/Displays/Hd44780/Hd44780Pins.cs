#region References

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Raspberry.IO.Components.Displays.Hd44780
{
    /// <summary>
    /// Represents the pins of a HD44780 LCD display.
    /// </summary>
    public class Hd44780Pins : IDisposable
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780Pins"/> class.
        /// </summary>
        /// <param name="registerSelect">The register select.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="data">The data.</param>
        public Hd44780Pins(IOutputBinaryPin registerSelect, IOutputBinaryPin clock, IEnumerable<IOutputBinaryPin> data)
            : this(registerSelect, clock, data.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hd44780Pins"/> class.
        /// </summary>
        /// <param name="registerSelect">The register select.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="data">The data.</param>
        public Hd44780Pins(IOutputBinaryPin registerSelect, IOutputBinaryPin clock, params IOutputBinaryPin[] data)
        {
            RegisterSelect = registerSelect;
            Clock = clock;
            Data = data;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// The register select (RS) pin.
        /// </summary>
        public IOutputBinaryPin RegisterSelect { get; private set; }

        /// <summary>
        /// The clock (EN) pin.
        /// </summary>
        public IOutputBinaryPin Clock { get; private set; }

        /// <summary>
        /// The backlight pin.
        /// </summary>
        public IOutputBinaryPin Backlight;

        /// <summary>
        /// The read write (RW) pin.
        /// </summary>
        public IOutputBinaryPin ReadWrite;

        /// <summary>
        /// The data pins.
        /// </summary>
        public IOutputBinaryPin[] Data { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {

            RegisterSelect.Dispose();
            Clock.Dispose();

            if (Backlight != null)
                Backlight.Dispose();
            if (ReadWrite != null)
                ReadWrite.Dispose();

            foreach (var dataPin in Data)
                dataPin.Dispose();
        }

        #endregion

    }
}