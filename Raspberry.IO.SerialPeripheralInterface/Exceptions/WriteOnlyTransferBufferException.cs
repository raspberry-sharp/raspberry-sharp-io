using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.SerialPeripheralInterface
{
#pragma warning disable 1591
    [Serializable]
    public class WriteOnlyTransferBufferException : Exception
    {
        public WriteOnlyTransferBufferException() {}
        public WriteOnlyTransferBufferException(string message) : base(message) {}
        public WriteOnlyTransferBufferException(string message, Exception innerException) : base(message, innerException) {}
        protected WriteOnlyTransferBufferException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
#pragma warning restore 1591
}