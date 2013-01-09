using System.Collections.Generic;

namespace SimTelemetry.Core.Aggregates
{
    public class Telemetry
    {
        private IList<TelemetryDriver> _drivers = new List<TelemetryDriver>();

        public bool Active { get; private set; }

        public TelemetryPlayer Player { get; private set; }
        public IEnumerable<TelemetryDriver> Drivers { get { return _drivers; } }

        public TelemetrySession Session { get; private set; }
        public TelemetryTrack Track { get; private set; }
        public TelemetryGame Simulator { get; private set; }

        public TelemetryAcquisition Acquisition { get; private set; }

        public TelemetrySupport Support;



    }
}