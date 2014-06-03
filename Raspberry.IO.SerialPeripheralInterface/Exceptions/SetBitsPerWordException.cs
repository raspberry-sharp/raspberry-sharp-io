using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.SerialPeripheralInterface
{
#pragma warning disable 1591
    [Serializable]
    public class SetBitsPerWordException : Exception {
        public SetBitsPerWordException() {}
        public SetBitsPerWordException(string message) : base(message) {}
        public SetBitsPerWordException(string message, Exception innerException) : base(message, innerException) {}
        protected SetBitsPerWordException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
#pragma warning restore 1591
}