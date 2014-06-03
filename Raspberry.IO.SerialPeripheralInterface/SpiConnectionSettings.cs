using System;

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// SPI connection parameters
    /// </summary>
    public class SpiConnectionSettings
    {
        #region Fields
        private uint maxSpeed = 5000000;
        private ushort delay;
        private byte bitsPerWord = 8;
        #endregion

        #region Properties
        /// <summary>
        /// Clock speed in Hz
        /// </summary>
        public UInt32 MaxSpeed {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        
        /// <summary>
        /// If nonzero, how long to delay (in µ seconds) after the last bit transfer before optionally deselecting the device before the next transfer
        /// </summary>
        public UInt16 Delay {
            get { return delay; }
            set { delay = value; }
        }

        /// <summary>
        /// The device's word size
        /// </summary>
        public byte BitsPerWord {
            get { return bitsPerWord; }
            set { bitsPerWord = value; }
        }

        /// <summary>
        /// SPI mode
        /// </summary>
        public SpiMode Mode { get; set; }

        #endregion
    }
}