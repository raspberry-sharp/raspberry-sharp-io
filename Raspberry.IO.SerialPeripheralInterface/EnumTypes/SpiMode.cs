using System;

namespace Raspberry.IO.SerialPeripheralInterface
{
    /// <summary>
    /// SPI mode
    /// </summary>
    [Flags]
    public enum SpiMode : uint
    {
        /// <summary>
        /// Clock phase, if set CPHA=1, otherwise CPHA=0.
        /// </summary>
        ClockPhase = Interop.SPI_CPHA,

        /// <summary>
        /// Clock polarity, if set CPOL=1, otherwise CPOL=0.
        /// </summary>
        ClockPolarity = Interop.SPI_CPOL,
        
        /// <summary>
        /// Chip select is a high signal.
        /// </summary>
        ChipSelectActiveHigh = Interop.SPI_CS_HIGH,

        /// <summary>
        /// The least significant bit comes first.
        /// </summary>
        LeastSignificantBitFirst = Interop.SPI_LSB_FIRST,
        
        /// <summary>
        /// Special 3-wire configuration.
        /// </summary>
        ThreeWire = Interop.SPI_3WIRE,
        /// <summary>
        /// Three-wire serial buses
        /// </summary>
        SlaveInOutShared = Interop.SPI_3WIRE,
        /// <summary>
        /// Loopback
        /// </summary>
        Loopback = Interop.SPI_LOOP,
        /// <summary>
        /// Send no chip select signal.
        /// </summary>
        NoChipSelect = Interop.SPI_NO_CS,
        /// <summary>
        /// Slave pulls low to pause.
        /// </summary>
        Ready = Interop.SPI_READY,
        /// <summary>
        /// Slave pulls low to pause.
        /// </summary>
        SlavePullsLowToPause = Interop.SPI_READY,
        
        /// <summary>
        /// CPOL=0, CPHA=0
        /// </summary>
        Mode0 = Interop.SPI_MODE_0,
        /// <summary>
        /// CPOL=0, CPHA=1
        /// </summary>
        Mode1 = Interop.SPI_MODE_1,
        /// <summary>
        /// CPOL =1, CPHA=0
        /// </summary>
        Mode2 = Interop.SPI_MODE_2,
        /// <summary>
        /// CPOL=1, CPHA=1
        /// </summary>
        Mode3 = Interop.SPI_MODE_3
    }
}