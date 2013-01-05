using System.Diagnostics;
using SimTelemetry.Core;
using SimTelemetry.Core.Events;

namespace SimTelemetry.Tests
{
    public class TestConstants
    {
        public const string TelemetryFolder =
            @"C:\Users\Hans\Documents\GitHub\SimTelemetry\LiveTelemetry\bin\Debug\";

        public const string SimulatorsBinFolder = TelemetryFolder + "Simulators\\";

        public static int Warnings { get; set; }

        public static void Prepare()
        {
            Warnings = 0;

            // Listen to warnings:
            GlobalEvents.Hook<DebugWarning>(PrintWarning, false);
        }

        private static void PrintWarning(DebugWarning w)
        {
            Debug.WriteLine("[Warning] " + w.Message);
            Debug.WriteLine(w.Exception.Message);
            Warnings++;

        }
    }
}