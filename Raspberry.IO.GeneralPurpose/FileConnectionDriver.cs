#region References

using System.IO;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connection driver using files.
    /// </summary>
    public class FileConnectionDriver : IConnectionDriver
    {
        #region Fields

        private const string gpioPath = "/sys/class/gpio";

        #endregion

        #region Methods

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
        /// Allocates the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="direction">The direction.</param>
        public void Allocate(ProcessorPin pin, PinDirection direction)
        {
            var gpioId = string.Format("gpio{0}", (int) pin);
            if (Directory.Exists(Path.Combine(gpioPath, gpioId)))
                Release(pin);

            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "export"), false))
                streamWriter.Write((int) pin);

            var filePath = Path.Combine(gpioId, "direction");
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, filePath), false))
                streamWriter.Write(direction == PinDirection.Input ? "in" : "out");
        }
        
        public void Release(ProcessorPin pin)
        {
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "unexport"), false))
                streamWriter.Write((int) pin);
        }

        #endregion
    }
}