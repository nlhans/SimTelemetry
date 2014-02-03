using System;
using System.Collections.Generic;
using System.Xml;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Tests.Telemetry
{
    public class MockDataNode : IDataNode
    {
        public Dictionary<string, IDataField> Fields { get { return new Dictionary<string, IDataField>(); }}

        public string Name
        {
            get { return "Mock group"; }
        }

        public T ReadAs<T>(string field)
        {
            return (T) Convert.ChangeType(0, typeof (T));
        }

        public IEnumerable<IDataField> GetDataFields()
        {
            return new List<IDataField>();
        }

        public byte[] ReadBytes(string field)
        {
            return new byte[0];
        }

        public IDataNode Clone(string newName, int newAddress)
        {
            return this;
        }

        public bool Contains(string name)
        {
            return Fields.ContainsKey(name);
        }

        public void GetDebugInfo(XmlWriter file)
        {
            
        }
    }
}