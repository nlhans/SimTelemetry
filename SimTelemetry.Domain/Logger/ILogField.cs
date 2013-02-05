using System.Collections.Generic;

namespace SimTelemetry.Domain.Logger
{
    public interface ILogField
    {
        uint ID { get; }
        string Name { get; }

    }
    public interface ILogNode
    {
        IEnumerable<LogGroup> Groups { get; }
        IEnumerable<ILogField> Fields { get; }
        LogGroup CreateGroup(string name);
    }
}