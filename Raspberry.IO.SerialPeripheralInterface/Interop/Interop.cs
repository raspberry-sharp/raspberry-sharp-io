using System;
using System.Runtime.InteropServices;

namespace Raspberry.IO.SerialPeripheralInterface
{
    // ReSharper disable InconsistentNaming
    internal static class Interop
    {
        public const UInt32 SPI_CPHA = 0x01;
        public const UInt32 SPI_CPOL = 0x02;

        public const UInt32 SPI_MODE_0 = (0 | 0);
        public const UInt32 SPI_MODE_1 = (0 | SPI_CPHA);
        public const UInt32 SPI_MODE_2 = (SPI_CPOL | 0);
        public const UInt32 SPI_MODE_3 = (SPI_CPOL | SPI_CPHA);

        public const UInt32 SPI_CS_HIGH = 0x04;
        public const UInt32 SPI_LSB_FIRST = 0x08;
        public const UInt32 SPI_3WIRE = 0x10;
        
        public const UInt32 SPI_LOOP = 0x20;
        public const UInt32 SPI_NO_CS = 0x40;
        public const UInt32 SPI_READY = 0x80;

        public const UInt32 SPI_IOC_MESSAGE_BASE = 0x40006b00;
        public const int SPI_IOC_MESSAGE_NUMBER_SHIFT = 16;

        private static readonly int transferMessageSize = Marshal.SizeOf(typeof(SpiTransferControlStructure));

        internal static UInt32 GetSpiMessageRequest(int numberOfMessages) {
            var size = unchecked((UInt32)(transferMessageSize * numberOfMessages));
            return SPI_IOC_MESSAGE_BASE | (size << SPI_IOC_MESSAGE_NUMBER_SHIFT);
        }
    }

    // ReSharper restore InconsistentNaming
}