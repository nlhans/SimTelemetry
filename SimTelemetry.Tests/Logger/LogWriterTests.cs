using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Tests.Logger
{
    [TestFixture]
    class LogWriterTests
    {
        private LogFile logFile;
        private Domain.Utils.ZipStorer zipFile;
        private int testDataFrames = 1200000; // Enough data for 2 log files
        private int switchPoint = 0;
        [Test]
        public void HouseKeeping()
        {
             logFile = new LogFile();
            Assert.AreEqual(false, logFile.ReadOnly);
            Assert.AreEqual(0, logFile.ID);
            Assert.AreEqual(0, logFile.Groups.Count());
            Assert.AreEqual(0, logFile.Fields.Count());

            // Our logger doesn't support top-entry level fields
            logFile.CreateField("Test", typeof(string), 1);
            Assert.AreEqual(0, logFile.Fields.Count());

            LogGroup myGroup = logFile.CreateGroup("My Group");
            Assert.AreEqual(logFile.Groups.Count(), 1);
            Assert.AreEqual("My Group", myGroup.Name);
            Assert.AreEqual(1, myGroup.ID);
            Assert.AreEqual(0, myGroup.Fields.Count());
            Assert.AreEqual(0, myGroup.Groups.Count());
            Assert.AreEqual(logFile, myGroup.Master);
            Assert.AreEqual(logFile, myGroup.File);
            Assert.AreEqual(myGroup, logFile.SearchGroup(1));
            Assert.AreEqual(myGroup, logFile.Groups.Where(x => x.ID == 1).FirstOrDefault());

            // Add a field to a group
            var myFloat = myGroup.CreateField<float>("myFloat");
            var myString = (LogField<string>) myGroup.CreateField("myString", typeof(string));

            // ID
            Assert.AreEqual(1, myFloat.ID);
            Assert.AreEqual(2, myString.ID);

            // Group
            Assert.AreEqual(myGroup, myFloat.Group);
            Assert.AreEqual(myGroup, myString.Group);

            // File
            Assert.AreEqual(logFile, myFloat.File);
            Assert.AreEqual(logFile, myString.File);

            // ValueType
            Assert.AreEqual(typeof(float), myFloat.ValueType);
            Assert.AreEqual(typeof(string), myString.ValueType);

            Assert.AreEqual(2, myGroup.Fields.Count());

            Assert.AreEqual(myFloat, logFile.SearchField(1));
            Assert.AreEqual(myString, logFile.SearchField(2));
            Assert.AreEqual(1, logFile.GetFieldId("My Group", "myFloat"));
            Assert.AreEqual(1, logFile.GetFieldId(1, "myFloat"));
            Assert.AreEqual(2, logFile.GetFieldId("My Group", "myString"));
            Assert.AreEqual(2, logFile.GetFieldId(1, "myString"));

            // Subgroup
            var subGroup = myGroup.CreateGroup("My Subgroup");
            Assert.AreEqual(logFile.Groups.Count(), 1);
            Assert.AreEqual(myGroup.Groups.Count(), 1);
            Assert.AreEqual(2, subGroup.ID);
            Assert.AreEqual("My Subgroup", subGroup.Name);

            Assert.AreEqual(0, subGroup.Fields.Count());
            Assert.AreEqual(0, subGroup.Groups.Count());
            Assert.AreEqual(myGroup, subGroup.Master);
            Assert.AreEqual(logFile, subGroup.File);

            Assert.AreEqual(subGroup, logFile.SearchGroup(2));
            Assert.AreEqual(null, logFile.Groups.Where(x => x.ID == 2).FirstOrDefault());

            // Add test field
            var myDouble = subGroup.CreateField("myDouble", typeof (double));

            Assert.AreEqual(3, myDouble.ID);
            Assert.AreEqual("myDouble", myDouble.Name);
            Assert.AreEqual(typeof(double), myDouble.ValueType);
            Assert.AreEqual(subGroup, myDouble.Group);
            Assert.AreEqual(typeof(LogField<double>), myDouble.GetType());
            Assert.AreEqual(logFile, ((LogField<double>)myDouble).File);

        }

        [Test]
        public void ZipStorer()
        {
            // Just double-checking my libaries.
            byte[] sourceData = new byte[1024];
            for(int i = 0;  i < 1024; i++)
            {
                sourceData[i] = (byte)((1234 + i/10 - i) % 255);
            }

            var sourceStream = new MemoryStream(sourceData);
            var zipWriter = Domain.Utils.ZipStorer.Create("zipzipzip.zip", "");
            zipWriter.AddStream(Domain.Utils.ZipStorer.Compression.Deflate, "Test.txt", sourceStream, DateTime.Now, "");
            zipWriter.Close();

            // Read it
            var zipReader = Domain.Utils.ZipStorer.Open("zipzipzip.zip", FileAccess.Read);
            var zipFiles = zipReader.ReadCentralDir();
            
            Assert.AreEqual(1, zipFiles.Count);
            
            var zipFile1 = zipFiles.FirstOrDefault();
            
            Assert.AreEqual("Test.txt", zipFile1.FilenameInZip);
            Assert.AreEqual(sourceData.Length, zipFile1.FileSize);
            
            var outputStream = new MemoryStream();
            zipReader.ExtractFile(zipFile1, outputStream);

            byte[] outputData = new byte[zipFile1.FileSize];
            outputStream.Seek(0, SeekOrigin.Begin);
            outputStream.Read(outputData, 0, outputData.Length);

            Assert.AreEqual(sourceData, outputData);

            zipReader.Close();

            File.Delete("zipzipzip.zip");
        }

        [Test]
        public void BinaryTests()
        {
            int i = 0;

            // Init structure:
            HouseKeeping();
            float[] floatData = GetFloatData();
            string[] stringData = GetStringData();
            double[] doubleData = GetDoubleData();

            Func<object> readFloat = () => floatData[i];
            Func<object> readDouble = () => doubleData[i];
            Func<object> readString = () => stringData[i/10];

            i = 0;
            while (i < testDataFrames)
            {
                logFile.Write("My Subgroup","myDouble", MemoryDataConverter.Rawify(readDouble));
                if (i%10 == 0)
                    logFile.Write("My Group", "myString", MemoryDataConverter.Rawify(readString));
                logFile.Write("My Group", "myFloat", MemoryDataConverter.Rawify(readFloat));

                logFile.Flush(i*40); // simulate every 40ms, 25Hz
                i++;
            }

            // This stores it into a zip
            logFile.Finish("temp.zip");

            // Use ZipStorer to extract the data again and byte-by-byte analyze the format.
            zipFile = Domain.Utils.ZipStorer.Open("temp.zip", FileAccess.Read);

            var files = zipFile.ReadCentralDir();
            Assert.AreEqual(4, files.Count); // Data1.bin, Data2.bin, Structure.bin, Time.bin
            Assert.True(files.Any(x => x.FilenameInZip == "Data1.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Data2.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Structure.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Time.bin"));
            // TODO: Add Laptimes index

        }

        [Test]
        public void TestData()
        {
            int timeout = 0;
            int sourceDataIndex = 0;

            var files = zipFile.ReadCentralDir();
            int i = 0;

            var floatData = GetFloatData();
            var stringData = GetStringData();
            var doubleData = GetDoubleData();

            // read the source data 1
            var dataFile1 = files.Where(x => x.FilenameInZip == "Data1.bin").FirstOrDefault();
            Assert.AreEqual(1024*1024*16, dataFile1.FileSize);
            var sourceData1 = new byte[dataFile1.FileSize];
            var sourceStream1 = new MemoryStream(sourceData1, true);
            zipFile.ExtractFile(dataFile1, sourceStream1);

            // read the source data 2
            var dataFile2 = files.Where(x => x.FilenameInZip == "Data2.bin").FirstOrDefault();
            Assert.AreEqual(1024*1024*16, dataFile2.FileSize);
            var sourceData2 = new byte[dataFile2.FileSize];
            var sourceStream2 = new MemoryStream(sourceData2, true);
            zipFile.ExtractFile(dataFile2, sourceStream2);

            byte[] sourceData = sourceData1;


            while (i < testDataFrames)
            {
                if (sourceDataIndex == 0x00ffffdd)
                {
                    switchPoint = i*40; // each float occurs on 40ms (25Hz) timebase
                    sourceData = sourceData2;
                    sourceDataIndex = 0;
                }

                // Is this a header?
                Assert.AreEqual(0x1F, sourceData[sourceDataIndex]);
                Assert.AreEqual(0x1F, sourceData[sourceDataIndex + 1]);

                // Get the field ID
                var fieldID = BitConverter.ToUInt32(sourceData, sourceDataIndex + 2);

                if (fieldID > 3 || fieldID == 0)
                    Assert.Fail("Invalid field ID at " + sourceDataIndex);

                if (fieldID == 1)
                {
                    var myFloat = BitConverter.ToSingle(sourceData, sourceDataIndex + 6);
                    Assert.AreEqual(myFloat, floatData[i]);
                    i++;

                    sourceDataIndex += 10;
                }
                else if(fieldID == 2)
                {
                    var stringLength = BitConverter.ToInt32(sourceData, sourceDataIndex + 6);
                    var myString = Encoding.ASCII.GetString(sourceData, sourceDataIndex + 10, stringLength);
                    Assert.AreEqual(myString, stringData[i/10]);

                    sourceDataIndex += 10 + stringLength;
                } else if(fieldID == 3)
                {
                    var myDouble = BitConverter.ToDouble(sourceData, sourceDataIndex + 6);
                    Assert.AreEqual(doubleData[i], myDouble);

                    sourceDataIndex += 14;
                }
                timeout++;
                if (timeout > sourceData.Length*2)
                    Assert.Fail();
            }
        }

        private double[] GetDoubleData()
        {
            var data = new double[testDataFrames];
           
            for(int i = 0 ; i < data.Length; i++)
                data[i] = 12345.6789/i;

            return data;
        }

        [Test]
        public void TestTimetable()
        {
            var files = zipFile.ReadCentralDir();
            int i = 0;

            int timeDataIndex = 0;
            uint lastTime = 0;
            uint lastTimeOffset = 0;
            bool justSwitched = false;

            // time table
            var timeFile = files.Where(x => x.FilenameInZip == "Time.bin").FirstOrDefault();
            var timeData = new byte[timeFile.FileSize];
            var timeStream = new MemoryStream(timeData, true);
            zipFile.ExtractFile(timeFile, timeStream);

            while(timeDataIndex < timeData.Length)
            {
                uint i1 = BitConverter.ToUInt32(timeData, timeDataIndex);
                uint i2 = BitConverter.ToUInt32(timeData, timeDataIndex + 4);

                if (i1 == 0x80000000)
                {
                    justSwitched = true;
                    if (i2 == 2)
                    {
                        Assert.AreEqual(switchPoint, lastTime+40); //lasttime is lagging by 40ms.
                        lastTimeOffset = 0; // reset index
                    }
                }
                else
                {
                    if (!justSwitched) // first time hasn't got previous records to track
                    {
                        Assert.Greater(i1, lastTime);
                        Assert.Greater(i2, lastTimeOffset);
                    }
                    lastTime = i1;
                    lastTimeOffset = i2;
                    justSwitched = false;
                }

                timeDataIndex += 8;
            }

        }

        [Test]
        public void TestStructure()
        {

            var files = zipFile.ReadCentralDir();
            int i = 0;
            var structureFileIndex = 0;

            // test structure
            var structureFile = files.Where(x => x.FilenameInZip == "Structure.bin").FirstOrDefault();
            var structureData = new byte[structureFile.FileSize];
            var structureStream = new MemoryStream(structureData, true);
            zipFile.ExtractFile(structureFile, structureStream);
             
            while(structureFileIndex < structureData.Length)
            {
                if (structureData[structureFileIndex] == 0x1E)
                {
                    // Field data
                    Assert.AreEqual(0x1E, structureData[structureFileIndex]);
                    var fieldID = BitConverter.ToInt32(structureData, structureFileIndex + 1);
                    var groupID = BitConverter.ToInt32(structureData, structureFileIndex + 5);
                    var nameLength = BitConverter.ToInt32(structureData, structureFileIndex + 9);
                    var name = Encoding.ASCII.GetString(structureData, structureFileIndex + 13, nameLength);
                    var typeLength = BitConverter.ToInt32(structureData, structureFileIndex + 13+nameLength);
                    var type = Encoding.ASCII.GetString(structureData, structureFileIndex + 17 + nameLength, typeLength);

                    if (fieldID != 1 && fieldID != 2 && fieldID != 3)
                        Assert.Fail("Invalid field at index " + structureFileIndex);
                    if(groupID != 1 && groupID!= 2)
                    Assert.Fail("Invalid group at index " +structureFileIndex);
                    if (fieldID == 1)
                    {
                        Assert.AreEqual("myFloat", name);
                        Assert.AreEqual(typeof (float).FullName, type);
                    }
                    if (fieldID == 2)
                    {
                        Assert.AreEqual("myString", name);
                        Assert.AreEqual(typeof(string).FullName, type);
                    }
                    if (fieldID == 3)
                    {
                        Assert.AreEqual("myDouble", name);
                        Assert.AreEqual(typeof(double).FullName, type);
                    }


                    structureFileIndex += 17 + nameLength + typeLength;
                }
                else if (structureData[structureFileIndex] == 0x1D)
                {
                    Assert.AreEqual(0x1D, structureData[structureFileIndex]);

                    var groupID = BitConverter.ToInt32(structureData, structureFileIndex + 1);
                    var masterID = BitConverter.ToInt32(structureData, structureFileIndex + 5);
                    var nameLength = BitConverter.ToInt32(structureData, structureFileIndex + 9);
                    var name = Encoding.ASCII.GetString(structureData, structureFileIndex + 13, nameLength);

                    if (groupID == 1)
                    {
                        Assert.AreEqual(0, masterID);
                        Assert.AreEqual("My Group", name);
                    }
                    else if (groupID == 2)
                    {
                        Assert.AreEqual(1, masterID);
                        Assert.AreEqual("My Subgroup", name);
                    }
                    else
                    {
                        Assert.Fail("Invalid group");
                    }

                    structureFileIndex += 13 + nameLength;

                }
                else
                {
                    Assert.Fail("Invalid data at index " +structureFileIndex);
                }
            }
        }

        public string[] GetStringData()
        {
            int i = 0;
            string[] stringData = new string[testDataFrames/10];
            while (i < stringData.Length)
            {
                stringData[i] = string.Format("{0:X}", i);
                i++;
            }
            return stringData;
        }

        public float[] GetFloatData()
        {
            int i = 0;
            // Write 100 floats
            float[] floatData = new float[testDataFrames];
            while (i < floatData.Length)
            {
                floatData[i] = i*(i + 10.0f)%550.0f + 5.1234f;
                i++;
            }
            return floatData;
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            if (zipFile != null)
            {
                zipFile.Close();
                zipFile = null;
            }

            if (File.Exists("temp.zip"))
                File.Delete("temp.zip");
            if (File.Exists("zipzipzip.zip"))
                File.Delete("zipzipzip.zip");
        }
    }
}
