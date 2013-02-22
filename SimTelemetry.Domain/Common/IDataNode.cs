using System.Collections.Generic;
using System.Xml;

namespace SimTelemetry.Domain.Common
{
    public interface IDataNode
    {
        string Name { get; }
        T ReadAs<T>(string field);
        IEnumerable<IDataField> GetDataFields();
        byte[] ReadBytes(string field);
        void GetDebugInfo(XmlWriter file);
    }
}
