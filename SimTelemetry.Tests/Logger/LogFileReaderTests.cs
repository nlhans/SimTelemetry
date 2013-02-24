using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Exceptions;
using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Tests.Logger
{
    [TestFixture]
    public class LogFileReaderTests
    {
        [TestFixtureSetUp]
        public void SetToRAMDisk()
        {
            Directory.SetCurrentDirectory(TestConstants.RAMDISK);
        }

        [Test]
        public void Open()
        {
            var reader = new LogFileReader("test4.zip");

            Assert.AreEqual("test4.zip", reader.FileName);
            Assert.AreEqual(1, reader.Groups.Count());
            Assert.AreEqual("test", reader.Groups.FirstOrDefault().Name);
            Assert.AreEqual(2, reader.Groups.FirstOrDefault().Fields.Count());
            Assert.AreEqual("testInt", reader.Groups.FirstOrDefault().Fields.FirstOrDefault().Name);
            Assert.AreEqual("test", reader.Groups.FirstOrDefault().Fields.Skip(1).FirstOrDefault().Name);
        }

        [Test]
        public void ReadFrequentData()
        {
            var reader = new LogFileReader("test4.zip");

            var sampleProvider = reader.GetProvider(new[] { "test" }, 100, 20000); // from 100ms to 2000ms

            int startI = 100;
            foreach (var sample in sampleProvider.GetSamples())
            {
                var group = sample.Get("test");
                var whatWasAnInteger = group.ReadAs<float>("testInt");

                Assert.AreEqual((float)startI, whatWasAnInteger);
                startI++;

                Assert.LessOrEqual(whatWasAnInteger, 20001);
                Assert.GreaterOrEqual(whatWasAnInteger, 100);
            }
        }

        [Test]
        public void ReadInfrequentData()
        {
            var reader = new LogFileReader("test5.zip");
            var sampleProvider = reader.GetProvider(new[] { "test", "Henk" }, 100, 20000); // from 100ms to 20000ms

            var lastTime = 99;
            int inperiodIntegerCheck = 100 / 20 - 1;
            string inperiodStringCheck = "FFFF";
            foreach (var sample in sampleProvider.GetSamples())
            {
                var time = sample.Timestamp;

                // The difference is always 1, because test has sample every 1.
                Assert.AreEqual(1, time - lastTime);
                lastTime = time;

                var other = sample.Get("Henk");
                var int1 = other.ReadAs<int>("testInt");
                var string1 = other.ReadAs<string>("test");


                if (time % 20 == 0)
                    inperiodIntegerCheck++;
                if (time % 20 == 5)
                {
                    byte[] b = new byte[4];
                    for (int i = 0; i < 4; i++)
                        b[i] = ((byte)(0x41 + (1 + inperiodIntegerCheck) % 26));
                    inperiodStringCheck = Encoding.ASCII.GetString(b);
                }


                Assert.AreEqual(inperiodIntegerCheck, int1);
                Assert.AreEqual(inperiodStringCheck, string1);

            }

            var sampleProvider2 = reader.GetProvider(new[] { "Henk" }, 100, 2000000); // from 100ms to 20000ms
            lastTime = 99;

            foreach (var sample in sampleProvider2.GetSamples())
            {
                var time = sample.Timestamp;
                Assert.AreEqual(1, time - lastTime);
                lastTime = time;

                // even though this group only has new data every '1', the main timeline still has every 1.
                // So they are synchronised this way
            }
        }

        [Test]
        [ExpectedException(typeof(LogFileException))]
        public void ReadEmptyFile()
        {
            LogFileReader reader = new LogFileReader(TestConstants.TestFolder + "EmptyTelemetryFile.zip");
        }
    }
}