using System;

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public class LogProperty : Attribute
    {
        public string Name { get; set; }

        public LogProperty(string name, string description)
        {
            this.Name = name;
        }
    }
}