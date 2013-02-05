using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    public class TelemetryTests
    {
        [Test]
        public void Basic()
        {
            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                testPlugin.Initialize();
                Process rf = Process.GetProcessesByName("rfactor")[0];
                testPlugin.SimulatorStart(rf);
                var telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);
                var telemetryLogger = new SimTelemetryLogWriter();
                telemetryObject.SetLogger(telemetryLogger);

            }
        }


        int calls = 0;
        private Domain.Aggregates.Telemetry telemetryObject;
        public void Continous()
        {
            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                Process rf = Process.GetProcessesByName("rfactor")[0];
                testPlugin.SimulatorStart(rf);
                telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);
                var telemetryLogger = new SimTelemetryLogWriter();
                telemetryObject.SetLogger(telemetryLogger);

                Console.ReadLine();
            }
        }

    }
}
