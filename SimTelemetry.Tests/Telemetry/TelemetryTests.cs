using System;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Plugins;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    public class TelemetryTests
    {
        [Test]
        public void Basic()
        {
            var rfp = Process.GetProcessesByName("rfactor");
            if(rfp.Length == 0)
                Assert.Ignore();
            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                testPlugin.Initialize();
                testPlugin.SimulatorStart(rfp[0]);
                var telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rfp[0]);
                var telemetryLogger = new TelemetryLogWriter();
                telemetryObject.SetLogger(telemetryLogger);

            }
        }

        [Test]
        public void ReadFile() 
        {
            TelemetryLog reader = new TelemetryLog("Telemetry.zip");
        }

        int calls = 0;
        private Domain.Aggregates.Telemetry telemetryObject;
        public void Continous()
        {
            var rfp = Process.GetProcessesByName("rfactor");
            if (rfp.Length == 0)
                Assert.Ignore();

            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                testPlugin.SimulatorStart(rfp[0]);
                telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rfp[0]);
                var telemetryLogger = new TelemetryLogWriter();
                telemetryObject.SetLogger(telemetryLogger);
                telemetryLogger.UpdateConfiguration(new TelemetryLogConfiguration(true, false, true, true));
                
                Console.ReadLine();
            }
        }

    }
}
