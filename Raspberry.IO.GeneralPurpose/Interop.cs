#region References

using System;
using System.Runtime.InteropServices;

#endregion

namespace Raspberry.IO.GeneralPurpose
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

        public const uint BCM2835_BLOCK_SIZE = (4 * 1024);

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
        public const uint BCM2835_GPIO_FSEL_OUTP = 1;
        public const uint BCM2835_GPIO_FSEL_ALT0 = 4;
        public const uint BCM2835_GPIO_FSEL_MASK = 7;

        public const uint BCM2835_GPFSEL0 = 0x0000;
        public const uint BCM2835_GPPUD = 0x0094;
        public const uint BCM2835_GPPUDCLK0 = 0x0098;
        public const uint BCM2835_GPSET0 = 0x001c;
        public const uint BCM2835_GPCLR0 = 0x0028;
        public const uint BCM2835_GPLEV0 = 0x0034;

        public const uint BCM2835_GPIO_PUD_OFF = 0;
        public const uint BCM2835_GPIO_PUD_DOWN = 1;
        public const uint BCM2835_GPIO_PUD_UP = 2;

        #endregion

        #endregion

        #region Libc
        
        #region Constants

        public const int EPOLLIN = 1;
        public const int EPOLLPRI = 2;
        public const int EPOLLET = (1 << 31);

        public const int EPOLL_CTL_ADD = 0x1;
        public const int EPOLL_CTL_DEL = 0x2;

        #endregion

        #region Methods

        [DllImport("libc.so.6", EntryPoint = "epoll_create")]
        public static extern int epoll_create(int size);

        [DllImport("libc.so.6", EntryPoint = "epoll_ctl")]
        public static extern int epoll_ctl(int epfd, int op, int fd, IntPtr epevent);

        [DllImport("libc.so.6", EntryPoint = "epoll_wait")]
        public static extern int epoll_wait(int epfd, IntPtr events, int maxevents, int timeout);

        #endregion

        [StructLayout(LayoutKind.Explicit)]
        public struct epoll_data
        {
            [FieldOffset(0)] public IntPtr ptr;
            [FieldOffset(0)] public int fd;
            [FieldOffset(0)] public UInt32 u32;
            [FieldOffset(0)] public UInt64 u64;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct epoll_event
        {
            [FieldOffset(0)] public int events;
            [FieldOffset(4)] public epoll_data data;
        };

        #endregion
    }
}