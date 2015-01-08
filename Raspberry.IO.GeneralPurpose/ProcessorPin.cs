using System;

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a processor pin.
    /// </summary>
    public enum ProcessorPin
    {
        /// <summary>
        /// Pin 0.
        /// </summary>
        [GpioName("gpio0")]
        Pin0 = 0,

        /// <summary>
        /// Pin 0.
        /// </summary>
        Pin00 = Pin0,

        /// <summary>
        /// Pin 1.
        /// </summary>
        [GpioName("gpio1")]
        Pin1 = 1,

        /// <summary>
        /// Pin 1.
        /// </summary>
        Pin01 = Pin1,

        /// <summary>
        /// Pin 2.
        /// </summary>
        [GpioName("gpio2")]
        Pin2 = 2,

        /// <summary>
        /// Pin 2.
        /// </summary>
        Pin02 = Pin2,

        /// <summary>
        /// Pin 3.
        /// </summary>
        [GpioName("gpio3")]
        Pin3 = 3,

        /// <summary>
        /// Pin 3.
        /// </summary>
        Pin03 = Pin3,

        /// <summary>
        /// Pin 4.
        /// </summary>
        [GpioName("gpio4")]
        Pin4 = 4,

        /// <summary>
        /// Pin 4.
        /// </summary>
        Pin04 = Pin4,

        /// <summary>
        /// Pin 5.
        /// </summary>
        [GpioName("gpio5")]
        Pin5 = 5,

        /// <summary>
        /// Pin 5.
        /// </summary>
        Pin05 = Pin5,

        /// <summary>
        /// Pin 6.
        /// </summary>
        [GpioName("gpio6")]
        Pin6 = 6,

        /// <summary>
        /// Pin 6.
        /// </summary>
        Pin06 = Pin6,

        /// <summary>
        /// Pin 7.
        /// </summary>
        [GpioName("gpio7")]
        Pin7 = 7,

        /// <summary>
        /// Pin 7.
        /// </summary>
        Pin07 = Pin7,

        /// <summary>
        /// Pin 8.
        /// </summary>
        [GpioName("gpio8")]
        Pin8 = 8,

        /// <summary>
        /// Pin 8.
        /// </summary>
        Pin08 = Pin8,

        // <summary>
        /// Pin 9.
        /// </summary>
        [GpioName("gpio9")]
        Pin9 = 9,

        /// <summary>
        /// Pin 9.
        /// </summary>
        Pin09 = Pin9,

        /// <summary>
        /// Pin 10.
        /// </summary>
        [GpioName("gpio10")]
        Pin10 = 10,

        /// <summary>
        /// Pin 11.
        /// </summary>
        [GpioName("gpio11")]
        Pin11 = 11,

        /// <summary>
        /// Pin 12.
        /// </summary>
        [GpioName("gpio12")]
        Pin12 = 12,

        /// <summary>
        /// Pin 13.
        /// </summary>
        [GpioName("gpio13")]
        Pin13 = 13,

        /// <summary>
        /// Pin 14.
        /// </summary>
        [GpioName("gpio14")]
        Pin14 = 14,

        /// <summary>
        /// Pin 15.
        /// </summary>
        [GpioName("gpio15")]
        Pin15 = 15,

        /// <summary>
        /// Pin 16.
        /// </summary>
        [GpioName("gpio16")]
        Pin16 = 16,

        /// <summary>
        /// Pin 17.
        /// </summary>
        [GpioName("gpio17")]
        Pin17 = 17,

        /// <summary>
        /// Pin 18.
        /// </summary>
        [GpioName("gpio18")]
        Pin18 = 18,

        /// <summary>
        /// Pin 19.
        /// </summary>
        [GpioName("gpio19")]
        Pin19 = 19,

        /// <summary>
        /// Pin 20.
        /// </summary>
        [GpioName("gpio20")]
        Pin20 = 20,

        /// <summary>
        /// Pin 21.
        /// </summary>
        [GpioName("gpio21")]
        Pin21 = 21,

        /// <summary>
        /// Pin 22.
        /// </summary>
        [GpioName("gpio22")]
        Pin22 = 22,

        /// <summary>
        /// Pin 23.
        /// </summary>
        [GpioName("gpio23")]
        Pin23 = 23,

        /// <summary>
        /// Pin 24.
        /// </summary>
        [GpioName("gpio24")]
        Pin24 = 24,

        /// <summary>
        /// Pin 25.
        /// </summary>
        [GpioName("gpio25")]
        Pin25 = 25,

        /// <summary>
        /// Pin 26.
        /// </summary>
        [GpioName("gpio26")]
        Pin26 = 26,

        /// <summary>
        /// Pin 27.
        /// </summary>
        [GpioName("gpio27")]
        Pin27 = 27,

        /// <summary>
        /// Pin 28.
        /// </summary>
        [GpioName("gpio28")]
        Pin28 = 28,

        /// <summary>
        /// Pin 29.
        /// </summary>
        [GpioName("gpio29")]
        Pin29 = 29,

        /// <summary>
        /// Pin 30.
        /// </summary>
        [GpioName("gpio30")]
        Pin30 = 30,

        /// <summary>
        /// Pin 31.
        /// </summary>
        [GpioName("gpio31")]
        Pin31 = 31
    }

    #region Custom Attribute classes

    class GpioName : Attribute
    {
        public string Name;

        public GpioName(string name)
        {
            Name = name;
        }
    }

    #endregion

}