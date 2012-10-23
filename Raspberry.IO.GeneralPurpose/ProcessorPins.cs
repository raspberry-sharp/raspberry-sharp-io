using System;

namespace Raspberry.IO.GeneralPurpose
{
    [Flags]
    public enum ProcessorPins : uint
    {
        None = 0,

        Pin0 = 1 << 0,
        Pin00 = Pin0,
        Pin1 = 1 << 1,
        Pin01 = Pin1,
        Pin2 = 1 << 2,
        Pin02 = Pin2,
        Pin3 = 1 << 3,
        Pin03 = Pin3,
        Pin4 = 1 << 4,
        Pin04 = Pin4,
        Pin7 = 1 << 7,
        Pin07 = Pin7,
        Pin8 = 1 << 8,
        Pin08 = Pin8,
        Pin9 = 1 << 9,
        Pin09 = Pin9,
        Pin10 = 1 << 10,
        Pin11 = 1 << 11,
        Pin14 = 1 << 14,
        Pin15 = 1 << 15,
        Pin17 = 1 << 17,
        Pin18 = 1 << 18,
        Pin21 = 1 << 21,
        Pin22 = 1 << 22,
        Pin23 = 1 << 23,
        Pin24 = 1 << 24,
        Pin25 = 1 << 25,
        Pin27 = 1 << 27,
        Pin28 = 1 << 28,
        Pin29 = 1 << 29,
        Pin30 = 1 << 30,
        Pin31 = (uint)1 << 31
    }
}