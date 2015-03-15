using System;
using System.Globalization;

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    public class InvalidChecksumException : Exception
    {
        private readonly long expectedValue;
        private readonly long actualValue;

        public InvalidChecksumException(long expectedValue, long actualValue) : base(GetMessage(null, expectedValue, actualValue))
        {
            this.expectedValue = expectedValue;
            this.actualValue = actualValue;
        }

        public InvalidChecksumException(string message, long expectedValue, long actualValue) : base(GetMessage(message, expectedValue, actualValue))
        {
            this.expectedValue = expectedValue;
            this.actualValue = actualValue;
        }

        public InvalidChecksumException(string message, Exception innerException, long expectedValue, long actualValue) : base(GetMessage(message, expectedValue, actualValue), innerException)
        {
            this.expectedValue = expectedValue;
            this.actualValue = actualValue;
        }

        public long ExpectedValue
        {
            get { return expectedValue; }
        }

        public long ActualValue
        {
            get { return actualValue; }
        }

        private static string GetMessage(string message, long expectedValue, long actualValue)
        {
            var valueMessage = string.Format(CultureInfo.InvariantCulture, "Expected {0}, found {1}", expectedValue, actualValue);
            return !string.IsNullOrEmpty(message) 
                ? message + ". " + valueMessage 
                : valueMessage;
        }
    }
}