using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(string message) : base(message)
        {
        }
    }
}
