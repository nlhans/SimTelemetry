using System;

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class Unloggable : Attribute
    {
        public Unloggable()
        {
        }
    }
}