#region References

using System;
using System.Runtime.InteropServices;

#endregion

namespace Raspberry.IO.InterIntegratedCircuit
{
    internal static class Interop
    {
        #region BCM2835

        #region Constants

        public const uint BCM2835_PERI_BASE = 0x20000000;
        public const uint BCM2835_GPIO_BASE = (BCM2835_PERI_BASE + 0x200000);
        public const uint BCM2835_BSC0_BASE = (BCM2835_PERI_BASE + 0x205000);
        public const uint BCM2835_BSC1_BASE = (BCM2835_PERI_BASE + 0x804000);
        
        public const uint BCM2836_PERI_BASE = 0x3F000000;
        public const uint BCM2836_GPIO_BASE = (BCM2836_PERI_BASE + 0x200000);
        public const uint BCM2836_BSC0_BASE = (BCM2836_PERI_BASE + 0x205000);
        public const uint BCM2836_BSC1_BASE = (BCM2836_PERI_BASE + 0x804000);

        public const uint BCM2835_BLOCK_SIZE = (4*1024);

        public const uint BCM2835_BSC_C = 0x0000;
        public const uint BCM2835_BSC_S = 0x0004;
        public const uint BCM2835_BSC_DLEN = 0x0008;
        public const uint BCM2835_BSC_A = 0x000c;
        public const uint BCM2835_BSC_FIFO = 0x0010;
        public const uint BCM2835_BSC_DIV = 0x0014;

        public const uint BCM2835_BSC_C_CLEAR_1 = 0x00000020;
        public const uint BCM2835_BSC_C_CLEAR_2 = 0x00000010;
        public const uint BCM2835_BSC_C_I2CEN = 0x00008000;
        public const uint BCM2835_BSC_C_ST = 0x00000080;
        public const uint BCM2835_BSC_C_READ = 0x00000001;

        public const uint BCM2835_BSC_S_CLKT = 0x00000200;
        public const uint BCM2835_BSC_S_ERR = 0x00000100;
        public const uint BCM2835_BSC_S_DONE = 0x00000002;
        public const uint BCM2835_BSC_S_TXD = 0x00000010;
        public const uint BCM2835_BSC_S_RXD = 0x00000020;

        public const uint BCM2835_BSC_FIFO_SIZE = 16;

        public const uint BCM2835_CORE_CLK_HZ = 250000000;

        public const uint BCM2835_GPIO_FSEL_INPT = 0;
        public const uint BCM2835_GPIO_FSEL_ALT0 = 4;
        public const uint BCM2835_GPIO_FSEL_MASK = 7;

        public const uint BCM2835_GPFSEL0 = 0x0000;

        #endregion

        #endregion

        #region Libc

        #region Constants

        public const int O_RDWR = 2;
        public const int O_SYNC = 10000;

        public const int PROT_READ = 1;
        public const int PROT_WRITE = 2;

        public const int MAP_SHARED = 1;
        public const int MAP_FAILED = -1;

        #endregion

        #region Methods

        [DllImport("libc.so.6", EntryPoint = "open")]
        public static extern IntPtr open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint = "close")]
        public static extern void close(IntPtr file);

        [DllImport("libc.so.6", EntryPoint = "mmap")]
        public static extern IntPtr mmap(IntPtr address, uint size, int protect, int flags, IntPtr file, uint offset);

        [DllImport("libc.so.6", EntryPoint = "munmap")]
        public static extern IntPtr munmap(IntPtr address, uint size);

        #endregion

        #endregion
    }
}