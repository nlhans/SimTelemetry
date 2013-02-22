using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    class TelemetryTests
    {
        [Test]
        public void TestData()
        {
            if(Process.GetProcessesByName("rfactor").Length==0) Assert.Ignore();
            var rFactorProcess = Process.GetProcessesByName("rfactor")[0];

            using(var host = new Plugins())
            {
                host.PluginDirectory = TestConstants.SimulatorsBinFolder;

                host.Load();

                // Simulators are now loaded

                var testPlugin = host.Simulators.Where(x => x.Name == "Test simulator").FirstOrDefault();
                testPlugin.SimulatorStart(rFactorProcess);

                var telSource = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rFactorProcess);
                var telLogger = new TelemetryLogger(new TelemetryLoggerConfiguration(true, false, true, false));
                telSource.SetLogger(telLogger);

                while(Console.KeyAvailable==false)
                Thread.Sleep(1000);

                telSource = null;
                telLogger.Close();
            }
        }
    }
}
