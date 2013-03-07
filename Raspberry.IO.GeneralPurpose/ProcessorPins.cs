using System;

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a set of pins on the Raspberry Pi Processor
    /// </summary>
    [Flags]
    public enum ProcessorPins : uint
    {
        /// <summary>
        /// No pins selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Pin 0 selected.
        /// </summary>
        Pin0 = 1 << 0,

        /// <summary>
        /// Pin 0 selected.
        /// </summary>
        Pin00 = Pin0,

        /// <summary>
        /// Pin 1 selected.
        /// </summary>
        Pin1 = 1 << 1,

        /// <summary>
        /// Pin 1 selected.
        /// </summary>
        Pin01 = Pin1,

        /// <summary>
        /// Pin 2 selected.
        /// </summary>
        Pin2 = 1 << 2,

        /// <summary>
        /// Pin 2 selected.
        /// </summary>
        Pin02 = Pin2,

        /// <summary>
        /// Pin 3 selected.
        /// </summary>
        Pin3 = 1 << 3,

        /// <summary>
        /// Pin 3 selected.
        /// </summary>
        Pin03 = Pin3,

        /// <summary>
        /// Pin 4 selected.
        /// </summary>
        Pin4 = 1 << 4,

        /// <summary>
        /// Pin 4 selected.
        /// </summary>
        Pin04 = Pin4,

        /// <summary>
        /// Pin 7 selected.
        /// </summary>
        Pin7 = 1 << 7,

        /// <summary>
        /// Pin 7 selected.
        /// </summary>
        Pin07 = Pin7,

        /// <summary>
        /// Pin 8 selected.
        /// </summary>
        Pin8 = 1 << 8,

        /// <summary>
        /// Pin 8 selected.
        /// </summary>
        Pin08 = Pin8,

        /// <summary>
        /// Pin 9 selected.
        /// </summary>
        Pin9 = 1 << 9,

        /// <summary>
        /// Pin 9 selected.
        /// </summary>
        Pin09 = Pin9,

        /// <summary>
        /// Pin 10 selected.
        /// </summary>
        Pin10 = 1 << 10,

        /// <summary>
        /// Pin 11 selected.
        /// </summary>
        Pin11 = 1 << 11,

        /// <summary>
        /// Pin 14 selected.
        /// </summary>
        Pin14 = 1 << 14,

        /// <summary>
        /// Pin 15 selected.
        /// </summary>
        Pin15 = 1 << 15,

        /// <summary>
        /// Pin 17 selected.
        /// </summary>
        Pin17 = 1 << 17,

        /// <summary>
        /// Pin 18 selected.
        /// </summary>
        Pin18 = 1 << 18,

        /// <summary>
        /// Pin 21 selected.
        /// </summary>
        Pin21 = 1 << 21,

        /// <summary>
        /// Pin 22 selected.
        /// </summary>
        Pin22 = 1 << 22,

        /// <summary>
        /// Pin 23 selected.
        /// </summary>
        Pin23 = 1 << 23,

        /// <summary>
        /// Pin 24 selected.
        /// </summary>
        Pin24 = 1 << 24,

        /// <summary>
        /// Pin 25 selected.
        /// </summary>
        Pin25 = 1 << 25,

        /// <summary>
        /// Pin 27 selected.
        /// </summary>
        Pin27 = 1 << 27,

        /// <summary>
        /// Pin 28 selected.
        /// </summary>
        Pin28 = 1 << 28,

        /// <summary>
        /// Pin 29 selected.
        /// </summary>
        Pin29 = 1 << 29,

        /// <summary>
        /// Pin 30 selected.
        /// </summary>
        Pin30 = 1 << 30,

        /// <summary>
        /// Pin 31 selected.
        /// </summary>
        Pin31 = (uint)1 << 31
    }
}