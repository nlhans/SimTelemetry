using System.Collections.Generic;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryLoggerConfiguration
    {
        private readonly IList<int> _driversLogSelective = new List<int>();
        public IEnumerable<int> DriversLogSelective { get { return _driversLogSelective; } }
        public bool DriversLogAll { get; private set; }
        public bool DriversLogAI { get; private set; }
        public bool RecordTimePathsAll { get; private set; }
        public bool RecordTimePathsAI { get; private set; }

        public TelemetryLoggerConfiguration(bool driversLogAll, bool driversLogAi, bool recordTimePathsAll, bool recordTimePathsAi)
        {
            DriversLogAll = driversLogAll;
            DriversLogAI = driversLogAi;
            RecordTimePathsAll = recordTimePathsAll;
            RecordTimePathsAI = recordTimePathsAi;
        }

        public void Add(int index)
        {
            if (_driversLogSelective.Contains(index) == false)
                _driversLogSelective.Add(index);

        }

        public void Remove(int index)
        {
            if (_driversLogSelective.Contains(index))
                _driversLogSelective.Remove(index);
        }
    }
}