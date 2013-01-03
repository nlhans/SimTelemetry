using System;

namespace SimTelemetry.Core.Exceptions
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
