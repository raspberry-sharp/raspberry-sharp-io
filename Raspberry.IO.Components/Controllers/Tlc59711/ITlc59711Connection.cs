using System;

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// A connection the one or more TLC59711 devices.
    /// </summary>
    public interface ITlc59711Connection : IDisposable
    {
        /// <summary>
        /// A chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711. 
        /// </summary>
        ITlc59711Cluster Devices { get; }

        /// <summary>
        /// Creates a TLC59711 command and sends it to the first device using the SPI bus.
        /// </summary>
        void Update();
    }
}