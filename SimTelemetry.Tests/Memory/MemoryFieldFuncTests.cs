using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    [TestFixture]
    public class MemoryFieldFuncTests
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
        public void CounterField()
        {
            InitTest();

            int iCounter = 0;

            var fieldCounter = new MemoryFieldFunc<int>("Counter", (pool) =>
                                                                       {
                                                                           iCounter++;
                                                                           return iCounter;
                                                                       });
            drvPool.Add(fieldCounter);
            memory.Refresh();

            Assert.AreEqual(1, drvPool.ReadAs<int>("Counter"));
            Assert.AreEqual(2, drvPool.ReadAs<int>("Counter"));
            Assert.AreEqual(3, drvPool.ReadAs<int>("Counter"));
            Assert.AreEqual(4, drvPool.ReadAs<int>("Counter"));
            Assert.AreEqual(5, drvPool.ReadAs<int>("Counter"));
            Assert.AreEqual(6, drvPool.ReadAs<int>("Counter"));
            Assert.AreEqual(7, drvPool.ReadAs<int>("Counter"));
        }

        [Test]
        public void Basic()
        {
            InitTest();

            var fieldBool = new MemoryFieldFunc<bool>("BoolTrue", (pool) => true);
            var testField = new MemoryFieldFunc<int>("IsItCorrect", (pool) =>
                                                                        {
                                                                            Assert.AreEqual(drvPool, pool);
                                                                            return 1337;
                                                                        });

            Assert.AreEqual("IsItCorrect", testField.Name);
            Assert.AreEqual(0, testField.Address);
            Assert.AreEqual(0, testField.Size);
            Assert.AreEqual(0, testField.Offset);

            Assert.False(testField.IsConstant);
            Assert.False(testField.IsDynamic);
            Assert.False(testField.IsStatic);

            drvPool.Add(fieldBool);
            drvPool.Add(testField);

            memory.Refresh();

            Assert.AreEqual(0, actionLogbook.Count);

            Assert.True(drvPool.ReadAs<bool>("BoolTrue"));
            Assert.AreEqual(1337, drvPool.ReadAs<int>("IsItCorrect"));

            // func() are always 'changing'
            Assert.True(fieldBool.HasChanged());
            Assert.True(testField.HasChanged());

            Assert.True(fieldBool.HasChanged());
            Assert.True(testField.HasChanged());

            Assert.True(fieldBool.HasChanged());
            Assert.True(testField.HasChanged());
        }
    }
}