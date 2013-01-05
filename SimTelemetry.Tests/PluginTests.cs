using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Core;
using SimTelemetry.Core.Events;
using SimTelemetry.Core.Plugins;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class PluginTests
    {

        const int  cfgSimulatorPlugins = 1;
        const int cfgExtensionPlugins = 0;
        const int cfgWidgetPlugins = 1;

        private const int cfgSimulatorPluginsInvalid = 0;
        private const int cfgExtensionPluginsInvalid = 0;
        private const int cfgWidgetPluginsInvalid = 1;


        [Test]
        public void PluginsFound()
        {
            bool pluginsLoadedEventFire = false;

            TestConstants.Prepare();

            // Listen to warnings:

            GlobalEvents.Hook<PluginsLoaded>((x) =>
            {
                Assert.AreNotEqual(x.Simulators, null);
                Assert.AreNotEqual(x.Widgets, null);
                Assert.AreNotEqual(x.Extensions, null);

                Assert.AreEqual(x.Simulators.ToList().Count, cfgSimulatorPlugins - cfgSimulatorPluginsInvalid);
                Assert.AreEqual(x.Widgets.ToList().Count, cfgWidgetPlugins - cfgWidgetPluginsInvalid);
                Assert.AreEqual(x.Extensions.ToList().Count, cfgExtensionPlugins - cfgExtensionPluginsInvalid);

                pluginsLoadedEventFire = true;
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

            Assert.AreEqual(pluginsLoadedEventFire, true);
            Assert.AreEqual(TestConstants.Warnings, cfgWidgetPluginsInvalid + cfgSimulatorPluginsInvalid + cfgExtensionPluginsInvalid);
        }

        [Test]
        public void PluginsMultipleLoad()
        {
            int pluginLoadIterations = 0;

            int constructorSimulator = 0;
            int constructorWidget = 0;
            int constructorExtension = 0;

            TestConstants.Prepare();

            GlobalEvents.Hook<PluginsLoaded>((x) =>
                                                 {
                                                     pluginLoadIterations++;

                                                     Assert.AreNotEqual(x.Simulators, null);
                                                     Assert.AreNotEqual(x.Widgets, null);
                                                     Assert.AreNotEqual(x.Extensions, null);

                                                     Assert.AreEqual(x.Simulators.ToList().Count, cfgSimulatorPlugins - cfgSimulatorPluginsInvalid);
                                                     Assert.AreEqual(x.Widgets.ToList().Count, cfgWidgetPlugins - cfgWidgetPluginsInvalid);
                                                     Assert.AreEqual(x.Extensions.ToList().Count, cfgExtensionPlugins - cfgExtensionPluginsInvalid);
                                                 }, true);

            // Count number of constructors.
            GlobalEvents.Hook<PluginTestExtensionConstructor>((x) => constructorExtension++, false);
            GlobalEvents.Hook<PluginTestWidgetConstructor>((x) => constructorWidget++, false);
            GlobalEvents.Hook<PluginTestSimulatorConstructor>((x) => constructorSimulator++, false);

            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                pluginHost.Unload();
                pluginHost.Load();
                pluginHost.Unload();
            }

            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
            }

            // Verify everything!
            Assert.AreEqual(constructorSimulator, pluginLoadIterations * cfgSimulatorPlugins);
            Assert.AreEqual(constructorExtension, pluginLoadIterations * cfgExtensionPlugins);
            Assert.AreEqual(constructorWidget, pluginLoadIterations * cfgWidgetPlugins);

            Assert.AreEqual(TestConstants.Warnings,
                            pluginLoadIterations*
                            (cfgWidgetPluginsInvalid + cfgSimulatorPluginsInvalid + cfgExtensionPluginsInvalid));

        }
    }
}