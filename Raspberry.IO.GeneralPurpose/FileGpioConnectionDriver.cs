#region References

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connection driver using files.
    /// </summary>
    public class FileGpioConnectionDriver : IGpioConnectionDriver
    {
        #region Fields

        private const string gpioPath = "/sys/class/gpio";
        private static readonly Dictionary<ProcessorPin, FileGpioHandle> gpioPathList = new Dictionary<ProcessorPin, FileGpioHandle>();

        /// <summary>
        /// The default timeout (5 seconds).
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="FileGpioConnectionDriver"/> class.
        /// </summary>
        public FileGpioConnectionDriver()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                throw new NotSupportedException("FileGpioConnectionDriver is only supported in Unix");
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
            return GpioConnectionDriverCapabilities.None;
        }

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            Release(pin);

            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "export"), false))
                streamWriter.Write((int)pin);

            if (!gpioPathList.ContainsKey(pin))
            {
                var gpio = new FileGpioHandle { GpioPath = GuessGpioPath(pin) };
                gpioPathList.Add(pin, gpio);
            }

            var filePath = Path.Combine(gpioPathList[pin].GpioPath, "direction");
            try {
                SetPinDirection(filePath, direction);
            } 
            catch (UnauthorizedAccessException) {
                // program hasn't been started as root, give it a second to correct file permissions
                Thread.Sleep(TimeSpan.FromSeconds(1));
                SetPinDirection(filePath, direction);
            }

            gpioPathList[pin].GpioStream = new FileStream(Path.Combine(GuessGpioPath(pin), "value"), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>
        /// Sets the pin resistor.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public void SetPinResistor(ProcessorPin pin, PinResistor resistor)
        {
            throw new NotSupportedException("Resistor are not supported by file GPIO connection driver");
        }

        /// <summary>
        /// Sets the detected edges on an input pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="edges">The edges.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// By default, both edges may be detected on input pins.
        /// </remarks>
        public void SetPinDetectedEdges(ProcessorPin pin, PinDetectedEdges edges)
        {
            throw new NotSupportedException("Edge detection is not supported by file GPIO connection driver");
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up. Default value is <c>true</c>.</param>
        /// <param name="timeout">The timeout. Default value is <see cref="TimeSpan.Zero" />.</param>
        /// <remarks>
        /// If <c>timeout</c> is set to <see cref="TimeSpan.Zero" />, a 5 second timeout is used.
        /// </remarks>
        public void Wait(ProcessorPin pin, bool waitForUp = true, TimeSpan timeout = new TimeSpan())
        {
            var startWait = DateTime.UtcNow;
            if (timeout == TimeSpan.Zero)
                timeout = DefaultTimeout;

            while (Read(pin) != waitForUp)
            {
                if (DateTime.UtcNow - startWait >= timeout)
                    throw new TimeoutException("A timeout occurred while waiting for pin status to change");
            }
        }

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Release(ProcessorPin pin)
        {
            if (gpioPathList.ContainsKey(pin) && gpioPathList[pin].GpioStream != null)
            {
                gpioPathList[pin].GpioStream.Close();
                gpioPathList[pin].GpioStream = null;
            } 
            if (Directory.Exists(GuessGpioPath(pin)))
                using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "unexport"), false))
                    streamWriter.Write((int)pin);
        }

        /// <summary>
        /// Modified the status of a pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="value">The pin status.</param>
        public void Write(ProcessorPin pin, bool value)
        {
            gpioPathList[pin].GpioStream.Seek(0, SeekOrigin.Begin);
            gpioPathList[pin].GpioStream.WriteByte(value ? (byte)'1' : (byte)'0');
            gpioPathList[pin].GpioStream.Flush();
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
            gpioPathList[pin].GpioStream.Seek(0, SeekOrigin.Begin);
            var rawValue = (char)gpioPathList[pin].GpioStream.ReadByte();
            gpioPathList[pin].GpioStream.Flush();
            return rawValue == '1';
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
            return pins.Enumerate()
                .Select(p => Read(p) ? (ProcessorPins) ((uint) 1 << (int) p) : ProcessorPins.None)
                    .Aggregate(
                        ProcessorPins.None, 
                        (a, p) => a | p);
        }

        #endregion

        #region Private Helpers

        private static void SetPinDirection(string fullFilePath, PinDirection direction) {
            using (var streamWriter = new StreamWriter(fullFilePath, false)) {
                streamWriter.Write(direction == PinDirection.Input
                                   ? "in"
                                   : "out");
            }
        }

        private static string GuessGpioPath(ProcessorPin pin)
        {
            // by default use Raspberry Pi pin path format
            string gpioId = string.Format("gpio{0}", (int)pin);
            string pinPath = Path.Combine(gpioPath, gpioId);
            // verify/lookup pin path
            if (!Directory.Exists(pinPath))
            {
                // check for sunxi gpio path name format (eg. "gpio11_pe10", "gpio2_pi21", ...)
                string[] dirs = Directory.GetDirectories(gpioPath);
                foreach (string d in dirs)
                {
                    if (d.StartsWith(pinPath + "_"))
                    {
                        pinPath = d;
                        break;
                    }
                }
            } 
            return pinPath;
        }

        #endregion
    }
}