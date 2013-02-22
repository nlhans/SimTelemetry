using System.Diagnostics;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;

namespace SimTelemetry.Tests
{
    public class TestConstants
    {
        public const string TestFolder = @"C:\Users\Hans\Documents\Visual Studio 2010\Projects\SimTelemetry\TestData\";

        public const string TelemetryFolder =
            @"C:\Users\Hans\Documents\Visual Studio 2010\Projects\SimTelemetry\SimTelemetry.Plugins.Tests\bin\Debug\";

        public const string SimulatorsBinFolder = TelemetryFolder;
        public static int Warnings { get; set; }

        public static void Prepare()
        {
            Warnings = 0;

            // Listen to warnings:
            GlobalEvents.Hook<DebugWarning>(PrintWarning, false);
        }

        private static void PrintWarning(DebugWarning w)
        {
            Debug.WriteLine("[Exception] " + w.Message);
            Debug.WriteLine(w.Exception.Message);
            Warnings++;

        }
    }
}