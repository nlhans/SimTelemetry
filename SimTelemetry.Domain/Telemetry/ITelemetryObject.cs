namespace SimTelemetry.Domain.Telemetry
{
    public interface ITelemetryObject
    {
        void Update(ITelemetry telemetry, IDataProvider Memory);
    }
}