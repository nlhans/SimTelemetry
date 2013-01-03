using System;

namespace SimTelemetry.Domain.Events
{
    public class DebugWarningException
    {
        public string Message { get; protected set; }
        public Exception Exception { get; protected set; }
        public string StackTrace { get; protected set; }

        public DebugWarningException(string message, Exception ex)
        {
            Message = message;
            Exception = ex;
        }

        public DebugWarningException(string message, Exception ex, string stackTrace)
            : this(message, ex)
        {
            StackTrace = stackTrace;
        }
    }
}
