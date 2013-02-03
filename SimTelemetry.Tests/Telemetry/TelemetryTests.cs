﻿using System;
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
                var telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);

                int i = 0;
                while (i++ < 100)
                {
                    telemetryObject.Update();
                    Debug.WriteLine(telemetryObject.Session.Time);

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
                var telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);

                while (true)
                {
                    telemetryObject.Update();
                    //Console.Write(telemetryObject.Session.Time.ToString("0000") + " | " + telemetryObject.Session.Cars.ToString("000"));
                    //foreach(var driver in telemetryObject.Drivers)
                    //{
                    //    Console.Write(" | " + driver.RPM.ToString("00000"));
                    //
                    //}
                    //Console.WriteLine();
                    Thread.Sleep(20);
                }

            }
        }
    }
}
