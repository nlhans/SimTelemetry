using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.LoggerO
{
    public interface ILogNode 
    {
        int ID { get; }
        IEnumerable<LogGroup> Groups { get; }
        IEnumerable<ILogField> Fields { get; }
        LogGroup CreateGroup(string name);
        LogGroup CreateGroup(string name, int id);
        ILogField CreateField(string name, Type valueType, int id, bool isConstant);
        ILogNode FindGroup(string name);
        ILogNode FindGroup(int id);

        bool ContainsGroup(string name);
        bool ContainsGroup(int id);

        bool ContainsField(string name);
        bool ContainsField(int id);

        T ReadAs<T>(string field);
    }
}