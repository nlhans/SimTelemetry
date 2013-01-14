using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;

namespace SimTelemetry.Tests.Repositories
{
    [TestFixture]
    class TrackRepositoryTests
    {
        [Test]
        public void TrackDataProviderTests()
        {
            TestConstants.Prepare();

            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;
                pluginHost.Load();

                Assert.Greater(pluginHost.Simulators.Count, 0);
                var testSim = pluginHost.Simulators[0];

                var w = new Stopwatch();

                w.Start();

                var mem = GC.GetTotalMemory(true);
                var trackRepo = new TrackRepository(testSim.TrackProvider);
                var tracks = trackRepo.GetIds().Count();

                w.Stop();
                Debug.WriteLine("[TIME] Retrieving ID list (" + tracks + ") costs " + w.ElapsedMilliseconds + "ms");
                w.Reset();

                w.Start();
                var Spa = trackRepo.GetByFile("67_SPA.GDB");
                Assert.AreNotEqual(null, Spa);
                Debug.WriteLine(Spa.Name + " " + Spa.Length);

                w.Stop();
                Debug.WriteLine("[TIME] Retrieving Spa 1967 costs " + w.ElapsedMilliseconds + "ms");
                w.Reset();

                // Building all tracks can take seconds.
                /*w.Start();
                foreach(var track in trackRepo.GetAll())
                    Debug.WriteLine(track.Name + "=" + track.Length);
                w.Stop();
                Debug.WriteLine("[TIME] Retrieving all tracks costs " + w.ElapsedMilliseconds + "ms");*/


                w.Start();
                
                Spa = trackRepo.GetByFile("67_SPA.GDB");
                Assert.AreNotEqual(null, Spa);
                Debug.WriteLine(Spa.Name + " " + Spa.Length);

                w.Stop();
                Debug.WriteLine("[TIME] Retrieving Spa 1967 costs " + w.ElapsedMilliseconds + "ms");
                Assert.LessOrEqual(w.ElapsedMilliseconds, 1);
                w.Reset();

                var dmem = GC.GetTotalMemory(true) - mem;
                Debug.WriteLine(dmem);
                w.Reset();
            }

        }
    }
}
