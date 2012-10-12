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
        /// Exports the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Export(PinConfiguration pin)
        {
            var gpioId = string.Format("gpio{0}", (int) pin.Pin);
            if (Directory.Exists(Path.Combine(gpioPath, gpioId)))
                Unexport(pin);

            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "export"), false))
                streamWriter.Write((int) pin.Pin);

            var filePath = Path.Combine(gpioId, "direction");
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, filePath), false))
                streamWriter.Write(pin.Direction == PinDirection.Input ? "in" : "out");
        }

        /// <summary>
        /// Unexports the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Unexport(PinConfiguration pin)
        {
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "unexport"), false))
                streamWriter.Write((int) pin.Pin);
        }

        #endregion
    }
}