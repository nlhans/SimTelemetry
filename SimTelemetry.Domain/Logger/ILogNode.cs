using System;
using System.Collections.Generic;

namespace SimTelemetry.Domain.Logger
{
    public interface ILogNode
    {
        uint ID { get; }
        IEnumerable<LogGroup> Groups { get; }
        IEnumerable<ILogField> Fields { get; }
        LogGroup CreateGroup(string name);
        LogGroup CreateGroup(string name, uint id);
        ILogField CreateField(string name, Type valueType, uint id);
    }
}