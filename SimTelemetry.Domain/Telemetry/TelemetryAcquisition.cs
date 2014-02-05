namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryAcquisition : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {

        }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {

        }
    }
}