#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Raspberry.IO.Interop;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connection driver that uses memory.
    /// </summary>
    public class MemoryGpioConnectionDriver : IGpioConnectionDriver
    {
        #region Fields

        private readonly IntPtr gpioAddress;
        private readonly Dictionary<ProcessorPin, PinResistor> pinResistors = new Dictionary<ProcessorPin, PinResistor>();  

        /// <summary>
        /// The default timeout (5 seconds).
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        private static readonly TimeSpan resistorSetDelay = TimeSpanUtility.FromMicroseconds(5);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryGpioConnectionDriver"/> class.
        /// </summary>
        public MemoryGpioConnectionDriver() {
            using (var memoryFile = UnixFile.Open("/dev/mem", UnixFileMode.ReadWrite | UnixFileMode.Synchronized)) {
                gpioAddress = MemoryMap.Create(
                    IntPtr.Zero, 
                    Interop.BCM2835_BLOCK_SIZE,
                    MemoryProtection.ReadWrite, 
                    MemoryFlags.Shared, 
                    memoryFile.Descriptor,
                    GetProcessorBaseAddress(Board.Current.Processor));
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MemoryGpioConnectionDriver"/> is reclaimed by garbage collection.
        /// </summary>
        ~MemoryGpioConnectionDriver()
        {
            MemoryMap.Close(gpioAddress, Interop.BCM2835_BLOCK_SIZE);
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Gets driver capabilities.
        /// </summary>
        /// <returns>The capabilites.</returns>
        GpioConnectionDriverCapabilities IGpioConnectionDriver.GetCapabilities()
        {
            return GetCapabilities();
        }

        /// <summary>
        /// Gets driver capabilities.
        /// </summary>
        /// <returns>The capabilites.</returns>
        public static GpioConnectionDriverCapabilities GetCapabilities()
        {
            return GpioConnectionDriverCapabilities.CanSetPinResistor | GpioConnectionDriverCapabilities.CanChangePinDirectionRapidly;
        }

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            // Set the direction on the pin and update the exported list
            SetPinMode(pin, direction == PinDirection.Input ? Interop.BCM2835_GPIO_FSEL_INPT : Interop.BCM2835_GPIO_FSEL_OUTP);

            if (direction == PinDirection.Input)
            {
                PinResistor pinResistor;
                if (!pinResistors.TryGetValue(pin, out pinResistor) || pinResistor != PinResistor.None)
                    SetPinResistor(pin, PinResistor.None);
            }
        }

        /// <summary>
        /// Sets the pin resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public void SetPinResistor(ProcessorPin pin, PinResistor resistor)
        {
            // Set the pullup/down resistor for a pin
            //
            // The GPIO Pull-up/down Clock Registers control the actuation of internal pull-downs on
            // the respective GPIO pins. These registers must be used in conjunction with the GPPUD
            // register to effect GPIO Pull-up/down changes. The following sequence of events is
            // required:
            // 1. Write to GPPUD to set the required control signal (i.e. Pull-up or Pull-Down or neither
            // to remove the current Pull-up/down)
            // 2. Wait 150 cycles ? this provides the required set-up time for the control signal
            // 3. Write to GPPUDCLK0/1 to clock the control signal into the GPIO pads you wish to
            // modify ? NOTE only the pads which receive a clock will be modified, all others will
            // retain their previous state.
            // 4. Wait 150 cycles ? this provides the required hold time for the control signal
            // 5. Write to GPPUD to remove the control signal
            // 6. Write to GPPUDCLK0/1 to remove the clock
            //
            // RPi has P1-03 and P1-05 with 1k8 pullup resistor

            uint pud;
            switch(resistor)
            {
                case PinResistor.None:
                    pud = Interop.BCM2835_GPIO_PUD_OFF;
                    break;
                case PinResistor.PullDown:
                    pud = Interop.BCM2835_GPIO_PUD_DOWN;
                    break;
                case PinResistor.PullUp:
                    pud = Interop.BCM2835_GPIO_PUD_UP;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("resistor", resistor, string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for pin resistor", resistor));
            }

            WriteResistor(pud);
            HighResolutionTimer.Sleep(resistorSetDelay);
            SetPinResistorClock(pin, true);
            HighResolutionTimer.Sleep(resistorSetDelay);
            WriteResistor(Interop.BCM2835_GPIO_PUD_OFF);
            SetPinResistorClock(pin, false);

            pinResistors[pin] = PinResistor.None;
        }

        /// <summary>
        /// Sets the detected edges on an input pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="edges">The edges.</param>
        /// <remarks>
        /// By default, both edges may be detected on input pins.
        /// </remarks>
        public void SetPinDetectedEdges(ProcessorPin pin, PinDetectedEdges edges)
        {
            throw new NotSupportedException("Edge detection is not supported by memory GPIO connection driver");
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero" />.</param>
        /// <remarks>
        /// If <c>timeout</c> is set to <see cref="TimeSpan.Zero" />, a default timeout of <see cref="DefaultTimeout"/> is used.
        /// </remarks>
        public void Wait(ProcessorPin pin, bool waitForUp = true, TimeSpan timeout = new TimeSpan())
        {
            var startWait = DateTime.UtcNow;
            if (timeout == TimeSpan.Zero)
                timeout = DefaultTimeout;

            while (Read(pin) != waitForUp)
            {
                if (DateTime.UtcNow >= startWait + timeout)
                    throw new TimeoutException("A timeout occurred while waiting for pin status to change");
            }
        }

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Release(ProcessorPin pin)
        {
            SetPinMode(pin, Interop.BCM2835_GPIO_FSEL_INPT);
        }

        /// <summary>
        /// Modified the status of a pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The pin status.</param>
        public void Write(ProcessorPin pin, bool value)
        {
            int shift;
            var offset = Math.DivRem((int)pin, 32, out shift);

            var pinGroupAddress = gpioAddress + (int)((value ? Interop.BCM2835_GPSET0 : Interop.BCM2835_GPCLR0) + offset);
            SafeWriteUInt32(pinGroupAddress, (uint) 1 << shift);
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
            int shift;
            var offset = Math.DivRem((int) pin, 32, out shift);

            var pinGroupAddress = gpioAddress + (int) (Interop.BCM2835_GPLEV0 + offset);
            var value = SafeReadUInt32(pinGroupAddress);

            return (value & (1 << shift)) != 0;
        }

        /// <summary>
        /// Reads the status of the specified pins.
        /// </summary>
        /// <param name="pins">The pins.</param>
        /// <returns>
        /// The pins status.
        /// </returns>
        public ProcessorPins Read(ProcessorPins pins)
        {
            var pinGroupAddress = gpioAddress + (int) (Interop.BCM2835_GPLEV0 + (uint) 0 * 4);
            var value = SafeReadUInt32(pinGroupAddress);

            return (ProcessorPins)((uint)pins & value);
        }

        #endregion

        #region Private Methods

        private static uint GetProcessorBaseAddress(Processor processor)
        {
            switch (processor)
            {
                case Processor.Bcm2708:
                    return Interop.BCM2835_GPIO_BASE;

                case Processor.Bcm2709:
                    return Interop.BCM2836_GPIO_BASE;

                default:
                    throw new ArgumentOutOfRangeException("processor");
            }
        }

        private void SetPinResistorClock(ProcessorPin pin, bool on)
        {
            int shift;
            var offset = Math.DivRem((int)pin, 32, out shift);

            var clockAddress = gpioAddress + (int)(Interop.BCM2835_GPPUDCLK0 + offset);
            SafeWriteUInt32(clockAddress, (uint) (on ? 1 : 0) << shift);
        }

        private void WriteResistor(uint resistor)
        {
            var resistorPin = gpioAddress + (int) Interop.BCM2835_GPPUD;
            SafeWriteUInt32(resistorPin, resistor);
        }

        private void SetPinMode(ProcessorPin pin, uint mode)
        {
            // Function selects are 10 pins per 32 bit word, 3 bits per pin
            var pinModeAddress = gpioAddress + (int) (Interop.BCM2835_GPFSEL0 + 4*((int)pin/10));

            var shift = 3*((int) pin%10);
            var mask = Interop.BCM2835_GPIO_FSEL_MASK << shift;
            var value = mode << shift;

            WriteUInt32Mask(pinModeAddress, value, mask);
        }

        private static void WriteUInt32Mask(IntPtr address, uint value, uint mask)
        {
            var v = SafeReadUInt32(address);
            v = (v & ~mask) | (value & mask);
            SafeWriteUInt32(address, v);
        }

        private static uint SafeReadUInt32(IntPtr address)
        {
            // Make sure we dont return the _last_ read which might get lost
            // if subsequent code changes to a different peripheral
            var ret = ReadUInt32(address);
            ReadUInt32(address);

            return ret;
        }

        private static uint ReadUInt32(IntPtr address)
        {
            unchecked
            {
                return (uint) Marshal.ReadInt32(address);
            }
        }

        private static void SafeWriteUInt32(IntPtr address, uint value)
        {
            // Make sure we don't rely on the first write, which may get
            // lost if the previous access was to a different peripheral.
            WriteUInt32(address, value);
            WriteUInt32(address, value);
        }

        private static void WriteUInt32(IntPtr address, uint value)
        {
            unchecked
            {
                Marshal.WriteInt32(address, (int)value);
            }
        }

        #endregion
    }
}