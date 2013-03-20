using System.Collections.Generic;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Tests.Telemetry
{
    public class MockDataSource : IDataProvider
    {
        public IDataNode Get(string name)
        {
            return new MockDataNode();
        }

        public IEnumerable<IDataNode> GetAll()
        {
            return new List<MockDataNode>();
        }

        public void MarkDirty()
        {
            
        }

        public void Refresh()
        {

        }
    }
}