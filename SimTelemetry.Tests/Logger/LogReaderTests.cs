using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Tests.Logger
{
    [TestFixture]
    class LogReaderTests
    {
        private LogWriterTests _logWriter;
        private LogFile logFile;
        [TestFixtureSetUp]
        public void CreateDataFile()
        {
            _logWriter = new LogWriterTests();

            //create us a log file
            _logWriter.BinaryTests();

            // create our own logfile Reader
            logFile = new LogFile("temp.zip");

        }

        [Test]
        public void TestStructure()
        {
            Assert.AreNotEqual(_logWriter, null);

            Assert.AreEqual(1, logFile.Groups.Count());
            Assert.AreEqual(0, logFile.Fields.Count());

            var myGroup = logFile.Groups.FirstOrDefault();
            var myGroupById = logFile.FindGroup(1);
            var myGroupId = logFile.GetGroupId("My Group");
            Assert.AreEqual(myGroupId, myGroupById.ID);
            Assert.AreEqual(myGroupById, myGroup);
            Assert.AreEqual("My Group", myGroup.Name);
            Assert.AreEqual(1, myGroup.ID);
            Assert.AreEqual(logFile, myGroup.Master);
            Assert.AreEqual(logFile, myGroup.File);

            Assert.AreEqual(2, myGroup.Fields.Count());
            Assert.AreEqual(1, myGroup.Groups.Count());

            // myFloat
            var myFloatFieldById = logFile.FindField(1);
            var floatFieldId = logFile.GetFieldId("My Group", "myFloat");
            var myFloatField = myGroup.Fields.Where(x => x.ID == 1).FirstOrDefault();

            Assert.AreEqual(floatFieldId, myFloatFieldById.ID);
            Assert.AreEqual(myFloatField, myFloatFieldById);

            Assert.AreEqual(1, myFloatField.ID);
            Assert.AreEqual(typeof(float), myFloatField.ValueType);
            Assert.AreEqual(myGroup, myFloatField.Group);
            Assert.AreEqual("myFloat", myFloatField.Name);

            // myString
            var myStringFieldById = logFile.FindField(2);
            var stringFieldId = logFile.GetFieldId("My Group", "myString");
            var myStringField = myGroup.Fields.Where(x => x.ID == 2).FirstOrDefault();

            Assert.AreEqual(stringFieldId, myStringFieldById.ID);
            Assert.AreEqual(myStringField, myStringFieldById);

            Assert.AreEqual(2, myStringField.ID);
            Assert.AreEqual(typeof(string), myStringField.ValueType);
            Assert.AreEqual(myGroup, myStringField.Group);
            Assert.AreEqual("myString", myStringField.Name);

            // subGroup
            var mySubGroupById = logFile.FindGroup(2);
            var mySubGroupId = logFile.GetGroupId("My Subgroup");
            var mySubGroup = myGroup.Groups.Where(x => x.ID == 2).FirstOrDefault();

            Assert.AreEqual(mySubGroup.ID, mySubGroupId);
            Assert.AreEqual(mySubGroupById, mySubGroup);

            Assert.AreEqual(2, mySubGroup.ID);
            Assert.AreEqual(1, mySubGroup.Fields.Count());
            Assert.AreEqual(0, mySubGroup.Groups.Count());
            Assert.AreEqual(myGroup, mySubGroup.Master);
            Assert.AreEqual(logFile, mySubGroup.File);
            Assert.AreEqual("My Subgroup", mySubGroup.Name);

            // myDouble
            var myDoubleFieldById = logFile.FindField(3);
            var doubleFieldId = logFile.GetFieldId("My Subgroup", "myDouble");
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
            Assert.AreEqual(2, logFile.Time.Count());

            var lastTime = 0;
            var lastOffset = 0;
            var mySwitchpoint = 0;
            foreach (var timeDict in logFile.Time)
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
            var timeline = logFile.Timeline.ToList();
            Assert.AreEqual(timeline.Count, _logWriter.testDataFrames);
            var floatData = _logWriter.GetFloatData();
            var doubleData = _logWriter.GetDoubleData();

            var myFloat1 = logFile.ReadAs<float>("My Group", "myFloat", timeline.FirstOrDefault());
            Assert.AreEqual(myFloat1, floatData[0]);

            Stopwatch w = new Stopwatch();
            w.Start();
            for(int i = 0; i < floatData.Length; i++)
            {
                Assert.AreEqual(logFile.ReadAs<float>("My Group", "myFloat", timeline[i]), floatData[i]);
                //Assert.AreEqual(logFile.ReadAs<double>("My Group", "myFloat", timeline[i]), (double)floatData[i]);
                Assert.AreEqual(logFile.ReadAs<double>("My Subgroup", "myDouble", timeline[i]), doubleData[i]);
            }
            w.Stop();

            Debug.WriteLine("Reading 3x" + floatData.Length + " floats from file took " + w.ElapsedMilliseconds + " ("+w.ElapsedMilliseconds*1000.0/floatData.Length/2+"us per read)");
        }
    }
}