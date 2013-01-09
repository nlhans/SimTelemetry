using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class Simulator
    {
        [Test]
        public void TestLazyScanning()
        {
            TestConstants.Prepare();

            var trackScanCount = 0;
            var modScanCount = 0;

            GlobalEvents.Hook<PluginTestSimulatorModScanner>((x) => {  modScanCount++;  },true);
            GlobalEvents.Hook<PluginTestSimulatorTrackScanner>((x) => { trackScanCount++; },true);


            using (var pluginHost = new SimTelemetry.Domain.Plugins.Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();

                Assert.Greater(pluginHost.Simulators.Count,0);

                var testSim = pluginHost.Simulators[0];

                var testSimulator = testSim.GetSimulator();
                Assert.AreEqual(0, trackScanCount);
                Assert.AreEqual(0, modScanCount);

                Assert.AreEqual(testSimulator.Name, "rFactor");
                Assert.AreEqual(testSimulator.ProcessName, "rfactor");
                Assert.AreEqual(testSimulator.Version, "1.255");

                // okay, get tracks & mod
                var tracks = testSimulator.Tracks;
                var mods = testSimulator.Mods;

                Assert.AreEqual(modScanCount, 1);
                Assert.AreEqual(trackScanCount, 1);

                Assert.AreEqual(tracks.Count(x => x.ID > 0), 1);
                Assert.AreEqual(mods.Count(x => x.Name != ""), 1);
            }
        }
    }
}
