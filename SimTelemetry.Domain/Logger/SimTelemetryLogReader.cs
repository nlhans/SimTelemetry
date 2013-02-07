namespace SimTelemetry.Domain.Logger
{
    public class SimTelemetryLogReader
    {
        protected LogFile _log;
        //
        public SimTelemetryLogReader(string file)
        {
            _log = new LogFile(file);

        }
    }
}