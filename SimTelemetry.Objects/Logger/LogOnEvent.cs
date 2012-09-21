using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public class LogOnEvent : Attribute
    {
        public List<string> Events = new List<string>();
        
        public LogOnEvent(params string[] events)
        {
            Events = events.OfType<string>().ToList();
        }
    }
}