using SimTelemetry.Tests.Telemetry;

namespace SimTelemetry.Tests
{
    class App
    {
        static void Main(string[] args)
        {
            var t = new TelemetryTests();
            t.Playback();
            return;
        }
    }
}