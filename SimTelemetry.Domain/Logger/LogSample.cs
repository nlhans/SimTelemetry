using System.Collections.Generic;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Logger
{
    public class LogSample
    {
        public Dictionary<string, IDataNode> Groups { get { return _groups; } }
        private Dictionary<string, IDataNode> _groups = new Dictionary<string, IDataNode>();

        public int Timestamp { get; protected set; }
        public LogSampleProvider Provider { get; protected set; }

        public LogSample(LogSampleProvider provider, int timestamp, IEnumerable<LogGroup> groups)
        {
            Provider = provider;
            Timestamp = timestamp;

            foreach(var group in groups)
                _groups.Add(group.Name, new LogSampleGroup(group));
        }

        public LogSampleGroup Get(string name)
        {
            if (!Groups.ContainsKey(name))
                return null;

            return (LogSampleGroup)Groups[name];
        }

        public void UpdateTime(int timestamp)
        {
            Timestamp = timestamp;
        }
    }
}