using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    class TelemetryTests
    {
        [Test]
        public void Record()
        {
            if (Process.GetProcessesByName("rfactor").Length == 0) Assert.Ignore();
            var rFactorProcess = Process.GetProcessesByName("rfactor")[0];

            IntPtr alloc = Marshal.AllocHGlobal(512 * 1024 * 1024);

            using (var host = new Plugins())
            {
                host.PluginDirectory = TestConstants.SimulatorsBinFolder;

                host.Load();

                // Simulators are now loaded

                var testPlugin = host.Simulators.Where(x => x.Name == "Test simulator").FirstOrDefault();
                testPlugin.SimulatorStart(rFactorProcess);

                var telSource = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rFactorProcess);
                var telLogger = new TelemetryLogger("testSim", new TelemetryLoggerConfiguration(true, false, true, false));
                telLogger.SetAnnotater(new TelemetryArchive());
                telSource.SetLogger(telLogger);

                Thread.Sleep(15000);

                telLogger.Close();
                telSource = null;
            }

            Marshal.FreeHGlobal(alloc);
        }


        private void GetWaves(out float[] rpmWave, out float[] speedWave)
        {
            rpmWave = new float[1024];
            speedWave = new float[1024];

            for (int i = 0; i < rpmWave.Length; i++)
            {
                rpmWave[i] = Convert.ToSingle(1000 + 500 * Math.Sin(i / 256.0 * Math.PI * 2));
                speedWave[i] = Convert.ToSingle(2000 - 500 * Math.Sin(i / 256.0 * Math.PI * 2));
            }
        }

        [Test]
        public void TestData()
        {
            RecordTestData();
            PlaybackTestData();
        }

        public void RecordTestData()
        {
            float[] rpmWave;
            float[] speedWave;
            GetWaves(out rpmWave, out speedWave);
            int index = 0;

            var provider = new MemoryProvider(null);
            var fakePool = new MemoryPool("Driver", MemoryAddress.StaticAbsolute, 0, 0);
            fakePool.Add(new MemoryFieldFunc<float>("Speed", (pool) => speedWave[index], true));
            fakePool.Add(new MemoryFieldFunc<float>("RPM", (pool) => rpmWave[index++], true));
            fakePool.Add(new MemoryFieldConstant<bool>("IsAI", false));
            fakePool.Add(new MemoryFieldConstant<bool>("IsPlayer", true));
            provider.Add(fakePool);

            var fakeDriver = new TelemetryDriver(fakePool);
            var fakeDrivers = new List<TelemetryDriver>(new[] {fakeDriver});

            TelemetryLogger logger = new TelemetryLogger("testSim", new TelemetryLoggerConfiguration(true, true, true, true));
            logger.SetDatasource(provider);
            logger.SetAnnotater( new TelemetryArchive());
            GlobalEvents.Fire(new SessionStarted(), true);
            GlobalEvents.Fire(new DriversAdded(null, fakeDrivers), true);

            for(int i = 0 ; i  <1024;i++)
                logger.Update(i*25);

            GlobalEvents.Fire(new SessionStopped(), true);
            Thread.Sleep(500);

            ZipStorer checkFile = ZipStorer.Open("tmp.zip", FileAccess.Read);

            var files = checkFile.ReadCentralDir();
            Assert.AreEqual(3, files.Count);
            Assert.AreEqual(1024 * 2 * 12 + 2 * 8, files.Where(x => x.FilenameInZip.Contains("Data.bin")).FirstOrDefault().FileSize);
            Assert.AreEqual(1024*8, files.Where(x => x.FilenameInZip.Contains("Time.bin")).FirstOrDefault().FileSize);
        }

        public void PlaybackTestData()
        {
            float[] rpmWave;
            float[] speedWave;
            GetWaves(out rpmWave, out speedWave);

            StringBuilder outSine = new StringBuilder();

            var telRead = new LogFileReader("Tmp.zip");
            var telProvider = telRead.GetProvider(new[] { "Driver" }, 0, 1024*25);
            var index = 0;
            foreach (var sample in telProvider.GetSamples())
            {
                var driver = sample.Get("Driver");

                Assert.AreEqual(true, driver.ReadAs<bool>("IsPlayer"));
                Assert.AreEqual(false, driver.ReadAs<bool>("IsAI"));
                Assert.AreEqual(speedWave[index], driver.ReadAs<float>("Speed"));
                Assert.AreEqual(rpmWave[index], driver.ReadAs<float>("RPM"));

                index++;
                outSine.AppendLine(index+","+driver.ReadAs<float>("Speed") + "," + driver.ReadAs<float>("RPM"));
            }
            File.WriteAllText("sinewave.csv", outSine.ToString());

            Assert.AreEqual(1024, index);

            telProvider = telRead.GetProvider(new[] { "Driver" }, 512*25, 1024 * 25);
            index = 512;
            foreach (var sample in telProvider.GetSamples())
            {
                var driver = sample.Get("Driver");

                Assert.AreEqual(true, driver.ReadAs<bool>("IsPlayer"));
                Assert.AreEqual(false, driver.ReadAs<bool>("IsAI"));
                Assert.AreEqual(speedWave[index], driver.ReadAs<float>("Speed"));
                Assert.AreEqual(rpmWave[index], driver.ReadAs<float>("RPM"));

                index++;

            }

            Assert.AreEqual(1024, index);
        }

        [Test]
        public void Export()
        {
            StringBuilder csv = new StringBuilder();

            var telRead = new LogFileReader("Tmp.zip");
            var telProvider = telRead.GetProvider(new[] {"Session","Driver 7427264"}, 0, 1000000);

            foreach(var sample in telProvider.GetSamples())
            {
                var me = sample.Get("Driver 7427264");
                var sess = sample.Get("Session");

                if(me!= null)
                csv.AppendLine(sample.Timestamp + "," +
                                 sess.ReadAs<float>("Time") + "," +
                                 me.ReadAs<string>("TyreCompoundFront") + "," +
                                 me.ReadAs<float>("Speed") + "," +
                                 me.ReadAs<float>("Fuel") + "," +
                                 me.ReadAs<float>("InputThrottle") + "," +
                                 me.ReadAs<float>("InputBrake") + "," +
                                 me.ReadAs<float>("TyreTemperatureInsideLF") + "," +
                                 me.ReadAs<float>("RPM") + "," +
                                 me.ReadAs<int>("Gear"));
            }

            File.WriteAllText("dump.csv", csv.ToString());

        }
    }
}
