#region References

using System;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.Components.Sensors.Pressure.Bmp085
{
    /// <summary>
    /// Represents an I2C connection to a BMP085 barometer / thermometer.
    /// </summary>
    public class Bmp085I2cConnection
    {
        #region Fields

        private readonly I2cDeviceConnection connection;
        private Bmp085Precision precision = Bmp085Precision.Standard;

        private short ac1;
        private short ac2;
        private short ac3;
        private ushort ac4;
        private ushort ac5;
        private ushort ac6;

        private short b1;
        private short b2;
        private short mb;
        private short mc;
        private short md;

        private static readonly TimeSpan lowDelay = TimeSpan.FromMilliseconds(5);
        private static readonly TimeSpan highDelay = TimeSpan.FromMilliseconds(14);
        private static readonly TimeSpan highestDelay = TimeSpan.FromMilliseconds(26);
        private static readonly TimeSpan defaultDelay = TimeSpan.FromMilliseconds(8);

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Bmp085I2cConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Bmp085I2cConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;
            Initialize();
        }

        #endregion

        #region Properties

        public const int DefaultAddress = 0x77;

        public Bmp085Precision Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the pressure.
        /// </summary>
        /// <returns>The pressure.</returns>
        public UnitsNet.Pressure GetPressure()
        {
            return GetData().Pressure;
        }

        /// <summary>
        /// Gets the temperature.
        /// </summary>
        /// <returns>The temperature.</returns>
        public UnitsNet.Temperature GetTemperature()
        {
            // Do not use GetData here since it would imply useless I/O and computation.
            var rawTemperature = GetRawTemperature();
            var b5 = ComputeB5(rawTemperature);

            return UnitsNet.Temperature.FromDegreesCelsius((double)((b5 + 8) >> 4) / 10);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        public Bmp085Data GetData()
        {
            var rawTemperature = GetRawTemperature();
            var rawPressure = GetRawPressure();

            var b5 = ComputeB5(rawTemperature);

            // do pressure calcs
            var b6 = b5 - 4000;
            var x1 = (b2*((b6*b6) >> 12)) >> 11;
            var x2 = (ac2*b6) >> 11;
            var x3 = x1 + x2;
            var b3 = (((ac1*4 + x3) << (int) precision) + 2)/4;

            x1 = (ac3*b6) >> 13;
            x2 = (b1*((b6*b6) >> 12)) >> 16;
            x3 = ((x1 + x2) + 2) >> 2;
            var b4 = (ac4*(uint) (x3 + 32768)) >> 15;
            var b7 = (uint) ((rawPressure - b3)*(uint) (50000UL >> (int) precision));

            int p;
            if (b4 == 0)
                p = int.MaxValue;
            else if (b7 < 0x80000000)
                p = (int) ((b7*2)/b4);
            else
                p = (int) ((b7/b4)*2);

            x1 = (p >> 8)*(p >> 8);
            x1 = (x1*3038) >> 16;
            x2 = (-7357*p) >> 16;

            return new Bmp085Data
            {
                Pressure = UnitsNet.Pressure.FromPascals(p + ((x1 + x2 + 3791) >> 4)),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius((double)((b5 + 8) >> 4) / 10)
            };
        }

        #endregion

        #region Private Helpers

        private static class Interop
        {
            public const byte CAL_AC1 = 0xAA; // R   Calibration data (16 bits)
            public const byte CAL_AC2 = 0xAC; // R   Calibration data (16 bits)
            public const byte CAL_AC3 = 0xAE; // R   Calibration data (16 bits)    
            public const byte CAL_AC4 = 0xB0; // R   Calibration data (16 bits)
            public const byte CAL_AC5 = 0xB2; // R   Calibration data (16 bits)
            public const byte CAL_AC6 = 0xB4; // R   Calibration data (16 bits)
            public const byte CAL_B1 = 0xB6;  // R   Calibration data (16 bits)
            public const byte CAL_B2 = 0xB8;  // R   Calibration data (16 bits)
            public const byte CAL_MB = 0xBA;  // R   Calibration data (16 bits)
            public const byte CAL_MC = 0xBC;  // R   Calibration data (16 bits)
            public const byte CAL_MD = 0xBE;  // R   Calibration data (16 bits)

            public const byte CONTROL = 0xF4;
            public const byte TEMPDATA = 0xF6;
            public const byte PRESSUREDATA = 0xF6;
            public const byte READTEMPCMD = 0x2E;
            public const byte READPRESSURECMD = 0x34;
        }

        private void Initialize()
        {
            if (precision > Bmp085Precision.Highest)
                precision = Bmp085Precision.Highest;

            if (ReadByte(0xD0) != 0x55)
                throw new InvalidOperationException("Device is not a BMP085 barometer");

            /* read calibration data */
            ac1 = ReadInt16(Interop.CAL_AC1);
            ac2 = ReadInt16(Interop.CAL_AC2);
            ac3 = ReadInt16(Interop.CAL_AC3);
            ac4 = ReadUInt16(Interop.CAL_AC4);
            ac5 = ReadUInt16(Interop.CAL_AC5);
            ac6 = ReadUInt16(Interop.CAL_AC6);
            
            b1 = ReadInt16(Interop.CAL_B1);
            b2 = ReadInt16(Interop.CAL_B2);
            
            mb = ReadInt16(Interop.CAL_MB);
            mc = ReadInt16(Interop.CAL_MC);
            md = ReadInt16(Interop.CAL_MD);
        }

        private ushort GetRawTemperature()
        {
            WriteByte(Interop.CONTROL, Interop.READTEMPCMD);
            HighResolutionTimer.Sleep(lowDelay);

            return ReadUInt16(Interop.TEMPDATA);
        }

        private uint GetRawPressure()
        {
            WriteByte(Interop.CONTROL, (byte)(Interop.READPRESSURECMD + ((int)precision << 6)));

            switch (precision)
            {
                case Bmp085Precision.Low:
                    HighResolutionTimer.Sleep(lowDelay);
                    break;

                case Bmp085Precision.High:
                    HighResolutionTimer.Sleep(highDelay);
                    break;

                case Bmp085Precision.Highest:
                    HighResolutionTimer.Sleep(highestDelay);
                    break;

                default:
                    HighResolutionTimer.Sleep(defaultDelay);
                    break;
            }

            var msb = ReadByte(Interop.PRESSUREDATA);
            var lsb = ReadByte(Interop.PRESSUREDATA + 1);
            var xlsb = ReadByte(Interop.PRESSUREDATA + 2);
            var raw = ((msb << 16) + (lsb << 8) + xlsb) >> (8 - (int) precision);
            
            return (uint)raw;
        }

        private int ComputeB5(int ut)
        {
            var x1 = (ut - ac6)*ac5 >> 15;

            if (x1 + md == 0) 
                return int.MaxValue;
            
            var x2 = (mc << 11)/(x1 + md);
            return x1 + x2;
        }

        private byte ReadByte(byte address)
        {
            return ReadBytes(address, 1)[0];
        }

        private byte[] ReadBytes(byte address, int byteCount)
        {
            connection.WriteByte(address);
            return connection.Read(byteCount);
        }

        private ushort ReadUInt16(byte address)
        {
            var bytes = ReadBytes(address, 2);
            unchecked
            {
                return (ushort)((bytes[0] << 8) + bytes[1]);
            }
        }

        private short ReadInt16(byte address)
        {
            var bytes = ReadBytes(address, 2);
            unchecked
            {
                return (short)((bytes[0] << 8) + bytes[1]);
            }
        }

        private void WriteByte(byte address, byte data)
        {
            connection.Write(address, data);
        }

        #endregion
    }
}
