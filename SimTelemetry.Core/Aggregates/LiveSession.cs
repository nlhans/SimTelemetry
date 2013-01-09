using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Core.Entities;
using SimTelemetry.Core.Exceptions;
using SimTelemetry.Core.ValueObjects;

namespace SimTelemetry.Core.Aggregates
{
    public class LiveSession
    {
        private IList<LiveScoringDriver> _scoring = new List<LiveScoringDriver>();

        public Telemetry Telemetry { get; private set; }
        public Session Session { get; private set; }
        public IEnumerable<LiveScoringDriver> Scoring { get { return _scoring; } }

        public LiveSession(Telemetry telemetry, Session session)
        {
            Telemetry = telemetry;
            Session = session;
        }

        public void AddScoringDriver(LiveScoringDriver driver)
        {
            if (_scoring.Any(x => x.Equals(driver)) == false)
                _scoring.Add(driver);
            else
                throw new DriverWasAlreadyAddedException();
        }
    }
}
