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
        [Test]
        public void Structure()
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
            var zipWriter = Domain.Utils.ZipStorer.Create("temp.zip", "");
            zipWriter.AddStream(Domain.Utils.ZipStorer.Compression.Deflate, "Test.txt", sourceStream, DateTime.Now, "");
            zipWriter.Close();

            // Read it
            var zipReader = Domain.Utils.ZipStorer.Open("temp.zip", FileAccess.Read);
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

            File.Delete("temp.zip");
        }

        [Test]
        public void BinaryTests()
        {
            int frames = 1500000;

            // Init structure:
            Structure();
            int i = 0;
            // Write 100 floats
            float[] floatData = new float[frames];
            while (i < frames)
            {
                floatData[i] = i*(i + 10.0f)%550.0f + 5.1234f;
                i++;
            }
            string[] testData = new string[frames/10];
            i=0;
            while (i < testData.Length)
            {
                testData[i] = string.Format("{0:X}", i);
                i++;
            }

            Func<object> readFloat = () => floatData[i];
            Func<object> readString = () => testData[i / 10];

            i = 0;
            while (i < frames)
            {
                logFile.Write("My Group", "myFloat", MemoryDataConverter.Rawify(readFloat));
                if (i % 10 == 0)
                    logFile.Write("My Group", "myString", MemoryDataConverter.Rawify(readString));

                logFile.Flush(i*40); // simulate every 40ms, 25Hz
                i++;
            }

            // This stores it into a zip
            logFile.Finish("temp.zip");


            // Use ZipStorer to extract the data again and byte-by-byte analyze the format.
            var zip = Domain.Utils.ZipStorer.Open("temp.zip", FileAccess.Read);

            var files = zip.ReadCentralDir();
            Assert.AreEqual(4, files.Count); // Data1.bin, Structure.bin, Time.bin
            Assert.True(files.Any(x => x.FilenameInZip == "Data1.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Data2.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Structure.bin"));
            Assert.True(files.Any(x => x.FilenameInZip == "Time.bin"));
            // TODO: Add Laptimes index

            // read the source data 1
            var dataFile1 = files.Where(x => x.FilenameInZip == "Data1.bin").FirstOrDefault();
            Assert.AreEqual(1024 * 1024 * 16, dataFile1.FileSize);
            var sourceData1 = new byte[dataFile1.FileSize];
            var sourceStream1 = new MemoryStream(sourceData1, true);
            zip.ExtractFile(dataFile1, sourceStream1);

            var dataFile2 = files.Where(x => x.FilenameInZip == "Data2.bin").FirstOrDefault();
            Assert.AreEqual(1024 * 1024 * 16, dataFile2.FileSize);
            var sourceData2 = new byte[dataFile2.FileSize];
            var sourceStream2 = new MemoryStream(sourceData2, true);
            zip.ExtractFile(dataFile2, sourceStream2);

            byte[] sourceData = sourceData1;
            int timeout = 0;
            int sourceDataIndex = 0;
            i = 0;

            int switchPoint = 0;

            while (i < frames)
            {
                if(sourceDataIndex == 0xFFFFFB)
                {
                    switchPoint = i * 40; // each float occurs on 40ms (25Hz) timebase
                    sourceData = sourceData2;
                    sourceDataIndex = 0;
                }

                // Is this a header?
                Assert.AreEqual(0x1F, sourceData[sourceDataIndex]);
                Assert.AreEqual(0x1F, sourceData[sourceDataIndex + 1]);

                // Get the field ID
                var fieldID = BitConverter.ToUInt32(sourceData, sourceDataIndex + 2);

                if (fieldID > 2 || fieldID == 0)
                    Assert.Fail();

                if(fieldID == 1)
                {
                    var myFloat = BitConverter.ToSingle(sourceData, sourceDataIndex + 6);
                    Assert.AreEqual(myFloat, floatData[i]);
                    i++;

                    sourceDataIndex += 10;
                }
                else
                {
                    var stringLength = BitConverter.ToInt32(sourceData, sourceDataIndex + 6);
                    var myString= ASCIIEncoding.ASCII.GetString(sourceData, sourceDataIndex + 10, stringLength);
                    Assert.AreEqual(myString, testData[i/10]);

                    sourceDataIndex += 10 + stringLength;
                }
                timeout++;
                if(timeout> sourceData.Length*2)
                    Assert.Fail();
            }

            // time table
            var timeFile = files.Where(x => x.FilenameInZip == "Time.bin").FirstOrDefault();
            var timeData = new byte[timeFile.FileSize];
            var timeStream = new MemoryStream(timeData, true);
            zip.ExtractFile(timeFile, timeStream);

            int timeDataIndex = 0;
            uint timeFileNumber = 0;
            uint lastTime = 0;
            uint lastTimeOffset = 0;
            bool justSwitched = false;
            while(timeDataIndex < timeData.Length)
            {
                uint i1 = BitConverter.ToUInt32(timeData, timeDataIndex);
                uint i2 = BitConverter.ToUInt32(timeData, timeDataIndex + 4);

                if (i1 == 0x80000000)
                {
                    timeFileNumber = i2;
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
    }
}
