using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;
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
                Stopwatch w = new Stopwatch();

                w.Start();

                var carRepo = new CarRepository(testSim.CarProvider);
                var cars = carRepo.GetIds().Count();

                w.Stop();
                Debug.WriteLine("[TIME] Retrieving ID list (" + cars + ") costs " + w.ElapsedMilliseconds + "ms");
                w.Reset();


                w.Start();
                var f1Car = carRepo.GetByFile("JButton05.veh");
                if(f1Car != null)
                    Debug.WriteLine("#" + f1Car.StartNumber + ". " + f1Car.Driver);
                Debug.WriteLine("[TIME] Retrieving JButton05.veh car costs " + w.ElapsedMilliseconds + "ms");
                w.Stop();
                w.Reset();


                /*w.Start();
                cars = carRepo.GetAll().Count();
                w.Stop();
                Debug.WriteLine("[TIME] Retrieving all (other) cars costs " + w.ElapsedMilliseconds + "ms");
                w.Reset();*/


                w.Start();
                f1Car = carRepo.GetByFile("TSATO05.veh");
                if (f1Car != null)
                    Debug.WriteLine("#" + f1Car.StartNumber + ". " + f1Car.Driver);
                Debug.WriteLine("[TIME] Retrieving TSATO05.veh car costs " + w.ElapsedMilliseconds + "ms");
                w.Stop();
            }
        }
    }
}
