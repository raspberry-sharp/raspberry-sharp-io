using System.IO;

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a handle on a GPIO file
    /// </summary>
    public class FileGpioHandle
    {
        /// <summary>
        /// Gets or sets the gpio path.
        /// </summary>
        /// <value>
        /// The gpio path.
        /// </value>
        public string GpioPath { get; set; }

        /// <summary>
        /// Gets or sets the gpio stream.
        /// </summary>
        /// <value>
        /// The gpio stream.
        /// </value>
        public Stream GpioStream { get; set; }
    }
}