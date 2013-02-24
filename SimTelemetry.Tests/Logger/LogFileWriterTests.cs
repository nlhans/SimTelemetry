using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Tests.Logger
{
    [TestFixture]
    public class LogFileWriterTests
    {
        [TestFixtureSetUp]
        public void SetToRAMDisk()
        {
            Directory.SetCurrentDirectory(TestConstants.RAMDISK);
        }

        [Test]
        public void Create()
        {
            var file = new LogFileWriter("test.zip");
            Assert.AreEqual("test.zip", file.FileName);
            Assert.AreEqual(0, file.Groups.Count());
            Assert.True(Directory.Exists("tmp"));

            file.Save();

            ZipStorer zip = ZipStorer.Open("test.zip", FileAccess.Read);

            Assert.AreEqual("SimTelemetry log", zip.Comment);
            Assert.AreEqual(0, zip.ReadCentralDir().Count);
            Assert.False(Directory.Exists("tmp"));

        }

        [Test]
        public void Groups()
        {
            var file = new LogFileWriter("test2.zip");
            var source = new MemoryPool("test", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source.Add(new MemoryFieldFunc<int>("testInt", (pool) => 123));
            source.Add(new MemoryFieldFunc<string>("test", (pool) => "test"));

            file.Subscribe(source);

            Assert.AreEqual(1, file.Groups.Count());
            Assert.AreEqual(source.Name, file.Groups.FirstOrDefault().Name);
            Assert.AreEqual(true, file.Groups.FirstOrDefault().Subscribed);
            Assert.AreEqual(2, file.Groups.FirstOrDefault().Fields.Count());
            Assert.AreEqual(true, file.Groups.FirstOrDefault().Fields.Any(x => x.Name == "testInt"));
            Assert.AreEqual(true, file.Groups.FirstOrDefault().Fields.Any(x => x.Name == "test"));

            Assert.AreEqual(source, file.Groups.FirstOrDefault().DataSource);

            file.Subscribe(source);

            Assert.AreEqual(1, file.Groups.Count());

            file.Unsubscribe(source);

            Assert.AreEqual(1, file.Groups.Count()); // but it's become inactive instead;
            Assert.AreEqual(false, file.Groups.FirstOrDefault().Subscribed);


            source.Add(new MemoryFieldFunc<string>("test2", (pool) => "test"));

            file.Subscribe(source);

            Assert.AreEqual(1, file.Groups.Count());
            Assert.AreEqual(true, file.Groups.FirstOrDefault().Subscribed);
            Assert.AreEqual(3, file.Groups.FirstOrDefault().Fields.Count());

            file.Save();

            ZipStorer z = ZipStorer.Open("test2.zip", FileAccess.Read);

            var files = z.ReadCentralDir();
            Assert.AreEqual(3, files.Count);
            Assert.True(files.Any(x => x.FilenameInZip == "test/Data.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Structure.xml"));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Time.bin"));
        }

        [Test]
        public void Data()
        {
            var counter = 0;
            var file = new LogFileWriter("test3.zip");
            var source = new MemoryPool("test", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source.Add(new MemoryFieldFunc<int>("testInt", (pool) => counter++)); // these are always logged
            source.Add(new MemoryFieldFunc<string>("test", (pool) => "test"));

            file.Subscribe(source);

            // Fill up  a data file
            for (int i = 0; i < 1441792; i++) // 38.5MiB
                file.Update(i); // +28 bytes

            // We should have logged 2 data files by now.
            Assert.True(File.Exists("tmp/test/Data.bin"));

            file.Save();

            ZipStorer z = ZipStorer.Open("test3.zip", FileAccess.Read);

            var files = z.ReadCentralDir();
            Assert.AreEqual(3, files.Count);
            Assert.True(files.Any(x => x.FilenameInZip == "test/Data.bin"));
            Assert.True(files.Where(x => x.FilenameInZip == "test/Data.bin").FirstOrDefault().FileSize ==
                        1441792*((2 + 4 + 4 + 2) + (2 + 4 + 4 + 4 + 2)));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Structure.xml"));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Time.bin"));
            Assert.True(files.Where(x => x.FilenameInZip == "test/Time.bin").FirstOrDefault().FileSize == 1441792 * 8);
        }

        [Test]
        public void DataWithResubscriptions()
        {
            var counter = 0;
            var file = new LogFileWriter("test4.zip");
            var source = new MemoryPool("test", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source.Add(new MemoryFieldFunc<int>("testInt", (pool) => counter++)); // these are always logged
            source.Add(new MemoryFieldFunc<string>("test", (pool) => "test"));

            file.Subscribe(source);

            // Fill up next 1/2 of data file
            int i = 0;
            for (i = 0; i < 1441792 / 2; i++) // 33MiB
                file.Update(i); // +24 bytes

            file.Unsubscribe(source);

            file.Update(i++); // does nothing now, but the data thing does miss 1 time .

            file.Subscribe(source);

            // Fill up next 1/2 of a data file
            for (; i < 1441792; i++) // 33MiB
                file.Update(i); // +24 bytes


            // We should have logged 2 data files by now.
            Assert.True(File.Exists("tmp/test/Data.bin"));

            file.Save();

            ZipStorer z = ZipStorer.Open("test4.zip", FileAccess.Read);

            var files = z.ReadCentralDir();
            Assert.AreEqual(3, files.Count);
            Assert.True(files.Any(x => x.FilenameInZip == "test/Data.bin"));
            Assert.True(files.Where(x => x.FilenameInZip == "test/Data.bin").FirstOrDefault().FileSize == (1441792-1)*28);
            Assert.True(files.Any(x => x.FilenameInZip == "test/Structure.xml"));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Time.bin"));
            Assert.True(files.Where(x => x.FilenameInZip == "test/Time.bin").FirstOrDefault().FileSize == 1441792 * 8 - 8);
        }

        [Test]
        public void DataWithInfrequentFields()
        {
            int i = 0;

            var counter = 0;
            var counter2 = 0;
            var checkCalls = 0;
            var file = new LogFileWriter("test5.zip");
            var source = new MemoryPool("test", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source.Add(new MemoryFieldFunc<int>("testInt", (pool) => counter++)); // these are always logged
            source.Add(new MemoryFieldFunc<string>("test", (pool) => "test"));

            var source2 = new MemoryPool("Henk", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source2.Add(new MemoryFieldCustomFunc<int>("testInt", (pool) => counter2++, () => (i%20 == 0))); // log every 20 samples.
            source2.Add(new MemoryFieldCustomFunc<string>("test", (pool) =>
                                                               {
                                                                   byte[] data = new byte[4];
                                                                   for (int j = 0; j < 4; j++)
                                                                       data[j] = ((byte)(0x41 + counter2 % 26));
                                                                   return Encoding.ASCII.GetString(data);
                                                               }, () => (i%20 == 5)));

            file.Subscribe(source);
            file.Subscribe(source2);

            // Fill up next 1/2 of data file
            for (i = 0; i < 224695; i++) // 33MiB
                file.Update(i); // +24 bytes

            // We should have logged 2 data files by now.
            Assert.True(File.Exists("tmp/test/Data.bin"));
            Assert.False(File.Exists("tmp/Henk/Data.bin"));

            file.Save();

            ZipStorer z = ZipStorer.Open("test5.zip", FileAccess.Read);

            var files = z.ReadCentralDir();
            Assert.AreEqual(6, files.Count);
            Assert.True(files.Any(x => x.FilenameInZip == "test/Data.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Structure.xml"));
            Assert.True(files.Any(x => x.FilenameInZip == "test/Time.bin"));

            Assert.True(files.Any(x => x.FilenameInZip == "Henk/Data.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Henk/Structure.xml"));
            Assert.True(files.Any(x => x.FilenameInZip == "Henk/Time.bin"));
        }
    }

    public class MemoryFieldCustomFunc<T> : MemoryFieldFunc<T>
    {
        private Func<bool> hasChangedFnc;

        public override bool HasChanged()
        {
            return hasChangedFnc();
        }

        public MemoryFieldCustomFunc(string name, Func<MemoryPool, T> getValue, Func<bool> hasChanged) : base(name, getValue)
        {
            hasChangedFnc = hasChanged;
        }
    }
}
