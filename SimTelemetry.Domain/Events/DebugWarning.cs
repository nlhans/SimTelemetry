using System;

namespace SimTelemetry.Domain.Events
{
    public class DebugWarning
    {
        public string Message { get; protected set; }
        public Exception Exception { get; protected set; }
        public string StackTrace { get; protected set; }

        public DebugWarning(string message, Exception ex)
        {
            Message = message;
            Exception = ex;
        }

        public DebugWarning(string message, Exception ex, string stackTrace)
            : this(message, ex)
        {
            StackTrace = stackTrace;
        }
    }
}
