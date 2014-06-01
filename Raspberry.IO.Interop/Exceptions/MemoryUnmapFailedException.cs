using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.Interop
{
    [Serializable]
    public class MemoryUnmapFailedException : Exception {
        public MemoryUnmapFailedException() {}
        public MemoryUnmapFailedException(string message) : base(message) {}
        public MemoryUnmapFailedException(string message, Exception innerException) : base(message, innerException) {}
        protected MemoryUnmapFailedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}