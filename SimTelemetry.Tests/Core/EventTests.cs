using System;
using NUnit.Framework;
using SimTelemetry.Core;

namespace SimTelemetry.Tests.Core
{
    public class TestEvent1Handler
    {
        public string Message1 { get; set; }
        public TestEvent1Handler(string message)
        {
            Message1 = message;
        }
    }

    public class TestEvent2Handler
    {
        public string Message2 { get; set; }
        public TestEvent2Handler(string message)
        {
            Message2 = message;
        }
    }

    [TestFixture]
    class EventTests : IDisposable
    {
        public void Initialize()
        {
            //
            _event1HandleCount = 0;
            _event2HandleCount = 0;
        }

        public void Dispose()
        {
            Events.Reset();
            Assert.AreEqual(0, Events.Count);
        }

        private int _event1HandleCount;
        private int _event2HandleCount;

        public void handleEvent1(TestEvent1Handler data)
        {
            Assert.AreEqual(data.Message1, "Hello event 1");
            _event1HandleCount++;
        }

        public void handleEvent2(TestEvent2Handler data)
        {
            Assert.AreEqual(data.Message2, "Hello event 2");
            _event2HandleCount++;
        }
        
        [Test]
        public void SimpleEventTester()
        {
            Initialize();
            Assert.AreEqual(0, Events.Count);

            // Add two simple events:
            Events.Hook<TestEvent1Handler>(handleEvent1, true);
            Events.Hook<TestEvent2Handler>(handleEvent2, false);

            Assert.AreEqual(2, Events.Count);

            // Cannot handle 1 event twice, even though network listening is different:7
            Events.Hook<TestEvent2Handler>(handleEvent2, true);
            Assert.AreEqual(2, Events.Count);

            // Fire event 1 for network:
            Events.Fire(new TestEvent1Handler("Hello event 1"), true);
            Assert.AreEqual(1, _event1HandleCount);

            // Fire event 2 for network:
            Events.Fire(new TestEvent2Handler("Hello event 2"), true);
            Assert.AreEqual(1, _event2HandleCount);

            // Fire event 1 for local:
            Events.Fire(new TestEvent1Handler("Hello event 1"), false);
            Assert.AreEqual(1, _event1HandleCount);

            // Fire event 2 for local:
            Events.Fire(new TestEvent2Handler("Hello event 2"), false);
            Assert.AreEqual(2, _event2HandleCount);

            // Unhook
            Events.Unhook<TestEvent1Handler>(handleEvent1);
            Events.Unhook<TestEvent2Handler>(handleEvent2);

            // Count
            Assert.AreEqual(0, Events.Count);

            // Fire event 1 for network, must remain unchanged
            Events.Fire(new TestEvent1Handler("Hello event 1"), true);
            Assert.AreEqual(1, _event1HandleCount);

            // Fire event 2 for network, must remain unchanged
            Events.Fire(new TestEvent2Handler("Hello event 2"), true);
            Assert.AreEqual(2, _event2HandleCount);
        }
    }
}
