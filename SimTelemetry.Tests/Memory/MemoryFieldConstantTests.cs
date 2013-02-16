using System.Diagnostics;
using System.Collections.Generic;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    [TestFixture]
    public class MemoryFieldConstantTests
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

            drvPool = new MemoryPool("Test", MemoryAddress.StaticAbsolute, 0, 0);
            memory.Add(drvPool);
        }

        [Test]
        public void Basic()
        {
            InitTest();

            var testField = new MemoryFieldConstant<bool>("Test", false);

            drvPool.Add(testField);

            memory.Refresh();

            Assert.AreEqual("Test", testField.Name);
            Assert.AreEqual(0, testField.Address);
            Assert.AreEqual(0, testField.Size);
            Assert.AreEqual(0, testField.Offset);

            Assert.True(testField.IsConstant);
            Assert.False(testField.IsDynamic);
            Assert.False(testField.IsStatic);

            Assert.True(testField.HasChanged());
            Assert.False(testField.HasChanged());
        }

        [Test]
        public void ConstantField()
        {
            InitTest();

            var boolTrue = new MemoryFieldConstant<bool>("BoolTrue", true);
            var boolFalse = new MemoryFieldConstant<bool>("BoolFalse", false);

            drvPool.Add(boolTrue);
            drvPool.Add(boolFalse);

            memory.Refresh();

            Assert.AreEqual(0, actionLogbook.Count);

            Assert.True(drvPool.ReadAs<bool>("BoolTrue"));
            Assert.False(drvPool.ReadAs<bool>("BoolFalse"));

        }
    }
}