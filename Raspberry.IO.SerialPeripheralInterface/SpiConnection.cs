#region References

using System;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.SerialPeripheralInterface
{
    public class SpiConnection : IDisposable
    {
        #region Fields

        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin clock;
        private readonly ProcessorPin ss;
        private readonly ProcessorPin? miso;
        private readonly ProcessorPin? mosi;

        private readonly Endianness endianness = Endianness.LittleEndian;

        #endregion

        #region Instance Management

        public SpiConnection(ProcessorPin clock, ProcessorPin ss, ProcessorPin? miso, ProcessorPin? mosi, Endianness endianness)
        {
            this.clock = clock;
            this.ss = ss;
            this.miso = miso;
            this.mosi = mosi;
            this.endianness = endianness;

            driver = GpioConnectionSettings.DefaultDriver;

            driver.Allocate(clock, PinDirection.Output);
            driver.Write(clock, false);

            driver.Allocate(ss, PinDirection.Output);
            driver.Write(ss, true);

            if (mosi.HasValue)
            {
                driver.Allocate(mosi.Value, PinDirection.Output);
                driver.Write(mosi.Value, false);
            }

            if (miso.HasValue)
                driver.Allocate(miso.Value, PinDirection.Input);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Methods

        public void Close()
        {
            driver.Release(clock);
            driver.Release(ss);
            if (mosi.HasValue)
                driver.Release(mosi.Value);
            if (miso.HasValue)
                driver.Release(miso.Value);
        }

        public SpiSlaveSelection SelectSlave()
        {
            driver.Write(ss, false);
            return new SpiSlaveSelection(this);
        }
        
        internal void DeselectSlave()
        {
            driver.Write(ss, true);
        }

        public void Synchronize()
        {
            driver.Write(clock, true);
            Timer.Sleep(1);
            driver.Write(clock, false);
        }

        public void Write(bool data)
        {
            if (!mosi.HasValue)
                throw new NotSupportedException("No MOSI pin has been provided");

            driver.Write(mosi.Value, data);
            Synchronize();
        }

        public void Write(byte data, int bitCount)
        {
            if (bitCount > 8)
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "byte data cannot contain more than 8 bits");

            SafeWrite(data, bitCount);
        }

        public void Write(ushort data, int bitCount)
        {
            if (bitCount > 16)
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "ushort data cannot contain more than 16 bits");

            SafeWrite(data, bitCount);
        }

        public void Write(uint data, int bitCount)
        {
            if (bitCount > 32)
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "uint data cannot contain more than 32 bits");

            SafeWrite(data, bitCount);
        }

        public void Write(ulong data, int bitCount)
        {
            if (bitCount > 64)
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "ulong data cannot contain more than 64 bits");

            SafeWrite(data, bitCount);
        }

        public bool Read()
        {
            if (!miso.HasValue)
                throw new NotSupportedException("No MISO pin has been provided");

            Synchronize();
            return driver.Read(miso.Value);
        }

        public ulong Read(int bitCount)
        {
            if (bitCount > 64)
                throw new ArgumentOutOfRangeException("bitCount", bitCount, "ulong data cannot contain more than 64 bits");

            ulong data = 0;
            for (var i = 0; i < bitCount; i++)
            {
                var index = endianness == Endianness.BigEndian
                                ? i
                                : bitCount - 1 - i;

                var bit = Read();
                if (bit)
                    data |= ((ulong)1 << index);
            }

            return data;
        }

        #endregion

        #region Private Helpers

        private void SafeWrite(ulong data, int bitCount)
        {
            for (var i = 0; i < bitCount; i++)
            {
                var index = endianness == Endianness.BigEndian
                                ? i
                                : bitCount - 1 - i;

                var bit = data & ((ulong) 1 << index);
                Write(bit != 0);
            }
        }

        #endregion
    }
}