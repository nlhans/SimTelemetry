using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    [TestFixture]
    public class MemoryFieldSignatureTests
    {
        private DiagnosticMemoryReader reader;
        private MemoryPool drvPool;
        private MemoryProvider memory;
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

            memory = new MemoryProvider(reader);

            drvPool = new MemoryPool("Test", MemoryAddress.StaticAbsolute, 0x7154C0, 0x6000);
            memory.Add(drvPool);
        }

        [Test]
        public void FieldStaticAbsoluteSignature()
        {
            InitTest();

            var plrMemoryPtr = new MemoryFieldSignature<int>("Player", MemoryAddress.StaticAbsolute,
                                                            "A0XXXXXXXX8B0D????????F6D81BC0", new[] { 0 }, 0x6000);

            drvPool.Add(plrMemoryPtr);

            memory.Scanner.Enable();
            memory.Refresh();
            memory.Scanner.Disable();

            Assert.True(plrMemoryPtr.HasChanged());

            int preRescanCount = actionLogbook.Count;

            Assert.Greater(actionLogbook.Count, 1);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 3].Address); // pool itself
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address); // our sig ptr scan
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address); // our sig
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);

            Assert.AreEqual(0x7154C0, drvPool.ReadAs<int>("Player"));
            Assert.AreEqual(0x7154C0, plrMemoryPtr.Address);

            Assert.AreEqual(actionLogbook.Count, preRescanCount);

            // It will only refresh the contents of the pointer
            memory.Refresh();

            Assert.False(plrMemoryPtr.HasChanged());

            int postRescanCount = actionLogbook.Count;
            Assert.AreEqual(2, postRescanCount - preRescanCount);

            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 5].Address); // pool itself
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 5].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 4].Address); // our sig ptr scan
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 3].Address); // our sig
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);

            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 2].Address); // pool itself
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address); // our result from sig
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);

        }

        [Test]
        public void FieldStaticAbsoluteSignatureConversion()
        {
            int calledConversion = 0;

            InitTest();

            var plrMemoryPtr = new MemoryFieldSignature<int>("Player", MemoryAddress.StaticAbsolute,
                                                             "A0XXXXXXXX8B0D????????F6D81BC0", new[] {0}, 0x6000,
                                                             (x) =>
                                                                 {
                                                                     calledConversion++;
                                                                     return x;
                                                                 });

            drvPool.Add(plrMemoryPtr);

            memory.Scanner.Enable();
            memory.Refresh();
            memory.Scanner.Disable();

            Assert.True(plrMemoryPtr.HasChanged());
            Assert.AreEqual(1, calledConversion);

            int preRescanCount = actionLogbook.Count;
            Assert.Greater(actionLogbook.Count, 1);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 3].Address); // pool itself
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address); // our sig ptr scan
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address); // our sig
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);

            Assert.AreEqual(0x7154C0, drvPool.ReadAs<int>("Player"));
            Assert.AreEqual(0x7154C0, plrMemoryPtr.Address);

            Assert.AreEqual(actionLogbook.Count, preRescanCount);

            // It will only refresh the contents of the pointer
            memory.Refresh();

            Assert.False(plrMemoryPtr.HasChanged());
            Assert.AreEqual(2, calledConversion);

            int postRescanCount = actionLogbook.Count;
            Assert.AreEqual(2, postRescanCount - preRescanCount);

            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 5].Address); // pool itself
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 5].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 4].Address); // our sig ptr scan
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 3].Address); // our sig
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);

            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 2].Address); // pool itself
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address); // our result from sig
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);

        }

        [Test]
        public void FieldDynamicSignature()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryFieldSignature<int>("Index", MemoryAddress.Dynamic,
                                                           "8B0C85XXXXXXXX8B56??3B91XXXXXXXX",
                                                           new[] { new MemoryFieldSignaturePointer(4, true) }, 4);
            // sig is +0x4, add additional +0x4 ->> +0x8 = driver index
            var FieldFuel = new MemoryFieldSignature<float>("FuelCapacity", MemoryAddress.Dynamic,
                                                            "DC86XXXXXXXXD993????????D9E8D9C0",
                                                            new[] { new MemoryFieldSignaturePointer(4, true) }, 4);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);
            memory.Scanner.Enable();
            memory.Refresh();
            memory.Scanner.Disable();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            int preRescanCount = actionLogbook.Count;
            Assert.Greater(actionLogbook.Count, 1);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);


            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            int postRescanCount = actionLogbook.Count;
            Assert.AreEqual(1, postRescanCount - preRescanCount);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);
        }

        [Test]
        public void FieldDynamicSignatureConversion()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryFieldSignature<int>("Index", MemoryAddress.Dynamic,
                                                           "8B0C85XXXXXXXX8B56??3B91XXXXXXXX",
                                                           new[] { new MemoryFieldSignaturePointer(4, true) }, 4);
            // sig is +0x4, add additional +0x4 ->> +0x8 = driver index
            var FieldFuel = new MemoryFieldSignature<float>("FuelCapacity", MemoryAddress.Dynamic,
                                                            "DC86XXXXXXXXD993????????D9E8D9C0",
                                                            new[] { new MemoryFieldSignaturePointer(4, true) }, 4, (x) => 2.0f * x);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);
            memory.Scanner.Enable();
            memory.Refresh();
            memory.Scanner.Disable();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            int preRescanCount = actionLogbook.Count;
            Assert.Greater(actionLogbook.Count, 1);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);


            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(2.0f * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            int postRescanCount = actionLogbook.Count;
            Assert.AreEqual(1, postRescanCount - preRescanCount);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 2].Address);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[actionLogbook.Count - 1].Address);
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 1].Size);
        }

        [Test]
        public void FieldStaticAbsoluteSignatureWithSignaturePointer()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryFieldSignature<int>("Index", MemoryAddress.StaticAbsolute,
                                                            "A0XXXXXXXX8B0D????????F6D81BC0", // player ptr
                                                           new[]
                                                               {
                                                                   new MemoryFieldSignaturePointer(0, false), 
                                                                   new MemoryFieldSignaturePointer("8B0C85XXXXXXXX8B56??3B91XXXXXXXX", true),
                                                                   new MemoryFieldSignaturePointer(4, true)
                                                               }, 4);
            // sig is +0x4, add additional +0x4 ->> +0x8 = driver index
            var FieldFuel = new MemoryFieldSignature<float>("FuelCapacity", MemoryAddress.StaticAbsolute,
                                                             "A0XXXXXXXX8B0D????????F6D81BC0",  // player ptr
                                                            new[]
                                                                {
                                                                   new MemoryFieldSignaturePointer(0, false), 
                                                                    new MemoryFieldSignaturePointer("DC86XXXXXXXXD993????????D9E8D9C0", true),
                                                                   new MemoryFieldSignaturePointer(4, true)
                                                                }, 4);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);
            memory.Scanner.Enable();
            memory.Refresh();
            memory.Scanner.Disable();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            int preRescanCount = actionLogbook.Count;
            Assert.Greater(actionLogbook.Count, 1);
            Assert.AreEqual(0x7154c0, actionLogbook[actionLogbook.Count - 5].Address); // pool
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 5].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 4].Address); // player ptr - index
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x7154C8, actionLogbook[actionLogbook.Count - 3].Address); // index
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address); // player ptr - fuel capacity
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x718620, actionLogbook[actionLogbook.Count - 1].Address); // fuel capacity
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);


            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            int postRescanCount = actionLogbook.Count;
            Assert.AreEqual(3, postRescanCount - preRescanCount);

            Assert.AreEqual(0x7154c0, actionLogbook[actionLogbook.Count - 3].Address); // pool 
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x7154C8, actionLogbook[actionLogbook.Count - 2].Address); // index
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x718620, actionLogbook[actionLogbook.Count - 1].Address); // fuel capacity
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);
        }

        [Test]
        public void FieldStaticAbsoluteSignatureWithSignaturePointerConversion()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryFieldSignature<int>("Index", MemoryAddress.StaticAbsolute,
                                                            "A0XXXXXXXX8B0D????????F6D81BC0", // player ptr
                                                           new[]
                                                               {
                                                                   new MemoryFieldSignaturePointer(0, false), 
                                                                   new MemoryFieldSignaturePointer("8B0C85XXXXXXXX8B56??3B91XXXXXXXX", true),
                                                                   new MemoryFieldSignaturePointer(4, true)
                                                               }, 4);
            // sig is +0x4, add additional +0x4 ->> +0x8 = driver index
            var FieldFuel = new MemoryFieldSignature<float>("FuelCapacity", MemoryAddress.StaticAbsolute,
                                                             "A0XXXXXXXX8B0D????????F6D81BC0",  // player ptr
                                                            new[]
                                                                {
                                                                   new MemoryFieldSignaturePointer(0, false), 
                                                                    new MemoryFieldSignaturePointer("DC86XXXXXXXXD993????????D9E8D9C0", true),
                                                                   new MemoryFieldSignaturePointer(4, true)
                                                                }, 4, (x) => 2.0f * x);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);
            memory.Scanner.Enable();
            memory.Refresh();
            memory.Scanner.Disable();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            int preRescanCount = actionLogbook.Count;
            Assert.Greater(actionLogbook.Count, 1);
            Assert.AreEqual(0x7154c0, actionLogbook[actionLogbook.Count - 5].Address); // pool
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 5].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 4].Address); // player ptr - index
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 4].Size);
            Assert.AreEqual(0x7154C8, actionLogbook[actionLogbook.Count - 3].Address); // index
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x71528C, actionLogbook[actionLogbook.Count - 2].Address); // player ptr - fuel capacity
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x718620, actionLogbook[actionLogbook.Count - 1].Address); // fuel capacity
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);


            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(2.0f * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            int postRescanCount = actionLogbook.Count;
            Assert.AreEqual(3, postRescanCount - preRescanCount);

            Assert.AreEqual(0x7154c0, actionLogbook[actionLogbook.Count - 3].Address); // pool 
            Assert.AreEqual(0x6000, actionLogbook[actionLogbook.Count - 3].Size);
            Assert.AreEqual(0x7154C8, actionLogbook[actionLogbook.Count - 2].Address); // index
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 2].Size);
            Assert.AreEqual(0x718620, actionLogbook[actionLogbook.Count - 1].Address); // fuel capacity
            Assert.AreEqual(0x4, actionLogbook[actionLogbook.Count - 1].Size);
        }

    }
}