using System;

namespace SimTelemetry.Core
{
    public class GlobalEventDelegate
    {
        public Delegate Action { get; set; }
        public bool Network { get; set; }
    }
}
