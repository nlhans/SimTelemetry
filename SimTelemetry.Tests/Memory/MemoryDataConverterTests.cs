using System;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Tests.Memory
{
    public enum TestCustomConverter : int
    {
        All = 3,
        Test1 = 4,
        Test2 = 12,
        Test3 = 565,
        Test4 = 165419,
        Error = 1337
    }

    [TestFixture]
    public class MemoryDataConverterTests
    {
        [Test]
        public void ConvertCustom()
        {
            var data = new byte[20];

            // integer code *2 is our 'conversion'
            Array.Copy(BitConverter.GetBytes((int)(TestCustomConverter.All)*2), 0, data, 0, 4);
            Array.Copy(BitConverter.GetBytes((int)(TestCustomConverter.Test1)*2), 0, data, 4, 4);
            Array.Copy(BitConverter.GetBytes((int)(TestCustomConverter.Test2)*2), 0, data, 8, 4);
            Array.Copy(BitConverter.GetBytes((int)(TestCustomConverter.Test3)*2), 0, data, 12, 4);
            Array.Copy(BitConverter.GetBytes((int)(TestCustomConverter.Test4)*2), 0, data, 16, 4);

            MemoryDataConverter.AddProvider(new MemoryDataConverterProvider<TestCustomConverter>(
                                                (arr, ind) =>
                                                    {
                                                        int intermediate = BitConverter.ToInt32(arr, ind);
                                                        return (TestCustomConverter) (intermediate/2);
                                                    }
                                                , (obj) =>
                                                      {
                                                          if (obj is int)
                                                              return (TestCustomConverter) ((int) obj/2);
                                                          else
                                                          {
                                                              return TestCustomConverter.Error;
                                                          }
                                                      }));

            Assert.AreEqual(TestCustomConverter.All, MemoryDataConverter.Read<TestCustomConverter>(data, 0));
            Assert.AreEqual(TestCustomConverter.Test1, MemoryDataConverter.Read<TestCustomConverter>(data, 4));
            Assert.AreEqual(TestCustomConverter.Test2, MemoryDataConverter.Read<TestCustomConverter>(data, 8));
            Assert.AreEqual(TestCustomConverter.Test3, MemoryDataConverter.Read<TestCustomConverter>(data, 12));
            Assert.AreEqual(TestCustomConverter.Test4, MemoryDataConverter.Read<TestCustomConverter>(data, 16));


            Assert.AreEqual(TestCustomConverter.All, MemoryDataConverter.Read<int, TestCustomConverter>(data, 0));
            Assert.AreEqual(TestCustomConverter.Test1, MemoryDataConverter.Read<int, TestCustomConverter>(data, 4));
            Assert.AreEqual(TestCustomConverter.Test2, MemoryDataConverter.Read<int, TestCustomConverter>(data, 8));
            Assert.AreEqual(TestCustomConverter.Test3, MemoryDataConverter.Read<int, TestCustomConverter>(data, 12));
            Assert.AreEqual(TestCustomConverter.Test4, MemoryDataConverter.Read<int, TestCustomConverter>(data, 16));

            // Short != type of int, so the conversion function returns 'error'
            Assert.AreEqual(TestCustomConverter.Error, MemoryDataConverter.Read<short, TestCustomConverter>(data, 16));
        }

        [Test]
        public void ConvertBool()
        {
            var data = new byte[3] {1, 0, 23};

            Assert.True(MemoryDataConverter.Read<bool>(data, 0));
            Assert.False(MemoryDataConverter.Read<bool>(data, 1));
            Assert.True(MemoryDataConverter.Read<bool>(data,2 ));
        }

        [Test]
        public void ConvertIntegers()
        {
            var data = new byte[8]
                           {
                               0xFF,4,6,8, 10,12,14,16
                           };

            byte controlU8 = (byte)data[0];
            char controlS8 = Encoding.ASCII.GetChars(data, 0, 1)[0];
            ushort controlU16 = BitConverter.ToUInt16(data, 0);
            short controlS16 = BitConverter.ToInt16(data, 0);
            uint controlU32 = BitConverter.ToUInt32(data, 0);
            int controlS32 = BitConverter.ToInt32(data, 0);
            ulong controlU64 = BitConverter.ToUInt64(data, 0);
            long controlS64 = BitConverter.ToInt64(data, 0);

            Assert.AreEqual(controlU8, MemoryDataConverter.Read<byte>(data, 0));
            Assert.AreEqual(controlS8, MemoryDataConverter.Read<char>(data, 0));

            Assert.AreEqual(controlU16, MemoryDataConverter.Read<ushort>(data, 0));
            Assert.AreEqual(controlS16, MemoryDataConverter.Read<short>(data, 0));

            Assert.AreEqual(controlU32, MemoryDataConverter.Read<uint>(data, 0));
            Assert.AreEqual(controlS32, MemoryDataConverter.Read<int>(data, 0));

            Assert.AreEqual(controlU64, MemoryDataConverter.Read<ulong>(data, 0));
            Assert.AreEqual(controlS64, MemoryDataConverter.Read<long>(data, 0));
        }

        [Test]
        public void ConvertString()
        {
            var data = new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x21 , 0 };
            var data2 = new byte[256];

            for (int i = 0; i < 256; i++)
                data2[i] = (byte) ((i + 124 - i/10)%256);
            Array.Copy(data, 0, data2, 64, data.Length);

            Assert.AreEqual("Hello World!", MemoryDataConverter.Read<string>(data, 0));
            Assert.AreEqual("Hello World!", MemoryDataConverter.Read<string>(data2, 64));
        }

        [Test]
        public void ConvertFloats()
        {
            float inp1 = 13.37f;
            double inp2 = Math.PI;

            byte[] bytes1 = BitConverter.GetBytes(inp1);
            byte[] bytes2 = BitConverter.GetBytes(inp2);

            var data = new byte[256];

            for (int i = 0; i < 256; i++)
                data[i] = (byte) ((i + 124 - i/10)%256);
            Array.Copy(bytes1, 0, data, 48, bytes1.Length);
            Array.Copy(bytes2, 0, data, 192, bytes2.Length);

            Assert.AreEqual(Math.PI, MemoryDataConverter.Read<double>(data, 192));
            Assert.AreEqual(13.37f, MemoryDataConverter.Read<float>(data, 48));

            // intermediate conversion:
            Assert.AreEqual((float)Math.PI, MemoryDataConverter.Read<double, float>(data, 192), 0.00001);
            Assert.AreEqual(13.37, MemoryDataConverter.Read<float, double>(data, 48), 0.0001);
        }

        [Test]
        public void ConvertLists()
        {
            byte[] data = new byte[256];
            for(int i = 0; i < 64;i++)
                Array.Copy(BitConverter.GetBytes(1337+i*64), 0, data, i*4, 4);

            int[] iData = MemoryDataConverter.Read<int[]>(data, 0);

            for(int i = 0; i < 64;i++)
            {
                int control = 1337 + i*64;
                Assert.AreEqual(control, iData[i]);
            }
        }

        [Test]
        public void RawifyString()
        {
            var testString = "Hello World!";
            byte[] realData = new byte[] { 0x0c, 0, 0, 0, 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x21 };

            Assert.AreEqual(realData, MemoryDataConverter.Rawify(testString));
        }
    }
}