using System;

namespace SimTelemetry.Domain
{
    public class GlobalEventDelegate
    {
        public Delegate Action { get; set; }
        public bool Network { get; set; }
    }
}
