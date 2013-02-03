using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Tests.Memory
{
    public class DiagnosticMemoryReader : MemoryReader
    {
        public override bool Read(int memoryAddress, byte[] buffer)
        {
            GlobalEvents.Fire(new MemoryReadAction(memoryAddress, buffer.Length), true);
            return base.Read(memoryAddress, buffer);
        }
        public override bool Read(IntPtr memoryAddress, byte[] buffer)
        {
            GlobalEvents.Fire(new MemoryReadAction(memoryAddress.ToInt32(), buffer.Length), true);
            return base.Read(memoryAddress, buffer);
        }

        public override byte[] Read(IntPtr memoryAddress, uint bytesToRead)
        {
            GlobalEvents.Fire(new MemoryReadAction(memoryAddress.ToInt32(), (int)bytesToRead), true);
            return base.Read(memoryAddress, bytesToRead);
        }
    }

    public class MemoryReadAction
    {
        public int Address;
        public int Size;

        public MemoryReadAction(int address, int size)
        {
            Address = address;
            Size = size;
        }
    }

    [TestFixture]
    public class MemoryPoolTests
    {
        [Test]
        public void Basic()
        {
            List<MemoryReadAction> actionLogbook = new List<MemoryReadAction>();
            GlobalEvents.Hook<MemoryReadAction>(x =>
                                                    {
                                                        actionLogbook.Add(x);
                                                        Debug.WriteLine(string.Format("Reading 0x{0:X}[0x{1:X}]", x.Address, x.Size));
                                                    }
                                                , true);
            var reader = new DiagnosticMemoryReader();
            var prs = Process.GetProcessesByName("flux");
            if(prs.Length == 0) Assert.Fail("Run the application to test memoryreader.");
            reader.Open(prs[0]);
            var provider = new MemoryProvider(reader);
            var pool = new MemoryPool("Test", MemoryAddress.StaticAbsolute, 0x500000, 0, 0x1500);
            provider.Add(pool);
            provider.Refresh();

            Assert.AreEqual(1, actionLogbook.Count);
        }
    }
}
