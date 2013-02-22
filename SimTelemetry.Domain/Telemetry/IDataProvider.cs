using System.Collections;
using System.Collections.Generic;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Telemetry
{
    public interface IDataProvider
    {
        IDataNode Get(string name);
        IEnumerable<IDataNode> GetAll();

        void MarkDirty();
    }
}