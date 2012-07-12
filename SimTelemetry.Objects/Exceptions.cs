using System;

namespace SimTelemetry.Objects
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(string message) : base(message)
        {
        }
    }
}
