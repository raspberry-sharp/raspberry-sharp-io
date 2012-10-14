#region References

using System;
using System.Runtime.InteropServices;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connection driver that uses memory.
    /// </summary>
    /// <remarks>Based on bmc2835_gpio library.</remarks>
    public class MemoryConnectionDriver : IConnectionDriver
    {
        #region Fields

        private static readonly Lazy<bool> initialized = new Lazy<bool>(() => bcm2835_init() != 0);

        #endregion

        #region Methods

        /// <summary>
        /// Modified the status of a pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The pin status.</param>
        public void Write(ProcessorPin pin, bool value)
        {
            bcm2835_gpio_write((uint) pin, (uint) (value ? 1 : 0));
        }

        /// <summary>
        /// Reads the status of the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        /// The pin status.
        /// </returns>
        public bool Read(ProcessorPin pin)
        {
            var value = bcm2835_gpio_lev((uint) pin);
            return value != 0;
        }

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            if (!initialized.Value)
                throw new InvalidOperationException("Unabled to initialize driver");

            // Set the direction on the pin and update the exported list
            // BCM2835_GPIO_FSEL_INPT = 0
            // BCM2835_GPIO_FSEL_OUTP = 1
            bcm2835_gpio_fsel((uint) pin, (uint) (direction == PinDirection.Input ? 0 : 1));

            if (direction == PinDirection.Input)
            {
                // BCM2835_GPIO_PUD_OFF = 0b00 = 0
                // BCM2835_GPIO_PUD_DOWN = 0b01 = 1
                // BCM2835_GPIO_PUD_UP = 0b10 = 2
                bcm2835_gpio_set_pud((uint) pin, 0);
            }
        }
        
        public void Release(ProcessorPin pin)
        {
            // TODO: Release pin ?
        }

        #endregion

        #region Interop Methods

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_init")]
        static extern int bcm2835_init();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_fsel")]
        static extern void bcm2835_gpio_fsel(uint pin, uint mode);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_write")]
        static extern void bcm2835_gpio_write(uint pin, uint value);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_lev")]
        static extern uint bcm2835_gpio_lev(uint pin);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_set_pud")]
        static extern void bcm2835_gpio_set_pud(uint pin, uint pud);

        #endregion
    }
}