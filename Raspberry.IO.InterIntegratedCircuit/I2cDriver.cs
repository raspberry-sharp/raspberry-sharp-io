#region References

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.InterIntegratedCircuit
{
    /// <summary>
    /// Represents a driver for I2C devices.
    /// </summary>
    public class I2cDriver : IDisposable
    {
        #region Fields

        private readonly object driverLock = new object();

        private readonly ProcessorPin sdaPin;
        private readonly ProcessorPin sclPin;

        private readonly IntPtr gpioAddress;
        private readonly IntPtr bscAddress;

        private int currentDeviceAddress;
        private int waitInterval;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="I2cDriver"/> class.
        /// </summary>
        /// <param name="sdaPin">The SDA pin.</param>
        /// <param name="sclPin">The SCL pin.</param>
        public I2cDriver(ProcessorPin sdaPin, ProcessorPin sclPin)
        {
            this.sdaPin = sdaPin;
            this.sclPin = sclPin;

            var bscBase = GetBscBase(sdaPin, sclPin);

            var memoryFile = Interop.open("/dev/mem", Interop.O_RDWR + Interop.O_SYNC);
            try
            {
                gpioAddress = Interop.mmap(IntPtr.Zero, Interop.BCM2835_BLOCK_SIZE, Interop.PROT_READ | Interop.PROT_WRITE, Interop.MAP_SHARED, memoryFile, Interop.BCM2835_GPIO_BASE);
                bscAddress = Interop.mmap(IntPtr.Zero, Interop.BCM2835_BLOCK_SIZE, Interop.PROT_READ | Interop.PROT_WRITE, Interop.MAP_SHARED, memoryFile, bscBase);
            }
            finally
            {
                Interop.close(memoryFile);
            }

            if (bscAddress == (IntPtr) Interop.MAP_FAILED)
                throw new InvalidOperationException("Unable to access device memory");

            // Set the I2C pins to the Alt 0 function to enable I2C access on them
            SetPinMode((uint) (int) sdaPin, Interop.BCM2835_GPIO_FSEL_ALT0); // SDA
            SetPinMode((uint) (int) sclPin, Interop.BCM2835_GPIO_FSEL_ALT0); // SCL

            // Read the clock divider register
            var dividerAddress = bscAddress + (int) Interop.BCM2835_BSC_DIV;
            var divider = (ushort) SafeReadUInt32(dividerAddress);
            waitInterval = GetWaitInterval(divider);

            var addressAddress = bscAddress + (int) Interop.BCM2835_BSC_A;
            SafeWriteUInt32(addressAddress, (uint) currentDeviceAddress);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Set all the I2C/BSC1 pins back to input
            SetPinMode((uint) (int) sdaPin, Interop.BCM2835_GPIO_FSEL_INPT); // SDA
            SetPinMode((uint) (int) sclPin, Interop.BCM2835_GPIO_FSEL_INPT); // SCL

            Interop.munmap(gpioAddress, Interop.BCM2835_BLOCK_SIZE);
            Interop.munmap(bscAddress, Interop.BCM2835_BLOCK_SIZE);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the clock divider.
        /// </summary>
        /// <value>
        /// The clock divider.
        /// </value>
        public int ClockDivider
        {
            get
            {
                var dividerAddress = bscAddress + (int) Interop.BCM2835_BSC_DIV;
                return (ushort) SafeReadUInt32(dividerAddress);
            }
            set
            {
                var dividerAddress = bscAddress + (int) Interop.BCM2835_BSC_DIV;
                SafeWriteUInt32(dividerAddress, (uint) value);

                var actualDivider = (ushort) SafeReadUInt32(dividerAddress);
                waitInterval = GetWaitInterval(actualDivider);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Connects the specified device address.
        /// </summary>
        /// <param name="deviceAddress">The device address.</param>
        /// <returns>The device connection</returns>
        public I2cDeviceConnection Connect(int deviceAddress)
        {
            return new I2cDeviceConnection(this, deviceAddress);
        }

        #endregion

        #region Internal Methods

        internal void Write(int deviceAddress, byte[] buffer)
        {
            lock (driverLock)
            {
                EnsureDeviceAddress(deviceAddress);

                var len = (uint) buffer.Length;

                var dlen = bscAddress + (int) Interop.BCM2835_BSC_DLEN;
                var fifo = bscAddress + (int) Interop.BCM2835_BSC_FIFO;
                var status = bscAddress + (int) Interop.BCM2835_BSC_S;
                var control = bscAddress + (int) Interop.BCM2835_BSC_C;

                var remaining = len;
                var i = 0;

                // Clear FIFO
                WriteUInt32Mask(control, Interop.BCM2835_BSC_C_CLEAR_1, Interop.BCM2835_BSC_C_CLEAR_1);

                // Clear Status
                WriteUInt32(status, Interop.BCM2835_BSC_S_CLKT | Interop.BCM2835_BSC_S_ERR | Interop.BCM2835_BSC_S_DONE);

                // Set Data Length
                WriteUInt32(dlen, len);

                while (remaining != 0 && i < Interop.BCM2835_BSC_FIFO_SIZE)
                {
                    WriteUInt32(fifo, buffer[i]);
                    i++;
                    remaining--;
                }

                // Enable device and start transfer
                WriteUInt32(control, Interop.BCM2835_BSC_C_I2CEN | Interop.BCM2835_BSC_C_ST);

                while ((ReadUInt32(status) & Interop.BCM2835_BSC_S_DONE) == 0)
                {
                    while (remaining != 0 && (ReadUInt32(status) & Interop.BCM2835_BSC_S_TXD) != 0)
                    {
                        // Write to FIFO, no barrier
                        WriteUInt32(fifo, buffer[i]);
                        i++;
                        remaining--;
                    }

                    Wait(remaining);
                }

                if ((SafeReadUInt32(status) & Interop.BCM2835_BSC_S_ERR) != 0) // Received a NACK
                    throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_NACK status");
                if ((SafeReadUInt32(status) & Interop.BCM2835_BSC_S_CLKT) != 0) // Received Clock Stretch Timeout
                    throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_CLKT status");
                if (remaining != 0) // Not all data is sent
                    throw new InvalidOperationException(string.Format("Read operation failed with BCM2835_I2C_REASON_ERROR_DATA status, missing {0} bytes", remaining));
                
                WriteUInt32Mask(control, Interop.BCM2835_BSC_S_DONE, Interop.BCM2835_BSC_S_DONE);
            }
        }

        internal byte[] Read(int deviceAddress, int byteCount)
        {
            lock (driverLock)
            {
                EnsureDeviceAddress(deviceAddress);

                var dlen = bscAddress + (int) Interop.BCM2835_BSC_DLEN;
                var fifo = bscAddress + (int) Interop.BCM2835_BSC_FIFO;
                var status = bscAddress + (int) Interop.BCM2835_BSC_S;
                var control = bscAddress + (int) Interop.BCM2835_BSC_C;

                var remaining = (uint) byteCount;
                uint i = 0;

                // Clear FIFO
                WriteUInt32Mask(control, Interop.BCM2835_BSC_C_CLEAR_1, Interop.BCM2835_BSC_C_CLEAR_1);

                // Clear Status
                WriteUInt32(status, Interop.BCM2835_BSC_S_CLKT | Interop.BCM2835_BSC_S_ERR | Interop.BCM2835_BSC_S_DONE);

                // Set Data Length
                WriteUInt32(dlen, (uint) byteCount);

                // Start read
                WriteUInt32(control, Interop.BCM2835_BSC_C_I2CEN | Interop.BCM2835_BSC_C_ST | Interop.BCM2835_BSC_C_READ);

                var buffer = new byte[byteCount];
                while ((ReadUInt32(status) & Interop.BCM2835_BSC_S_DONE) == 0)
                {
                    while ((ReadUInt32(status) & Interop.BCM2835_BSC_S_RXD) != 0)
                    {
                        // Read from FIFO, no barrier
                        buffer[i] = (byte) ReadUInt32(fifo);

                        i++;
                        remaining--;
                    }

                    Wait(remaining);
                }

                while (remaining != 0 && (ReadUInt32(status) & Interop.BCM2835_BSC_S_RXD) != 0)
                {
                    buffer[i] = (byte) ReadUInt32(fifo);
                    i++;
                    remaining--;
                }

                if ((SafeReadUInt32(status) & Interop.BCM2835_BSC_S_ERR) != 0) // Received a NACK
                    throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_NACK status");
                if ((SafeReadUInt32(status) & Interop.BCM2835_BSC_S_CLKT) != 0) // Received Clock Stretch Timeout
                    throw new InvalidOperationException("Read operation failed with BCM2835_I2C_REASON_ERROR_CLKT status");
                if (remaining != 0) // Not all data is received
                    throw new InvalidOperationException(string.Format("Read operation failed with BCM2835_I2C_REASON_ERROR_DATA status, missing {0} bytes", remaining));

                WriteUInt32Mask(control, Interop.BCM2835_BSC_S_DONE, Interop.BCM2835_BSC_S_DONE);

                return buffer;
            }
        }

        #endregion

        #region Private Helpers

        private void EnsureDeviceAddress(int deviceAddress)
        {
            if (deviceAddress != currentDeviceAddress)
            {
                var addressAddress = bscAddress + (int)Interop.BCM2835_BSC_A;
                SafeWriteUInt32(addressAddress, (uint)deviceAddress);

                currentDeviceAddress = deviceAddress;
            }
        }

        private void Wait(uint remaining)
        {
            // When remaining data is to be received, then wait for a fully FIFO
            if (remaining != 0)
                Timer.Sleep(waitInterval * (remaining >= Interop.BCM2835_BSC_FIFO_SIZE ? Interop.BCM2835_BSC_FIFO_SIZE : remaining) / 1000m);
        }

        private static int GetWaitInterval(ushort actualDivider)
        {
            // Calculate time for transmitting one byte
            // 1000000 = micros seconds in a second
            // 9 = Clocks per byte : 8 bits + ACK

            return (int)((decimal)actualDivider * 1000000 * 9 / Interop.BCM2835_CORE_CLK_HZ);
        }

        private static uint GetBscBase(ProcessorPin sdaPin, ProcessorPin sclPin)
        {
            switch (GpioConnectionSettings.BoardConnectorRevision)
            {
                case 1:
                    if (sdaPin == ProcessorPin.Pin0 && sclPin == ProcessorPin.Pin1)
                        return Interop.BCM2835_BSC0_BASE;
                    throw new InvalidOperationException("No I2C device exist on the specified pins");

                case 2:
                    if (sdaPin == ProcessorPin.Pin28 && sclPin == ProcessorPin.Pin29)
                        return Interop.BCM2835_BSC0_BASE;
                    if (sdaPin == ProcessorPin.Pin2 && sclPin == ProcessorPin.Pin3)
                        return Interop.BCM2835_BSC1_BASE;
                    throw new InvalidOperationException("No I2C device exist on the specified pins");

                case 3:
                    if (sdaPin == ProcessorPin.Pin2 && sclPin == ProcessorPin.Pin3)
                        return Interop.BCM2835_BSC1_BASE;
                    throw new InvalidOperationException("No I2C device exist on the specified pins");

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Board revision {0} is not supported", GpioConnectionSettings.BoardConnectorRevision));
            }
        }

        private void SetPinMode(uint pin, uint mode)
        {
            // Function selects are 10 pins per 32 bit word, 3 bits per pin
            var paddr = gpioAddress + (int) (Interop.BCM2835_GPFSEL0 + 4*(pin/10));
            var shift = (pin%10)*3;
            var mask = Interop.BCM2835_GPIO_FSEL_MASK << (int) shift;
            var value = mode << (int) shift;

            WriteUInt32Mask(paddr, value, mask);
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
            unchecked
            {
                var returnValue = (uint) Marshal.ReadInt32(address);
                Marshal.ReadInt32(address);

                return returnValue;
            }
        }
        
        private static uint ReadUInt32(IntPtr address)
        {
            unchecked
            {
                return (uint) Marshal.ReadInt32(address);
            }
        }

        private static void SafeWriteUInt32(IntPtr address, uint value)
        {
            // Make sure we don't rely on the first write, which may get
            // lost if the previous access was to a different peripheral.
            unchecked
            {
                Marshal.WriteInt32(address, (int)value);
                Marshal.WriteInt32(address, (int)value);
            }
        }

        private static void WriteUInt32(IntPtr address, uint value)
        {
            unchecked
            {
                Marshal.WriteInt32(address, (int)value);
            }
        }

        #endregion
    }
}