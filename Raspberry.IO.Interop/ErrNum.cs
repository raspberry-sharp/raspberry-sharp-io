using System;
using System.Runtime.InteropServices;

namespace Raspberry.IO.Interop
{
    public static class ErrNum
    {
        #region libc imports
        [DllImport("libc", EntryPoint = "strerror", SetLastError = true)]
        private static extern IntPtr strerror(int errnum);
        #endregion

        #region Methods
        public static void ThrowOnPInvokeError<TException>(this int result, string message = null)
            where TException : Exception, new() 
        {
            if (result >= 0) {
                return;
            }

            var type = typeof(TException);
            var constructorInfo = type.GetConstructor(new[] { typeof(string) });
            if (ReferenceEquals(constructorInfo, null)) {
                throw new TException();
            }
            
            var err = Marshal.GetLastWin32Error();
            var messagePtr = strerror(err);

            var strErrorMessage = (messagePtr != IntPtr.Zero)
                ? Marshal.PtrToStringAuto(messagePtr)
                : "unknown";

            var exceptionMessage = (message == null)
                ? string.Format("Error {0}: {1}", err, strErrorMessage)
                : string.Format(message, result, err, strErrorMessage);
            
            throw (TException)constructorInfo.Invoke(new object[] { exceptionMessage });
        }
        #endregion
    }
}