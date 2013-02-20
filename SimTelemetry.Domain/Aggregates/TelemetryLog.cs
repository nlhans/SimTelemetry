using System.Collections.Generic;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Aggregates
{
    public class TelemetryLog
    {
        public IEnumerable<RecordedLap> Laps { get { return _laps; } }
        private readonly List<RecordedLap> _laps = new List<RecordedLap>();

        protected LogFile _log;

        public TelemetryLog(string file)
        {
            _log = new LogFile(file);

            RecordedLap ind = new RecordedLap(1, file, new Lap(1, null, 15, 25, 35), 5, 80 );
            _laps.Add(ind);
        }

        public IEnumerable<LogSample> GetSamples(RecordedLap lap, int prefix, int suffix)
        {
            int startTime = lap.SessionTimeStart - prefix;
            int stopTime = lap.SessionTimeEnd + suffix;

            LogSample start = new LogSample(_log, startTime);

            yield return start;

            int timestamp = start.Timestamp;
            var dataOffsetStart = _log.Time;
            //while ()
        }
    }
}