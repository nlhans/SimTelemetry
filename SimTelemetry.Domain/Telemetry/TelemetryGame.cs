namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryGame : ITelemetryObject
    {
        public string Version;

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
        }


        public TelemetryGame Clone()
        {
            return (TelemetryGame)MemberwiseClone();
        }
    }
}