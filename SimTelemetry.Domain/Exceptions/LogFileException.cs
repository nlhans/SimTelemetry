using System;

namespace SimTelemetry.Domain.Exceptions
{
    public class LogFileException : Exception
    {
        public LogFileException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}