namespace SimTelemetry.Domain.Events
{
    public class TelemetryRefresh
    {
        public Aggregates.Telemetry Telemetry { get; private set; }

        public TelemetryRefresh(Aggregates.Telemetry instance)
        {
            Telemetry = instance;
        }
    }
}