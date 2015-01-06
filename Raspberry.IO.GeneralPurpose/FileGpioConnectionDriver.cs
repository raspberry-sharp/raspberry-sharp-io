#region References

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;

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

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="FileGpioConnectionDriver"/> class.
        /// </summary>
        public FileGpioConnectionDriver()
        {
			if (System.Environment.OSVersion.Platform != PlatformID.Unix)
                throw new NotSupportedException("FileGpioConnectionDriver is only supported in Unix");
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            var gpioId = string.Format("gpio{0}", (int)pin);
            if (Directory.Exists(Path.Combine(gpioPath, gpioId)))
                Release(pin);

            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "export"), false))
                streamWriter.Write((int)pin);

            var filePath = Path.Combine(gpioPath, gpioId, "direction");
            try {
                SetPinDirection(filePath, direction);
            } catch (UnauthorizedAccessException) {
                // program hasn't been started as root, give it a second to correct file permissions
                Thread.Sleep(TimeSpan.FromSeconds(1));
                SetPinDirection(filePath, direction);
            }
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
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up.</param>
        /// <param name="timeout">The timeout, in milliseconds.</param>
        /// <exception cref="System.TimeoutException">A timeout occurred while waiting</exception>
        public void Wait(ProcessorPin pin, bool waitForUp = true, decimal timeout = 0)
        {
            var startWait = DateTime.Now;
            if (timeout == 0)
                timeout = 5000;

            while (Read(pin) != waitForUp)
            {
                if (DateTime.Now.Ticks - startWait.Ticks >= 10000 * timeout)
                    throw new TimeoutException("A timeout occurred while waiting for pin status to change");
            }
        }

        /// <summary>
        /// Releases the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Release(ProcessorPin pin)
        {
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
            var gpioId = string.Format("gpio{0}", (int) pin);
            var filePath = Path.Combine(gpioId, "value");
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, filePath), false))
                streamWriter.Write(value ? "1" : "0");
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
            var gpioId = string.Format("gpio{0}", (int) pin);
            var filePath = Path.Combine(gpioId, "value");

            using (var streamReader = new StreamReader(new FileStream(Path.Combine(gpioPath, filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var rawValue = streamReader.ReadToEnd();
                return !string.IsNullOrEmpty(rawValue) && rawValue[0] == '1';
            }
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

        #endregion
    }
}