using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Repositories;

namespace SimTelemetry.Tests.Repositories
{
    [TestFixture]
    public class TelemetryRepositoryTests
    {
        [Test]
        public void CreateNewRepositoryAndDoubleOpen()
        {
            if (File.Exists(TestConstants.RAMDISK + "\\logs1.xml"))
                File.Delete(TestConstants.RAMDISK + "\\logs1.xml");

            TelemetryRepository repo1 = new TelemetryRepository(TestConstants.RAMDISK + "\\logs1.xml");
            TelemetryRepository repo2 = new TelemetryRepository(TestConstants.RAMDISK + "\\logs1.xml");

            // should not throw errors, because the file hasn't been created yet.
            repo1.Export();

            Assert.IsTrue(File.Exists(TestConstants.RAMDISK+"\\logs1.xml"));

            TelemetryRepository repo3 = new TelemetryRepository(TestConstants.RAMDISK + "\\logs1.xml");
        }

        [Test]
        public void CreateNewRepositoryAndReadIt()
        {
            if(File.Exists(TestConstants.RAMDISK + "\\logs.xml"))
                File.Delete(TestConstants.RAMDISK + "\\logs.xml");
            TelemetryRepository repo = new TelemetryRepository(TestConstants.RAMDISK + "\\logs.xml");

            Assert.AreEqual(0, repo.GetAll().Count());

            repo.Add(new TelemetryLog(1, "test/test.zip"));

            Assert.AreEqual(1, repo.GetAll().Count());

            repo.Export();

            TelemetryRepository repo2 = new TelemetryRepository(TestConstants.RAMDISK + "\\logs.xml");
            Assert.AreEqual(1, repo2.GetAll().Count());

        }
    }
}
