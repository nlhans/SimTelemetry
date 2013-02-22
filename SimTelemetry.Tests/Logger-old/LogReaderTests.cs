using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain.LoggerO;

namespace SimTelemetry.Tests.LoggerO
{
    [TestFixture]
    class LogReaderTests
    {
        private LogWriterTests _logWriter;
        private LogFile LogFileTests;
        [TestFixtureSetUp]
        public void CreateDataFile()
        {
            _logWriter = new LogWriterTests();

            //create us a log file
            _logWriter.BinaryTests();

            // create our own logfile Reader
            LogFileTests = new LogFile("temp.zip");

        }

        [Test]
        public void TestStructure()
        {
            Assert.AreNotEqual(_logWriter, null);

            Assert.AreEqual(1, LogFileTests.Groups.Count());
            Assert.AreEqual(0, LogFileTests.Fields.Count());

            var myGroup = LogFileTests.Groups.FirstOrDefault();
            var myGroupById = LogFileTests.FindGroup(1);
            var myGroupId = LogFileTests.GetGroupId("My Group");
            Assert.AreEqual(myGroupId, myGroupById.ID);
            Assert.AreEqual(myGroupById, myGroup);
            Assert.AreEqual("My Group", myGroup.Name);
            Assert.AreEqual(1, myGroup.ID);
            Assert.AreEqual(LogFileTests, myGroup.Master);
            Assert.AreEqual(LogFileTests, myGroup.File);

            Assert.AreEqual(2, myGroup.Fields.Count());
            Assert.AreEqual(1, myGroup.Groups.Count());

            // myFloat
            var myFloatFieldById = LogFileTests.FindField(1);
            var floatFieldId = LogFileTests.GetFieldId("My Group", "myFloat");
            var myFloatField = myGroup.Fields.Where(x => x.ID == 1).FirstOrDefault();

            Assert.AreEqual(floatFieldId, myFloatFieldById.ID);
            Assert.AreEqual(myFloatField, myFloatFieldById);

            Assert.AreEqual(1, myFloatField.ID);
            Assert.AreEqual(typeof(float), myFloatField.ValueType);
            Assert.AreEqual(myGroup, myFloatField.Group);
            Assert.AreEqual("myFloat", myFloatField.Name);

            // myString
            var myStringFieldById = LogFileTests.FindField(2);
            var stringFieldId = LogFileTests.GetFieldId("My Group", "myString");
            var myStringField = myGroup.Fields.Where(x => x.ID == 2).FirstOrDefault();

            Assert.AreEqual(stringFieldId, myStringFieldById.ID);
            Assert.AreEqual(myStringField, myStringFieldById);

            Assert.AreEqual(2, myStringField.ID);
            Assert.AreEqual(typeof(string), myStringField.ValueType);
            Assert.AreEqual(myGroup, myStringField.Group);
            Assert.AreEqual("myString", myStringField.Name);

            // subGroup
            var mySubGroupById = LogFileTests.FindGroup(2);
            var mySubGroupId = LogFileTests.GetGroupId("My Subgroup");
            var mySubGroup = myGroup.Groups.Where(x => x.ID == 2).FirstOrDefault();

            Assert.AreEqual(mySubGroup.ID, mySubGroupId);
            Assert.AreEqual(mySubGroupById, mySubGroup);

            Assert.AreEqual(2, mySubGroup.ID);
            Assert.AreEqual(1, mySubGroup.Fields.Count());
            Assert.AreEqual(0, mySubGroup.Groups.Count());
            Assert.AreEqual(myGroup, mySubGroup.Master);
            Assert.AreEqual(LogFileTests, mySubGroup.File);
            Assert.AreEqual("My Subgroup", mySubGroup.Name);

            // myDouble
            var myDoubleFieldById = LogFileTests.FindField(3);
            var doubleFieldId = LogFileTests.GetFieldId("My Subgroup", "myDouble");
            var myDoubleField = mySubGroup.Fields.Where(x => x.ID == 3).FirstOrDefault();

            Assert.AreEqual(doubleFieldId, myDoubleFieldById.ID);
            Assert.AreEqual(myDoubleField, myDoubleFieldById);

            Assert.AreEqual(3, myDoubleField.ID);
            Assert.AreEqual(typeof(double), myDoubleField.ValueType);
            Assert.AreEqual(mySubGroup, myDoubleField.Group);
            Assert.AreEqual("myDouble", myDoubleField.Name);

        }

        [Test]
        public void TestTime()
        {
            // 2 data files:
            Assert.AreEqual(2, LogFileTests.Time.Count());

            var lastTime = 0;
            var lastOffset = 0;
            var mySwitchpoint = 0;
            foreach (var timeDict in LogFileTests.Time)
            {
                foreach(var timeKVP in timeDict)
                {
                    Assert.GreaterOrEqual(timeKVP.Key, lastTime);
                    Assert.GreaterOrEqual(timeKVP.Value, lastOffset);

                    lastTime = timeKVP.Key;
                    lastOffset = timeKVP.Value;

                }
                lastOffset = 0;
                if(mySwitchpoint == 0)
                    mySwitchpoint = lastTime+40; // next sample is in the next data file, so this one is lagging by 1tick(=40ms)
            }

            Assert.AreEqual(mySwitchpoint, _logWriter.switchPoint);
        }

        [Test]
        public void TestData()
        {
            var timeline = LogFileTests.Timeline.ToList();
            Assert.AreEqual(timeline.Count, _logWriter.testDataFrames);
            var floatData = _logWriter.GetFloatData();
            var doubleData = _logWriter.GetDoubleData();
            var stringData = _logWriter.GetStringData();

            var myFloat1 = LogFileTests.ReadAs<float>("My Group", "myFloat", timeline.FirstOrDefault());
            Assert.AreEqual(myFloat1, floatData[0]);

            Stopwatch w = new Stopwatch();
            w.Start();
            for (int i = 0; i < floatData.Length; i++)
            {
                Assert.AreEqual(LogFileTests.ReadAs<float>("My Group", "myFloat", timeline[i]), floatData[i]);
                //Assert.AreEqual(logFile.ReadAs<double>("My Group", "myFloat", timeline[i]), (double)floatData[i]);
                Assert.AreEqual(LogFileTests.ReadAs<double>("My Subgroup", "myDouble", timeline[i]), doubleData[i]);

                // Test string; this only occurs 1/10 sample time.
                // It should look up the last written string.
                string lastString = stringData[i/10];
                string foundString = LogFileTests.ReadAs<string>("My Group", "myString", timeline[i]);
                //Assert.AreEqual(lastString, foundString);

            }
            w.Stop();

            Debug.WriteLine("Reading 3x" + floatData.Length + " floats from file took " + w.ElapsedMilliseconds + " ("+w.ElapsedMilliseconds*1000.0/floatData.Length/2+"us per read)");
        }
    }
}