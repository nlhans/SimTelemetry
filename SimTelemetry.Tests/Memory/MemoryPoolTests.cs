using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    [TestFixture]
    public class MemoryPoolTests
    {
        private DiagnosticMemoryReader reader;
        private List<MemoryReadAction> actionLogbook;

        public void InitTest()
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

        }

        [Test]
        public void PoolTemplate()
        {
            InitTest();

            var driversPool = new MemoryPool("Drivers", MemoryAddress.Static, 0x315298, 0x200);
            driversPool.SetTemplate(true);

            var provider = new MemoryProvider(reader);
            provider.Add(driversPool);
            provider.Refresh();

            Assert.AreEqual("Drivers", driversPool.Name);
            Assert.AreEqual(MemoryAddress.Static, driversPool.AddressType);
            Assert.AreEqual(0x315298, driversPool.Address);
            Assert.AreEqual(0x200, driversPool.Size);
            Assert.AreEqual(0, driversPool.Offset);
            Assert.False(driversPool.IsDynamic);
            Assert.False(driversPool.IsConstant);
            Assert.True(driversPool.IsStatic);
            Assert.False(driversPool.IsSignature);
            Assert.True(driversPool.IsTemplate);

            Assert.AreEqual(0, actionLogbook.Count);

            for (int i = 0; i < 100; i++)
                provider.Refresh();
            Assert.AreEqual(0, actionLogbook.Count);

            driversPool.SetTemplate(false);

            provider.Refresh();

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x200, actionLogbook[0].Size);
        }

        [Test]
        public void PoolDynamic()
        {
            InitTest();

            var driversPool = new MemoryPool("Drivers", MemoryAddress.Static, 0x315298, 0x200);

            var provider = new MemoryProvider(reader);
            provider.Add(driversPool);
            provider.Refresh();

            Assert.AreEqual("Drivers", driversPool.Name);
            Assert.AreEqual(MemoryAddress.Static, driversPool.AddressType);
            Assert.AreEqual(0x315298, driversPool.Address);
            Assert.AreEqual(0x200, driversPool.Size);
            Assert.AreEqual(0, driversPool.Offset);
            Assert.False(driversPool.IsDynamic);
            Assert.False(driversPool.IsConstant);
            Assert.True(driversPool.IsStatic);
            Assert.False(driversPool.IsSignature);
            Assert.False(driversPool.IsTemplate);

            Assert.AreEqual(1, actionLogbook.Count);

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x200, actionLogbook[0].Size);

            var driver1 = new MemoryPool("Driver1", MemoryAddress.Dynamic, driversPool, 0x0, 0x6000);
            var driver2 = new MemoryPool("Driver2", MemoryAddress.Dynamic, driversPool, 0x4, 0x6000);

            provider.Add(driver1);
            provider.Add(driver2);
            provider.Refresh();

            Assert.AreEqual("Driver1", driver1.Name);
            Assert.AreEqual(MemoryAddress.Dynamic, driver1.AddressType);
            Assert.AreEqual(0, driver1.Address);
            Assert.AreEqual(0x6000, driver1.Size);
            Assert.AreEqual(0, driver1.Offset);
            Assert.AreEqual(driversPool, driver1.Pool);
            Assert.True(driver1.IsDynamic);
            Assert.False(driver1.IsConstant);
            Assert.False(driver1.IsStatic);
            Assert.False(driver1.IsSignature);
            Assert.False(driver1.IsTemplate);

            Assert.AreEqual("Driver2", driver2.Name);
            Assert.AreEqual(MemoryAddress.Dynamic, driver2.AddressType);
            Assert.AreEqual(0, driver2.Address);
            Assert.AreEqual(0x6000, driver2.Size);
            Assert.AreEqual(4, driver2.Offset);
            Assert.AreEqual(driversPool, driver2.Pool);
            Assert.True(driver2.IsDynamic);
            Assert.False(driver2.IsConstant);
            Assert.False(driver2.IsStatic);
            Assert.False(driver2.IsSignature);
            Assert.False(driver2.IsTemplate);

            Assert.AreEqual(4, actionLogbook.Count);

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x200, actionLogbook[0].Size);
            Assert.AreEqual(0x715298, actionLogbook[1].Address);
            Assert.AreEqual(0x200, actionLogbook[1].Size);
            Assert.AreEqual(0x7154c0, actionLogbook[2].Address);
            Assert.AreEqual(0x6000, actionLogbook[2].Size);
            Assert.AreEqual(0x71b408, actionLogbook[3].Address);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);

        }

        [Test]
        public void PoolContainer()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, 0, 0, 0);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Refresh();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0, p.Address);
            Assert.AreEqual(0, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsSignature);
            Assert.False(p.IsTemplate);

            Assert.AreEqual(0, actionLogbook.Count);
        }

        [Test]
        public void PoolContainerNonZeroSize()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, 0, 0, 4);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Refresh();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0, p.Address);
            Assert.AreEqual(4, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsSignature);
            Assert.False(p.IsTemplate);

            Assert.AreEqual(1, actionLogbook.Count);

            Assert.AreEqual(0, actionLogbook[0].Address);
            Assert.AreEqual(4, actionLogbook[0].Size);
        }

        [Test]
        public void PoolStaticAbsolute()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, 0x7154c0, 0x6000);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Refresh();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0x7154c0, p.Address);
            Assert.AreEqual(0x6000, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsSignature);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(1, p.AddressTree.Length);
            Assert.AreEqual(0x7154C0, p.AddressTree[0]);

            Assert.AreEqual(1, actionLogbook.Count);

            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);

            provider.Refresh();

            Assert.AreEqual(2, actionLogbook.Count);

            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);

        }

        [Test]
        public void PoolStatic()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.Static, 0x3154c0, 0x6000);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Refresh();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.Static, p.AddressType);
            Assert.AreEqual(0x3154c0, p.Address);
            Assert.AreEqual(0x6000, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(1, p.AddressTree.Length);
            Assert.AreEqual(0x7154C0, p.AddressTree[0]);

            Assert.AreEqual(1, actionLogbook.Count);

            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);

            provider.Refresh();

            Assert.AreEqual(2, actionLogbook.Count);

            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);

        }

        [Test]
        public void PoolStaticAbsolutePointer()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, 0x715298, new[] { 0 }, 0x6000);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Refresh();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0x715298, p.Address);
            Assert.AreEqual(0x6000, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.AreEqual(1, p.Pointers.Count());
            Assert.AreEqual(0, p.Pointers.FirstOrDefault().Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsSignature);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(2, p.AddressTree.Length);
            Assert.AreEqual(0x715298, p.AddressTree[0]);
            Assert.AreEqual(0x7154C0, p.AddressTree[1]);

            Assert.AreEqual(2, actionLogbook.Count);

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);

            Assert.AreEqual(0x4, actionLogbook[0].Size);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);

            // re-read
            // it will follow the pointer every refresh cycle.
            provider.Refresh();

            Assert.AreEqual(4, actionLogbook.Count);

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);
            Assert.AreEqual(0x715298, actionLogbook[2].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[3].Address);

            Assert.AreEqual(0x4, actionLogbook[0].Size);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);
            Assert.AreEqual(0x4, actionLogbook[2].Size);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);
        }

        [Test]
        public void PoolStaticAbsolutePointerSignature()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, 0x715298, 
                                   new[]
                                       {
                                           new MemoryFieldSignaturePointer(0, false),
                                           new MemoryFieldSignaturePointer("7CD5D9XX????????518BCFD91C24E8", true),
                                       }, 0x4);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Scanner.Enable();
            provider.Refresh();
            provider.Scanner.Disable();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0x715298, p.Address);
            Assert.AreEqual(0x4, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.AreEqual(2, p.Pointers.Count());
            Assert.AreEqual(0, p.Pointers.FirstOrDefault().Offset);
            Assert.AreEqual(0x317C, p.Pointers.ToList()[1].Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsSignature);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(3, p.AddressTree.Length);
            Assert.AreEqual(0x715298, p.AddressTree[0]);
            Assert.AreEqual(0x7154C0, p.AddressTree[1]);
            Assert.AreEqual(0x71863C, p.AddressTree[2]);

            Assert.Greater(actionLogbook.Count, 2);
            int preLogBookSize = actionLogbook.Count;

            Assert.AreEqual(0x715298, actionLogbook[actionLogbook.Count-2].Address);
            Assert.AreEqual(0x71863C, actionLogbook[actionLogbook.Count-1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);

            // re-read
            // it will follow the pointer every refresh cycle.
            provider.Refresh();
            int postLogBookSize = actionLogbook.Count;
            Assert.AreEqual(postLogBookSize - preLogBookSize, 2);

            Assert.AreEqual(0x715298, actionLogbook[actionLogbook.Count - 4].Address);
            Assert.AreEqual(0x71863C, actionLogbook[actionLogbook.Count - 3].Address);
            Assert.AreEqual(0x715298, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x71863C, actionLogbook[actionLogbook.Count - 1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);
        }

        [Test]
        public void PoolStaticPointer()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.Static, 0x315298, new[] { 0 }, 0x6000);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Refresh();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.Static, p.AddressType);
            Assert.AreEqual(0x315298, p.Address);
            Assert.AreEqual(0x6000, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.AreEqual(1, p.Pointers.Count());
            Assert.AreEqual(0, p.Pointers.FirstOrDefault().Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsStatic);
            Assert.False(p.IsSignature);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(2, p.AddressTree.Length);
            Assert.AreEqual(0x715298, p.AddressTree[0]);
            Assert.AreEqual(0x7154C0, p.AddressTree[1]);

            Assert.AreEqual(2, actionLogbook.Count);

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);

            Assert.AreEqual(0x4, actionLogbook[0].Size);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);

            // re-read
            // it will follow the pointer every refresh cycle.
            provider.Refresh();

            Assert.AreEqual(4, actionLogbook.Count);

            Assert.AreEqual(0x715298, actionLogbook[0].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);
            Assert.AreEqual(0x715298, actionLogbook[2].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[3].Address);

            Assert.AreEqual(0x4, actionLogbook[0].Size);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);
            Assert.AreEqual(0x4, actionLogbook[2].Size);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);
        }

        [Test]
        public void PoolSignature()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, "A0XXXXXXXX8B0D????????F6D81BC0", 0x4);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Scanner.Enable();
            provider.Refresh();
            provider.Scanner.Disable();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0x71528C, p.Address);
            Assert.AreEqual(0x4, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.AreEqual(0, p.Pointers.Count());
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsSignature);
            Assert.True(p.IsStatic);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(1, p.AddressTree.Length);
            Assert.AreEqual(0x71528C, p.AddressTree[0]);


            Assert.Greater(actionLogbook.Count, 1);
            var preRefreshCount = actionLogbook.Count;

            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 1].Address);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);

            // re-read
            // it will only read what was initially found by the signature.
            provider.Refresh();

            var postRefreshCount = actionLogbook.Count;
            Assert.AreEqual(1, postRefreshCount - preRefreshCount);

            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);
        }

        [Test]
        public void PoolSignaturePointer()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute, "A0XXXXXXXX8B0D????????F6D81BC0", new[] { 0 }, 0x6000);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Scanner.Enable();
            provider.Refresh();
            provider.Scanner.Disable();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0x71528C, p.Address);
            Assert.AreEqual(0x6000, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.AreEqual(1, p.Pointers.Count());
            Assert.AreEqual(0, p.Pointers.FirstOrDefault().Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsSignature);
            Assert.True(p.IsStatic);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(2, p.AddressTree.Length);
            Assert.AreEqual(0x71528C, p.AddressTree[0]);
            Assert.AreEqual(0x7154C0, p.AddressTree[1]);


            Assert.Greater(actionLogbook.Count, 2);
            var preRefreshCount = actionLogbook.Count;

            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);

            // re-read
            // it will only read what was initially found by the signature.
            provider.Refresh();

            var postRefreshCount = actionLogbook.Count;
            Assert.AreEqual(2, postRefreshCount - preRefreshCount);

            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 4].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 3].Address);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);
        }

        [Test]
        public void PoolSignaturePointerWithSignature()
        {
            InitTest();

            var p = new MemoryPool("MyPool", MemoryAddress.StaticAbsolute,
                                   "A0XXXXXXXX8B0D????????F6D81BC0",
                                   new[]
                                       {
                                           new MemoryFieldSignaturePointer(0, false),
                                           new MemoryFieldSignaturePointer("7CD5D9XX????????518BCFD91C24E8", true),
                                       }, 0x4);

            var provider = new MemoryProvider(reader);
            provider.Add(p);
            provider.Scanner.Enable();
            provider.Refresh();
            provider.Scanner.Disable();

            Assert.AreEqual("MyPool", p.Name);
            Assert.AreEqual(MemoryAddress.StaticAbsolute, p.AddressType);
            Assert.AreEqual(0x71528C, p.Address);
            Assert.AreEqual(0x4, p.Size);
            Assert.AreEqual(0, p.Offset);
            Assert.AreEqual(2, p.Pointers.Count());
            Assert.AreEqual(0, p.Pointers.FirstOrDefault().Offset);
            Assert.AreEqual(0x317C, p.Pointers.ToList()[1].Offset);
            Assert.False(p.IsDynamic);
            Assert.False(p.IsConstant);
            Assert.True(p.IsSignature);
            Assert.True(p.IsStatic);
            Assert.False(p.IsTemplate);
            Assert.AreEqual(3, p.AddressTree.Length);
            Assert.AreEqual(0x71528C, p.AddressTree[0]);
            Assert.AreEqual(0x7154c0, p.AddressTree[1]);
            Assert.AreEqual(0x71863C, p.AddressTree[2]);

            Assert.Greater(actionLogbook.Count, 2);
            var preRefreshCount = actionLogbook.Count;

            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x71863C, actionLogbook[actionLogbook.Count - 1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);

            // re-read
            // it will only read what was initially found by the signatures.
            provider.Refresh();

            var postRefreshCount = actionLogbook.Count;
            Assert.AreEqual(2, postRefreshCount - preRefreshCount);

            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 4].Address);
            Assert.AreEqual(0x71863C, actionLogbook[actionLogbook.Count - 3].Address);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x71863C, actionLogbook[actionLogbook.Count - 1].Address);

            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);

        }

    }
}
