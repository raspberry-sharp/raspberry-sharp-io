using System;
using System.Runtime.InteropServices;

namespace Raspberry.IO.Interop
{
    public static class MemoryMap
    {
        #region Fields
        private static readonly IntPtr FAILED = new IntPtr(-1);
        #endregion

        #region Libc imports
        [DllImport("libc.so.6", EntryPoint = "mmap")]
        private static extern IntPtr mmap(IntPtr address, UIntPtr size, int protect, int flags, int file, UIntPtr offset);

        [DllImport("libc.so.6", EntryPoint = "munmap")]
        private static extern IntPtr munmap(IntPtr address, UIntPtr size);
        #endregion

        #region Methods
        public static IntPtr Create(IntPtr address, ulong size, MemoryProtection protection, MemoryFlags memoryflags, int fileDescriptor, ulong offset) {
            var result = mmap(address, new UIntPtr(size), (int) protection, (int) memoryflags, fileDescriptor, new UIntPtr(offset));
            ThrowOnError<MemoryMapFailedException>(result);
            return result;
        }

        public static IntPtr Create(IntPtr address, uint size, MemoryProtection protection, MemoryFlags memoryflags, int fileDescriptor, uint offset) {
            var result = mmap(address, new UIntPtr(size), (int)protection, (int)memoryflags, fileDescriptor, new UIntPtr(offset));
            ThrowOnError<MemoryMapFailedException>(result);
            return result;
        }
        
        public static void Close(IntPtr address, ulong size) {
            var result = munmap(address, new UIntPtr(size));
            ThrowOnError<MemoryUnmapFailedException>(result);
        }

        public static void Close(IntPtr address, uint size) {
            var result = munmap(address, new UIntPtr(size));
            ThrowOnError<MemoryUnmapFailedException>(result);
        }
        #endregion

        #region Private Helpers
        private static void ThrowOnError<TException>(IntPtr result) 
            where TException: Exception, new() 
        {
            if (result == FAILED) {
                throw new TException();
            }
        }
        #endregion
    }
}