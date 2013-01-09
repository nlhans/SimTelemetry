using System;

namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginBase : IEquatable<IPluginBase>
    {
        int ID { get; }
        string Name { get; }
        string Version { get; }
        string Description { get; }
        string Author { get; }
        DateTime CompilationTime { get; }
    }
}