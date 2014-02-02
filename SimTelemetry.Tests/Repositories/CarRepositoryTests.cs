using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;

namespace SimTelemetry.Tests.Repositories
{
    [TestFixture]
    class CarRepositoryTests
    {
        [Test]
        public void CarDataProviderTests()
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

                var carRepo = new CarRepository(testSim.CarProvider);
                var cars = carRepo.GetIds().Count();

                // I got over 1106 cars installed, so that's more than 100.
                Assert.Greater(cars,100);
                w.Stop();
                Debug.WriteLine("[TIME] Retrieving ID list (" + cars + ") costs " + w.ElapsedMilliseconds + "ms");
                w.Reset();


                w.Start();

                // TODO: You need to install CM F1 2010 rFactor mod.
                var f1Car = carRepo.GetByFile("Sauber_22.veh");
                Assert.AreNotEqual(f1Car, null);
                if (f1Car != null)
                    Debug.WriteLine("#" + f1Car.StartNumber + ". " + f1Car.Driver);
                Debug.WriteLine("[TIME] Retrieving Sauber_22.veh car costs " + w.ElapsedMilliseconds + "ms");
                w.Stop();
                w.Reset();


                // This test is very very very slow:
                /*w.Start();
                cars = carRepo.GetAll().Count();
                w.Stop();
                Debug.WriteLine("[TIME] Retrieving all (other) cars costs " + w.ElapsedMilliseconds + "ms");
                w.Reset();*/

                w.Start();
                f1Car = carRepo.GetByFile("Sauber_23.veh");
                Assert.AreNotEqual(f1Car, null);
                if (f1Car != null)
                    Debug.WriteLine("#" + f1Car.StartNumber + ". " + f1Car.Driver);
                Debug.WriteLine("[TIME] Retrieving Sauber_23.veh car costs " + w.ElapsedMilliseconds + "ms");
                w.Stop();

                w.Reset();

                // Verify that when cached, this is very quick:
                w.Start();
                f1Car = carRepo.GetByFile("Sauber_22.veh");
                Assert.AreNotEqual(f1Car, null);
                if (f1Car != null)
                    Debug.WriteLine("#" + f1Car.StartNumber + ". " + f1Car.Driver);
                Debug.WriteLine("[TIME] Retrieving Sauber_22.veh car costs " + w.ElapsedMilliseconds + "ms");

                w.Stop();
                w.Reset();
            }
        }

    }
}
