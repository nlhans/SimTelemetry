using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class SimulatorTests
    {
        [Test]
        public void TestLazyScanning()
        {
            TestConstants.Prepare();

            var trackScanCount = 0;
            var modScanCount = 0;

            GlobalEvents.Hook<PluginTestSimulatorModScanner>((x) => { modScanCount++; }, true);
            GlobalEvents.Hook<PluginTestSimulatorTrackScanner>((x) => { trackScanCount++; }, true);


            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();

                Assert.Greater(pluginHost.Simulators.Count, 0);

                var testSim = pluginHost.Simulators[0];

                var testSimulator = testSim.GetSimulator();
                Assert.AreEqual(0, trackScanCount);
                Assert.AreEqual(0, modScanCount);

                Assert.AreEqual("rFactor", testSimulator.Name);
                Assert.AreEqual("rfactor", testSimulator.ProcessName);
                Assert.AreEqual("1.255", testSimulator.Version);

                var tracks = testSimulator.Tracks;
                var mods = testSimulator.Mods;

                Assert.AreEqual(1, modScanCount);
                Assert.AreEqual(1, trackScanCount);

                Assert.AreEqual(1, tracks.Count(x => x.ID != string.Empty));
                Assert.AreEqual(1, mods.Count(x => x.Name != ""));
            }
        }
    }
}
