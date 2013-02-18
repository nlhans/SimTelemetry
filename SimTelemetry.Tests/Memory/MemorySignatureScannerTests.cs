using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    [TestFixture]
    class MemorySignatureScannerTests
    {
        private DiagnosticMemoryReader reader;
        private MemorySignatureScanner sigScan;
        private List<MemoryReadAction> actionLogbook;

        public void InitTests()
        {
            if (Process.GetProcessesByName("rfactor").Length == 0) Assert.Ignore();

            actionLogbook = new List<MemoryReadAction>();
            GlobalEvents.Hook<MemoryReadAction>(x =>
            {
                actionLogbook.Add(x);
                Debug.WriteLine(string.Format("Reading 0x{0:X}[0x{1:X}]", x.Address, x.Size));
            }, true);

            reader = new DiagnosticMemoryReader();
            reader.Open(Process.GetProcessesByName("rfactor")[0]);

            sigScan = new MemorySignatureScanner(reader);
        }

        [Test]
        public void ScanRegions()
        {
            InitTests();

            Assert.AreEqual(0, actionLogbook.Count);

            sigScan.Enable();

            Assert.AreEqual(reader.Regions.Count(x => x.Size < 0x1000000), actionLogbook.Count);

            int i = 0;
            foreach (var region in reader.Regions.Where(x => x.Size < 0x1000000))
            {
                Assert.AreEqual(region.Size, actionLogbook[i].Size);
                Assert.AreEqual(region.BaseAddress, actionLogbook[i].Address);
                Assert.AreEqual(region.Size, region.Data.Length);
                i++;
            }

            sigScan.Disable();

            Assert.AreEqual(reader.Regions.Count(x => x.Size < 0x1000000), actionLogbook.Count);

            foreach (var region in reader.Regions.Where(x => x.Size < 0x1000000))
            {
                Assert.AreEqual(0, region.Data.Length);
            }
        }

        [Test]
        public void SearchInt_OneResult()
        {
            InitTests();
            sigScan.Enable();

            // This sig should only be found once:
            var mySig = "A0XXXXXXXX8B0D????????F6D81BC0";
            var expected = 0x71528c;

            var result = sigScan.Scan<int>(MemoryRegionType.EXECUTE, mySig);
            Assert.AreEqual(expected, result);

            var resultArr = sigScan.ScanAll<int>(MemoryRegionType.EXECUTE, mySig);
            Assert.True(resultArr.Contains(expected));
            Assert.AreEqual(1, resultArr.Count());
        }

        [Test]
        public void SearchInt_MultipleResult()
        {
            InitTests();
            sigScan.Enable();

            // This sig should only be found once:
            var mySig = "894D??8D";
            var expected = 0x18;

            var result = sigScan.Scan<int>(MemoryRegionType.EXECUTE, mySig);
            Assert.AreEqual(expected, result);

            var resultArr = sigScan.ScanAll<int>(MemoryRegionType.EXECUTE, mySig);
            Assert.True(resultArr.Contains(expected));
            Assert.AreEqual(7, resultArr.Count());

            var freqArr = sigScan.ScanAllFrequencies<int>(MemoryRegionType.EXECUTE, mySig);
            Assert.True(freqArr.ContainsKey(expected));
            Assert.AreEqual(7, freqArr.Count());
            Assert.AreEqual(2, freqArr[0x18]);
            Assert.AreEqual(1, freqArr[0x14]);
            Assert.AreEqual(1, freqArr[0x30]);
            Assert.AreEqual(1, freqArr[0xB4]);
            Assert.AreEqual(1, freqArr[0xC8]);
            Assert.AreEqual(1, freqArr[0xC0]);
            Assert.AreEqual(1, freqArr[0xEC]);
            foreach (var r in freqArr)
                Debug.WriteLine(string.Format("{0:X}: {1}", r.Key, r.Value));
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Cannot start signature with a target address.")]
        public void SearchInt_IncorrectSignature_StartTarget()
        {
            InitTests();
            sigScan.Enable();

            // This sig should only be found once:
            var mySig = "??894D";

            sigScan.Scan<int>(MemoryRegionType.EXECUTE, mySig);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Cannot start signature with a wildcard.")]
        public void SearchInt_IncorrectSignature_StartWildcard()
        {
            InitTests();
            sigScan.Enable();

            // This sig should only be found once:
            var mySig = "XX894D??";

            sigScan.Scan<int>(MemoryRegionType.EXECUTE, mySig);
        }
    }
}
