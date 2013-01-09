using System;

namespace SimTelemetry.Domain.Exceptions
{
    public class PluginHostException : Exception
    {
        public PluginHostException(string message) : base(message)
        {
            
        }

        public PluginHostException(string message, Exception innerException) : base (message, innerException)
        {
            
        }
    }
}
