using System;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain.Aggregates;


namespace SimTelemetry.Tests.LoggerO
{
    [TestFixture]
    public class TelemetryLogReaderTests
    {
        private TelemetryLog reader;

        [Test]
        public void ReadFile()
        {
            reader = new TelemetryLog("Telemetry.zip");

            // Read a region of data
            foreach(var sample in reader.GetSamples(reader.Laps.FirstOrDefault(), 3, 3))
            {
                Console.WriteLine(sample.Drivers.FirstOrDefault().EngineRpm);
            }
        }
    }
}
