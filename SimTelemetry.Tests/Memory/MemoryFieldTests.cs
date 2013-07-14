using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    // test data for these tests are from a Lister Storm EOAA GT

    [TestFixture]
    public class MemoryFieldTests
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
        public void FieldDynamic()
        {
            InitTest();

            var FieldFuel = new MemoryField<float>("FuelCapacity", MemoryAddress.Dynamic, 0, 0x3160, 4);
            var FieldIndex = new MemoryField<int>("Index", MemoryAddress.Dynamic, 0, 0x8, 4); // +0x8 = driver index
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);

            memory.Refresh();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            Assert.AreEqual(1, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);

            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            Assert.AreEqual(2, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);
        }


        [Test]
        public void FieldDynamicConversion()
        {
            InitTest();

            var FieldFuel = new MemoryField<float>("FuelCapacity", MemoryAddress.Dynamic, 0, 0x3160, 4, (x) => x * 2.0f);
            var FieldIndex = new MemoryField<int>("Index", MemoryAddress.Dynamic, 0, 0x8, 4); // +0x8 = driver index
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);

            memory.Refresh();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            Assert.AreEqual(1, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);

            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(2.0f * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            Assert.AreEqual(2, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[1].Address);
            Assert.AreEqual(0x6000, actionLogbook[1].Size);
        }

        [Test]
        public void FieldStatic()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryField<int>("Index", MemoryAddress.Static, 0x3154c8, 0, 4); // +0x8 = driver index
            var FieldFuel = new MemoryField<float>("FuelCapacity", MemoryAddress.Static, 0x318620, 0, 4);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);

            memory.Refresh();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            Assert.AreEqual(3, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);

            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            Assert.AreEqual(6, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[3].Address);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[4].Address);
            Assert.AreEqual(0x4, actionLogbook[4].Size);
            Assert.AreEqual(0x718620, actionLogbook[5].Address);
            Assert.AreEqual(0x4, actionLogbook[5].Size);
        }

        [Test]
        public void FieldStaticConversion()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryField<int>("Index", MemoryAddress.Static, 0x3154c8, 0, 4); // +0x8 = driver index
            var FieldFuel = new MemoryField<float>("FuelCapacity", MemoryAddress.Static, 0x318620, 0, 4, (x) => x * 2.0f);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);

            memory.Refresh();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            Assert.AreEqual(3, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);

            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(2.0f * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            Assert.AreEqual(6, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[3].Address);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[4].Address);
            Assert.AreEqual(0x4, actionLogbook[4].Size);
            Assert.AreEqual(0x718620, actionLogbook[5].Address);
            Assert.AreEqual(0x4, actionLogbook[5].Size);
        }

        [Test]
        public void FieldStaticAbsolute()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryField<int>("Index", MemoryAddress.StaticAbsolute, 0x7154c8, 0, 4); // +0x8 = driver index
            var FieldFuel = new MemoryField<float>("FuelCapacity", MemoryAddress.StaticAbsolute, 0x718620, 0, 4);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);

            memory.Refresh();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            Assert.AreEqual(3, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);

            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            Assert.AreEqual(6, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[3].Address);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[4].Address);
            Assert.AreEqual(0x4, actionLogbook[4].Size);
            Assert.AreEqual(0x718620, actionLogbook[5].Address);
            Assert.AreEqual(0x4, actionLogbook[5].Size);
        }

        [Test]
        public void FieldStaticAbsoluteConversion()
        {
            InitTest();

            // test data for Lister Storm EOAA GT:
            var FieldIndex = new MemoryField<int>("Index", MemoryAddress.StaticAbsolute, 0x7154c8, 0, 4); // +0x8 = driver index
            var FieldFuel = new MemoryField<float>("FuelCapacity", MemoryAddress.StaticAbsolute, 0x718620, 0, 4, (x) => x * 2.0f);
            drvPool.Add(FieldIndex);
            drvPool.Add(FieldFuel);

            memory.Refresh();

            Assert.True(FieldIndex.HasChanged());
            Assert.True(FieldFuel.HasChanged());

            // only 1 read from pool:
            Assert.AreEqual(3, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);

            // And read some values in different types:
            Assert.AreEqual(0, drvPool.ReadAs<int>("Index"));
            Assert.AreEqual(0, drvPool.ReadAs<uint>("Index"));
            Assert.AreEqual(0.0f, drvPool.ReadAs<float>("Index"));

            Assert.AreEqual(2.0f * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<float>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<int>("FuelCapacity"));
            Assert.AreEqual(2 * TestConstants.MemoryTestFuelCapacity, drvPool.ReadAs<double>("FuelCapacity"));

            memory.Refresh();

            Assert.False(FieldIndex.HasChanged());
            Assert.False(FieldFuel.HasChanged());

            Assert.AreEqual(6, actionLogbook.Count);
            Assert.AreEqual(0x7154C0, actionLogbook[0].Address);
            Assert.AreEqual(0x6000, actionLogbook[0].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[1].Address);
            Assert.AreEqual(0x4, actionLogbook[1].Size);
            Assert.AreEqual(0x718620, actionLogbook[2].Address);
            Assert.AreEqual(0x4, actionLogbook[2].Size);
            Assert.AreEqual(0x7154C0, actionLogbook[3].Address);
            Assert.AreEqual(0x6000, actionLogbook[3].Size);
            Assert.AreEqual(0x7154c8, actionLogbook[4].Address);
            Assert.AreEqual(0x4, actionLogbook[4].Size);
            Assert.AreEqual(0x718620, actionLogbook[5].Address);
            Assert.AreEqual(0x4, actionLogbook[5].Size);
        }
    }
}