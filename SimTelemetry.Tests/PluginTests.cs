using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Core;
using SimTelemetry.Core.Events;
using SimTelemetry.Core.Plugins;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class PluginTests
    {
        [Test]
        public void PluginsFound()
        {
            bool PluginsLoadedEventFire = false;
            int Warnings = 0;

            TestConstants.Prepare();

            // Listen to warnings:
            GlobalEvents.Hook<DebugWarning>((ex) =>
                                          {
                                              Debug.WriteLine("[Warning] " + ex.Message);
                                              Debug.WriteLine(ex.Exception.Message);
                                              Warnings++;
                                          }, false);

            GlobalEvents.Hook<PluginsLoaded>((x) =>
            {
                Assert.AreNotEqual(x.Simulators, null);
                Assert.AreNotEqual(x.Widgets, null);
                Assert.AreNotEqual(x.Extensions, null);

                Assert.AreEqual(x.Simulators.ToList().Count, 1);
                Assert.AreEqual(x.Widgets.ToList().Count, 0);
                Assert.AreEqual(x.Extensions.ToList().Count, 0);

                PluginsLoadedEventFire = true;
            }, true);

            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();

                // Manually count the no of plugins in the bin directory.
                var files = Directory.GetFiles(TestConstants.SimulatorsBinFolder);
                var plugins = files.Where(x => x.Contains("SimTelemetry.Plugins."));

                // Resolve the no. of plugins there are available:
                var iPluginsManualCount = plugins.ToList().Count;
                var iPluginsTelemetry = pluginHost.Simulators.ToList().Count;

                Assert.AreNotEqual(0, iPluginsManualCount);
                Assert.AreEqual(iPluginsTelemetry, iPluginsManualCount);
            }

            Assert.AreEqual(PluginsLoadedEventFire, true);
            Assert.AreEqual(Warnings, 1); // 1 Widget fails.
        }
    }
}