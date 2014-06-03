using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.SerialPeripheralInterface
{
#pragma warning disable 1591
    [Serializable]
    public class SendSpiMessageException : Exception {
        public SendSpiMessageException() {}
        public SendSpiMessageException(string message) : base(message) {}
        public SendSpiMessageException(string message, Exception innerException) : base(message, innerException) {}
        protected SendSpiMessageException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
#pragma warning restore 1591
}