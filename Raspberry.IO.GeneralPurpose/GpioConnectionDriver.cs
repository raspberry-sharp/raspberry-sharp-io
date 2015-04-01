#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Raspberry.IO.Interop;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the default connection driver that uses memory for accesses and files for edge detection.
    /// </summary>
    public class GpioConnectionDriver : IGpioConnectionDriver
    {
        #region Fields

        private const string gpioPath = "/sys/class/gpio";

        private readonly IntPtr gpioAddress;
        private readonly Dictionary<ProcessorPin, PinResistor> pinResistors = new Dictionary<ProcessorPin, PinResistor>();
        private readonly Dictionary<ProcessorPin, PinPoll> pinPolls = new Dictionary<ProcessorPin, PinPoll>();

        /// <summary>
        /// The default timeout (5 seconds).
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The minimum timeout (1 milliseconds)
        /// </summary>
        public static readonly TimeSpan MinimumTimeout = TimeSpan.FromMilliseconds(1);

        private static readonly TimeSpan resistorSetDelay = TimeSpanUtility.FromMicroseconds(5);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionDriver"/> class.
        /// </summary>
        public GpioConnectionDriver() {
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
        /// <see cref="GpioConnectionDriver"/> is reclaimed by garbage collection.
        /// </summary>
        ~GpioConnectionDriver()
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
            return GpioConnectionDriverCapabilities.CanSetPinResistor | GpioConnectionDriverCapabilities.CanSetPinDetectedEdges;
        }

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            var gpioId = string.Format("gpio{0}", (int)pin);
            if (Directory.Exists(Path.Combine(gpioPath, gpioId)))
            {
                // Reinitialize pin virtual file
                using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "unexport"), false))
                    streamWriter.Write((int) pin);
            }

            // Export pin for file mode
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "export"), false))
                streamWriter.Write((int)pin);

            // Set the direction on the pin and update the exported list
            SetPinMode(pin, direction == PinDirection.Input ? Interop.BCM2835_GPIO_FSEL_INPT : Interop.BCM2835_GPIO_FSEL_OUTP);

            // Set direction in pin virtual file
            var filePath = Path.Combine(gpioId, "direction");
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, filePath), false))
                streamWriter.Write(direction == PinDirection.Input ? "in" : "out");

            if (direction == PinDirection.Input)
            {
                PinResistor pinResistor;
                if (!pinResistors.TryGetValue(pin, out pinResistor) || pinResistor != PinResistor.None)
                    SetPinResistor(pin, PinResistor.None);

                SetPinDetectedEdges(pin, PinDetectedEdges.Both);
                InitializePoll(pin);
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
            switch (resistor)
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
        /// <remarks>By default, both edges may be detected on input pins.</remarks>
        public void SetPinDetectedEdges(ProcessorPin pin, PinDetectedEdges edges)
        {
            var edgePath = Path.Combine(gpioPath, string.Format("gpio{0}/edge", (int)pin));
            using (var streamWriter = new StreamWriter(edgePath, false))
                streamWriter.Write(ToString(edges));
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero" />.</param>
        /// <remarks>
        /// If <c>timeout</c> is set to <see cref="TimeSpan.Zero" />, a 5 seconds timeout is used.
        /// </remarks>
        public void Wait(ProcessorPin pin, bool waitForUp = true, TimeSpan timeout = new TimeSpan())
        {
            var pinPoll = pinPolls[pin];
            if (Read(pin) == waitForUp)
                return;

            var actualTimeout = GetActualTimeout(timeout);

            while (true)
            {
                // TODO: timeout after the remaining amount of time.
                var waitResult = Interop.epoll_wait(pinPoll.PollDescriptor, pinPoll.OutEventPtr, 1, (int)actualTimeout.TotalMilliseconds);
                if (waitResult > 0)
                {
                    if (Read(pin) == waitForUp)
                        break;
                }
                else if (waitResult == 0)
                    throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Operation timed out after waiting {0}ms for the pin {1} to be {2}", actualTimeout, pin, (waitForUp ? "up" : "down")));
                else
                    throw new IOException("Call to epoll_wait API failed");
            }
        }

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Release(ProcessorPin pin)
        {
            UninitializePoll(pin);
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "unexport"), false))
                streamWriter.Write((int)pin);

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
            SafeWriteUInt32(pinGroupAddress, (uint)1 << shift);
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
            var offset = Math.DivRem((int)pin, 32, out shift);

            var pinGroupAddress = gpioAddress + (int)(Interop.BCM2835_GPLEV0 + offset);
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
            var pinGroupAddress = gpioAddress + (int)(Interop.BCM2835_GPLEV0 + (uint)0 * 4);
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

        private static TimeSpan GetActualTimeout(TimeSpan timeout)
        {
            if (timeout > TimeSpan.Zero)
                return timeout;
            
            if (timeout < TimeSpan.Zero)
                return MinimumTimeout;
            
            return DefaultTimeout;
        }

        private void InitializePoll(ProcessorPin pin)
        {
            lock (pinPolls)
            {
                PinPoll poll;
                if (pinPolls.TryGetValue(pin, out poll))
                    return;

                var pinPoll = new PinPoll();

                pinPoll.PollDescriptor = Interop.epoll_create(1);
                if (pinPoll.PollDescriptor < 0)
                    throw new IOException("Call to epoll_create(1) API failed with the following return value: " + pinPoll.PollDescriptor);

                var valuePath = Path.Combine(gpioPath, string.Format("gpio{0}/value", (int)pin));
                
                pinPoll.FileDescriptor = UnixFile.OpenFileDescriptor(valuePath, UnixFileMode.ReadOnly | UnixFileMode.NonBlocking);

                var ev = new Interop.epoll_event
                {
                    events = (Interop.EPOLLIN | Interop.EPOLLET | Interop.EPOLLPRI),
                    data = new Interop.epoll_data { fd = pinPoll.FileDescriptor }
                };

                pinPoll.InEventPtr = Marshal.AllocHGlobal(64);
                Marshal.StructureToPtr(ev, pinPoll.InEventPtr, false);

                var controlResult = Interop.epoll_ctl(pinPoll.PollDescriptor, Interop.EPOLL_CTL_ADD, pinPoll.FileDescriptor, pinPoll.InEventPtr);
                if (controlResult != 0)
                    throw new IOException("Call to epoll_ctl(EPOLL_CTL_ADD) API failed with the following return value: " + controlResult);

                pinPoll.OutEventPtr = Marshal.AllocHGlobal(64);
                pinPolls[pin] = pinPoll;
            }
        }

        private void UninitializePoll(ProcessorPin pin)
        {
            PinPoll poll;
            if (pinPolls.TryGetValue(pin, out poll))
            {
                pinPolls.Remove(pin);

                var controlResult = poll.InEventPtr != IntPtr.Zero ? Interop.epoll_ctl(poll.PollDescriptor, Interop.EPOLL_CTL_DEL, poll.FileDescriptor, poll.InEventPtr) : 0;

                Marshal.FreeHGlobal(poll.InEventPtr);
                Marshal.FreeHGlobal(poll.OutEventPtr);

                UnixFile.CloseFileDescriptor(poll.PollDescriptor);
                UnixFile.CloseFileDescriptor(poll.FileDescriptor);

                if (controlResult != 0)
                    throw new IOException("Call to epoll_ctl(EPOLL_CTL_DEL) API failed with the following return value: " + controlResult);
            }
        }

        private static string ToString(PinDetectedEdges edges)
        {
            switch (edges)
            {
                case PinDetectedEdges.Both:
                    return "both";
                case PinDetectedEdges.Rising:
                    return "rising";
                case PinDetectedEdges.Falling:
                    return "falling";
                case PinDetectedEdges.None:
                    return "none";
                default:
                    throw new ArgumentOutOfRangeException("edges", edges, string.Format(CultureInfo.InvariantCulture, "{0} is not a valid value for edge detection", edges));
            }
        }

        private void SetPinResistorClock(ProcessorPin pin, bool on)
        {
            int shift;
            var offset = Math.DivRem((int)pin, 32, out shift);

            var clockAddress = gpioAddress + (int)(Interop.BCM2835_GPPUDCLK0 + offset);
            SafeWriteUInt32(clockAddress, (uint)(on ? 1 : 0) << shift);
        }

        private void WriteResistor(uint resistor)
        {
            var resistorPin = gpioAddress + (int)Interop.BCM2835_GPPUD;
            SafeWriteUInt32(resistorPin, resistor);
        }

        private void SetPinMode(ProcessorPin pin, uint mode)
        {
            // Function selects are 10 pins per 32 bit word, 3 bits per pin
            var pinModeAddress = gpioAddress + (int)(Interop.BCM2835_GPFSEL0 + 4 * ((int)pin / 10));

            var shift = 3 * ((int)pin % 10);
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
                return (uint)Marshal.ReadInt32(address);
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

        private struct PinPoll
        {
            public int FileDescriptor;
            public int PollDescriptor;
            public IntPtr InEventPtr;
            public IntPtr OutEventPtr;
        }

        #endregion
    }
}