using System.Collections.Generic;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Aggregates
{
    public class TelemetryLog : IEntity<int>
    {
        public int ID { get; private set; }
        public string File { get; private set; }
        public List<string> Drivers { get; private set; }
        public IEnumerable<Lap> Laps { get { return _laps; } }
        private readonly List<Lap> _laps = new List<Lap>();

        public TelemetryLog(int id, string file)
        {
            ID = id;
            File = file;
            Drivers = new List<string>();
        }

        public void AddDriver(string name)
        {
            Drivers.Add(name);
        }

        public void AddLap(Lap l)
        {
            if (_laps.Contains(l) == false)
            {
                _laps.Add(l);
            }
        }

        public bool Equals(int other)
        {
            return ID == other;
        }
    }

}