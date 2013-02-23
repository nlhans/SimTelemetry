using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    class TelemetryTests
    {
        [Test]
        public void Record()
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

                Thread.Sleep(300000);

                telLogger.Close();
                telSource = null;
            }
        }

        [Test]
        public void Playback()
        {
            StringBuilder csv = new StringBuilder();

            var telRead = new LogFileReader("Tel.zip");
            var telProvider = telRead.GetProvider(new[] {"Session","Driver 9183488"}, 0, 1000000);

            Stopwatch w = new Stopwatch();
            w.Start();
            int samples = 0;
            foreach(var sample in telProvider.GetSamples())
            {
                var me = sample.Get("Driver 9183488");
                var sess = sample.Get("Session");
                samples++;

                csv.AppendLine(sample.Timestamp + "," +
                                 sess.ReadAs<float>("Time") + "," +
                                 me.ReadAs<string>("TyreCompoundFront") + "," +
                                 me.ReadAs<float>("Speed") + "," +
                                 me.ReadAs<float>("Fuel") + "," +
                                 me.ReadAs<float>("TyreTemperatureInsideLF") + "," +
                                 me.ReadAs<float>("RPM") + "," +
                                 me.ReadAs<int>("Gear"));
            }

            w.Stop();
            Debug.WriteLine(w.ElapsedMilliseconds +","+samples);
            File.WriteAllText("dump.csv", csv.ToString());

        }
    }
}
