using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain.Plugins;

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

                Domain.Aggregates.Telemetry t = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);

                int i = 0;
                while (i++ < 100)
                {

                    t.Update();
                    Debug.WriteLine(t.Session.Time);

                    Thread.Sleep(10);
                }

            }
        }

        public void Continous()
        {
            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                testPlugin.Initialize();

                Process rf = Process.GetProcessesByName("rfactor")[0];

                testPlugin.SimulatorStart(rf);

                Domain.Aggregates.Telemetry t = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);

                int i = 0;
                while (true)
                {

                    t.Update();
                   Console.WriteLine(t.Session.Time);

                    Thread.Sleep(20);
                }

            }
        }
    }
}
