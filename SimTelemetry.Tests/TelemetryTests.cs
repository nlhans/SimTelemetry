using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Core;
using SimTelemetry.Core.Plugins;
using SimTelemetry.Domain.Events;

namespace SimTelemetry.Tests
{
    public class TestConstants
    {
        public const string TelemetryFolder =
            @"C:\Users\Hans\Documents\GitHub\SimTelemetry\LiveTelemetry\bin\Debug\";

        public const string SimulatorsBinFolder = TelemetryFolder + "Simulators\\";

        public static void Prepare()
        {
            //

        }
    }

    [TestFixture]
    public class TelemetryTests
    {
        public void PrintWarnings(DebugWarningException ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.Exception.Message);
            Debug.WriteLine(ex.StackTrace);
        }

        [Test]
        public void SimulatorPluginTest()
        {
            TestConstants.Prepare();

            // Listen to warnings:
            Events.Hook<DebugWarningException>(PrintWarnings,false);

            var pluginHost = new Plugins();
            pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

            pluginHost.Load();

            // Manually count the no of plugins in the bin directory.
            var files = Directory.GetFiles(TestConstants.SimulatorsBinFolder);
            var plugins = files.Where(x => x.Contains("SimTelemetry.Game."));

            // Resolve the no. of plugins there are available:
            var iPluginsManualCount = plugins.ToList().Count;
            var iPluginsTelemetry = pluginHost.Simulators.ToList().Count;

            Assert.AreNotEqual(0, iPluginsManualCount);
            Assert.AreEqual(iPluginsTelemetry, iPluginsManualCount);
            
        }

        [Test]
        public void SimulatorRunningTest()
        {
            Assert.AreEqual(0, 0);
        }
    }
}
