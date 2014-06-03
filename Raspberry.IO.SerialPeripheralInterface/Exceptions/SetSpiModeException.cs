using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.SerialPeripheralInterface
{
#pragma warning disable 1591
    [Serializable]
    public class SetSpiModeException : Exception {
        public SetSpiModeException() {}
        public SetSpiModeException(string message) : base(message) {}
        public SetSpiModeException(string message, Exception innerException) : base(message, innerException) {}
        protected SetSpiModeException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
#pragma warning restore 1591
}