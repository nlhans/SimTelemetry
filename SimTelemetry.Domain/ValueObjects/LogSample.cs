using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Domain.LoggerO;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.ValueObjects
{
    public class LogSample
    {
        public TelemetrySession Session { get; protected set; }
        public TelemetryGame Simulator { get; protected set; }

        public TelemetryAcquisition Acquisition { get; protected set; }

        public TelemetrySupport Support { get; protected set; }

        public IEnumerable<TelemetryDriver> Drivers { get { return _drivers; } }
        protected readonly IList<TelemetryDriver> _drivers = new List<TelemetryDriver>();

        public int Timestamp { get; private set; }

        public LogSample(LogFile file, int timestamp)
        {
            Session = new TelemetrySession();
            this.Simulator = new TelemetryGame();
            Acquisition = new TelemetryAcquisition();
            Support = new TelemetrySupport();

            foreach(var group in file.Groups.Where(x => x.Name.StartsWith("Driver ")))
                _drivers.Add(new TelemetryDriver(group));
        }

        protected LogSample(TelemetrySession session, TelemetryGame simulator, TelemetryAcquisition acquisition, TelemetrySupport support, IList<TelemetryDriver> drivers, int timestamp)
        {
            Session = session;
            Simulator = simulator;
            Acquisition = acquisition;
            Support = support;
            Timestamp = timestamp;
            _drivers = drivers;
        }

        public LogSample Clone()
        {
            var clonedDrivers = new List<TelemetryDriver>(Drivers.Select(x => x.Clone()));
           LogSample cloneOfMe = new LogSample(Session.Clone(), Simulator.Clone(), Acquisition, Support, clonedDrivers, Timestamp);

            return cloneOfMe;
        }
    }
}