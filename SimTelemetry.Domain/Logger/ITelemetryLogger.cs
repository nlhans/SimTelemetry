using System.Collections.Generic;
using SimTelemetry.Domain.Memory;


namespace SimTelemetry.Domain.Logger
{
    public interface ITelemetryLogger
    {
        ITelemetryLogConfiguration Configuration { get; }

        void Update();
        void Initialize(Aggregates.Telemetry telemetry, MemoryProvider memory);
        void UpdateConfiguration(ITelemetryLogConfiguration configuration);
        void Deinitialize();
    }

    public interface ITelemetryLogConfiguration
    {
        IEnumerable<int> DriversLogSelective { get; }
        bool DriversLogAll { get; }
        bool DriversLogAI { get; }
        bool RecordTimePathsAll { get; }
        bool RecordTimePathsAI { get; }
    }
    
    public class TelemetryLogConfiguration : ITelemetryLogConfiguration
    {
        public TelemetryLogConfiguration(bool driversLogAll, bool driversLogAi, bool recordTimePathsAll, bool recordTimePathsAi)
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

        private readonly IList<int> _driversLogSelective = new List<int>();
        public IEnumerable<int> DriversLogSelective { get { return _driversLogSelective; } }
        public bool DriversLogAll { get; private set; }
        public bool DriversLogAI { get; private set; }
        public bool RecordTimePathsAll { get; private set; }
        public bool RecordTimePathsAI { get; private set; }
    }
}