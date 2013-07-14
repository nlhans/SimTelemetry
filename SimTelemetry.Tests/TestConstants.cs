using System.Diagnostics;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;

namespace SimTelemetry.Tests
{
    public class TestConstants
    {
        public const string RAMDISK = @"C:\Projects\Software\SimTelemetry\SimTelemetry.Tests\bin\Debug\Test\";

        public const string TestFolder = @"C:\Projects\Software\SimTelemetry\TestData\";

        public const string TelemetryFolder =
            @"C:\Projects\Software\SimTelemetry\SimTelemetry.Tests\bin\Debug\";

        public const string SimulatorsBinFolder = @"C:\Projects\Software\SimTelemetry\SimTelemetry.Plugins.Tests\bin\Debug\";
        public static int Warnings { get; set; }


        public const  float MemoryTestFuelCapacity = 260.0f;

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