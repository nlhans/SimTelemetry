using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Objects;
using Triton.Memory;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class MemoryTests
    {
        [Test]
        public void TestBitconverterSpeed()
        {
            int samples = 10000000;
            int sampleSize = 8;
            double[] d = new double[samples];
            byte[] data = new byte[samples * sampleSize];
            for (int i = 0; i < samples; i++)
            {
                d[i] = i * i * 1.0;
                var bytes = BitConverter.GetBytes(d[i]);
                Array.Copy(bytes, 0, data, i * sampleSize, sampleSize);
            }

            Stopwatch w = new Stopwatch();
            w.Start();

            int xd = 0;

            for (int i = 0; i < samples; i++)
            {
                double v = d[i];
                double a = d[i];
                if (a == v) xd++;
                //Assert.AreEqual(v, a);
            }
            Assert.AreEqual(xd, samples);
            w.Stop();
            Debug.WriteLine("Baseline took " + w.ElapsedMilliseconds + "ms -> " + (w.ElapsedMilliseconds / sampleSize) + "ns per 1 call");
            w.Reset();
            w.Start();

            xd = 0;

            // Bitconverter
            for (int i = 0; i < samples; i++)
            {
                double v = BitConverter.ToDouble(data, i * sampleSize);
                double a = d[i];
                if (a == v) xd++;
                //Assert.AreEqual(v, a);
            }
            Assert.AreEqual(xd, samples);
            w.Stop();
            Debug.WriteLine("10M double bitconverters took " + w.ElapsedMilliseconds + "ms -> " + (w.ElapsedMilliseconds / sampleSize) + "ns per 1 call");

        }

        [Test]
        public void MemReaderTest()
        {

            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("flux")[0];

            Stopwatch w = new Stopwatch();
            w.Start();
            // Read some data from player.
            for (int i = 0; i < 10000; i++)
            {
                int[] ints = new int[256];
                for (int j = 0; j < 256; j++)
                    ints[j] = r.ReadInt32(new IntPtr(0x7154C0 + j * 4));
            }
            w.Stop();
            Debug.WriteLine("10K iterations of 256 reads took " + w.ElapsedMilliseconds);
            w.Reset();


            w.Start();
            // Read some data from player.
            for (int i = 0; i < 10000; i++)
            {
                byte[] data = r.ReadBytes(new IntPtr(0x7154C0), 0x4000);
                float[] ints = new float[256];
                for (int j = 0; j < 256; j++)
                    ints[j] = BitConverter.ToSingle(data, j * 4);
            }
            w.Stop();
            Debug.WriteLine("10K iterations of 10KB reads took " + w.ElapsedMilliseconds);
            w.Reset();


        }

        [Test]
        public void DataConversionTest()
        {
            var dataarr = new byte[1024];

            Array.Copy(BitConverter.GetBytes(1337.0f), 0, dataarr, 16, 4);
            Array.Copy(BitConverter.GetBytes(1337 * 2.0), 0, dataarr, 32, 8);

            float f1 = MemoryDataConverter.Read<float>(dataarr, 16);
            float f2 = MemoryDataConverter.Read<double, float>(dataarr, 32);

            double d1 = MemoryDataConverter.Read<double>(dataarr, 32);
            double d2 = MemoryDataConverter.Read<float, double>(dataarr, 16);

            Assert.AreEqual(1337.0f, f1);
            Assert.AreEqual(1337.0f * 2.0f, f2);

            Assert.AreEqual(1337 * 2.0, d1);
            Assert.AreEqual(1337.0, d2);

            // Speed tests.
            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("rfactor")[0];

            int[] refints = new int[256];
            Stopwatch w = new Stopwatch();
            w.Start();
            // Read some data from player.
            for (int i = 0; i < 250000 / 256; i++)
            {
                for (int j = 0; j < 256; j++)
                    refints[j] = r.ReadInt32(new IntPtr(0x7154C0 + j * 4));
            }
            w.Stop();
            Debug.WriteLine("10K iterations of 256 reads took " + w.ElapsedMilliseconds + "(" + Math.Round(1.0 / w.ElapsedMilliseconds * 10000 * 256, 1) + "k IOPS)");
            w.Reset();

            w.Start();
            for (int i = 0; i < 250000 / 256; i++)
            {
                byte[] data = r.ReadBytes(new IntPtr(0x7154C0), 0x4000);
                long[] ints = new long[256];
                for (int j = 0; j < 256; j++)
                {
                    ints[j] = MemoryDataConverter.Read<int, long>(data, j * 4);
                    if (ints[j] != refints[j])
                        Assert.Fail();
                }

            }
            w.Stop();
            Debug.WriteLine("10K iterations of 10kB reads +256 conversions with MemoryDataConverter took  " + w.ElapsedMilliseconds + "(" + Math.Round(1.0 / w.ElapsedMilliseconds * 10000 * 256, 1) + "k IOPS)");
            w.Reset();

            w.Start();
            for (int i = 0; i < 250000 / 256; i++)
            {
                byte[] data = r.ReadBytes(new IntPtr(0x7154C0), 0x4000);
                int[] ints = new int[256];
                for (int j = 0; j < 256; j++)
                {
                    ints[j] = BitConverter.ToInt32(data, j * 4);
                    if (ints[j] != refints[j])
                        Assert.Fail();
                }

            }
            w.Stop();
            Debug.WriteLine("10K iterations of 10kB reads +256 conversions with BitConverter took " + w.ElapsedMilliseconds + "(" + Math.Round(1.0 / w.ElapsedMilliseconds * 10000 * 256, 1) + "k IOPS)");
            w.Reset();

        }

        [Test]
        public void testPerformance()
        {
            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("rfactor")[0];
            r.Diagnostic = true;
            r.Open();

            // The MemoryProvider object is filled/generated inside a plugin.
            MemoryProvider provider = new MemoryProvider(r);
            MemoryPool DriverPtrs = new MemoryPool("Drivers", MemoryRefreshLevel.SlowParameter, MemoryAddressType.STATIC, 0x315284, 0x200);
            DriverPtrs.Add(new MemoryField<int>("CarViewPort", MemoryRefreshLevel.SlowParameter, MemoryAddressType.DYNAMIC, 0, 0, 4));
            DriverPtrs.Add(new MemoryField<int>("CarPlayer", MemoryRefreshLevel.SlowParameter, MemoryAddressType.DYNAMIC, 0, 0x8, 4));
            DriverPtrs.Add(new MemoryField<int>("Cars", MemoryRefreshLevel.SlowParameter, MemoryAddressType.DYNAMIC, 0, 0xC, 4));
            provider.Add(DriverPtrs);
            DriverPtrs.Refresh(MemoryRefreshLevel.Always);
            int Drivers = DriverPtrs.ReadAs<int>("Cars");
            for (int i = 0; i < Drivers; i++)
            {
                //MemoryPool Driver = new MemoryPool("Driver", MemoryRefreshLevel.Parameter, MemoryAddressType.STATIC_ABSOLUTE, DriverPtrs.ReadAs<int>(0x14 + i * 4), 0x5F48); // base, 0x5F48 size
                MemoryPool Driver = new MemoryPool("Driver", MemoryRefreshLevel.Parameter, MemoryAddressType.DYNAMIC, DriverPtrs, 0x14+i*4, 0x5F48); // base, 0x5F48 size
                Driver.Add(new MemoryLazyField<string>("Name", MemoryRefreshLevel.OneTime, MemoryAddressType.DYNAMIC, 0, 0x5B08, 32));
                for (int j = 0; j < ((i == 0) ? 35 : 15); j++)
                {
                    Driver.Add(new MemoryLazyField<float>("Speed", MemoryRefreshLevel.Parameter, MemoryAddressType.DYNAMIC, 0, 0x57C0, 8, (x) => x * 3.6f));
                    Driver.Add(new MemoryLazyField<float>("RPM", MemoryRefreshLevel.Parameter, MemoryAddressType.DYNAMIC, 0, 0xA4, 8, Rotations.Rads_RPM));
                    Driver.Add(new MemoryLazyField<byte>("Gear", MemoryRefreshLevel.Parameter, MemoryAddressType.DYNAMIC, 0, 0x321C, 1));
                }
                Driver.Add(new MemoryField<string>("Track", MemoryRefreshLevel.OneTime, MemoryAddressType.STATIC, 0x00309D28, 0, 128));
                provider.Add(Driver);
            }
            MemoryPool session = new MemoryPool("Session", MemoryRefreshLevel.Parameter, MemoryAddressType.STATIC, 0x00309D24, 0x1000);
            session.Add(new MemoryLazyField<string>("Track", MemoryRefreshLevel.OneTime, MemoryAddressType.DYNAMIC, 0, 4, 128));
            provider.Add(session);

            // From here 'application'.
            provider.Refresh(MemoryRefreshLevel.Always);

            MemoryPool drv1 = provider.Pools.Where(x => x.Name == "Driver").FirstOrDefault();
            MemoryPool sess = provider.Pools.Where(x => x.Name == "Session").FirstOrDefault();

             Debug.WriteLine(sess.ReadAs<string>("Track"));
            // Debug.WriteLine(drv1.ReadAs<string>("Track"));
            Debug.WriteLine(drv1.ReadAs<string>("Name"));
            Debug.WriteLine(drv1.ReadAs<double>("RPM"));
            Debug.WriteLine(drv1.ReadAs<double>("Speed"));
            Debug.WriteLine(drv1.ReadAs<int>("Gear"));
            Thread.Sleep(1000);
            Debug.WriteLine("ReadMemory calls: " + r.ReadCalls);
            for (int i = 0; i < 10000; i++)
            {
                provider.Refresh(i % 10 == 0 ? MemoryRefreshLevel.SlowParameter : MemoryRefreshLevel.Parameter);
                /*drv1.ReadAs<string>("Name");
                drv1.ReadAs<double>("RPM");
                drv1.ReadAs<double>("Speed");
                drv1.ReadAs<int>("Gear");*/
            }
        }

        [Test]
        public void TestRfactor()
        {
            int Drivers = 100;

            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("rfactor")[0];
            r.Diagnostic = true;
            r.Open();

            // The MemoryProvider object is filled/generated inside a plugin.
            MemoryProvider provider = new MemoryProvider(r);
            for (int i = 0; i < Drivers; i++)
            {
                MemoryPool Driver = new MemoryPool("Driver", MemoryRefreshLevel.Parameter, MemoryAddressType.STATIC, 0x3154C0, 0x5F48); // base, 0x5F48 size
                Driver.Add(new MemoryLazyField<string>("Name", MemoryRefreshLevel.OneTime, MemoryAddressType.DYNAMIC, 0, 0x5B08));
                for (int j = 0; j < ((i == 0) ? 100 : 15); j++)
                {
                    Driver.Add(new MemoryLazyField<float>("Speed", MemoryRefreshLevel.SlowParameter, MemoryAddressType.DYNAMIC, 0, 0x57C0, (x) => x * 3.6f));
                    Driver.Add(new MemoryLazyField<float>("RPM", MemoryRefreshLevel.Parameter, MemoryAddressType.DYNAMIC, 0, 0xA4, Rotations.Rads_RPM));
                    Driver.Add(new MemoryLazyField<byte>("Gear", MemoryRefreshLevel.SlowParameter, MemoryAddressType.DYNAMIC, 0, 0x321C));
                }
                Driver.Add(new MemoryLazyField<string>("Track", MemoryRefreshLevel.OneTime, MemoryAddressType.STATIC, 0x00309D28, 0, 128));
                provider.Add(Driver);
            }
            MemoryPool session = new MemoryPool("Session", MemoryRefreshLevel.SlowParameter, MemoryAddressType.STATIC, 0x00309D24, 0x1000);
            session.Add(new MemoryLazyField<string>("Track", MemoryRefreshLevel.OneTime, MemoryAddressType.DYNAMIC, 0, 4, 128));
            provider.Add(session);

            // From here 'application'.
            provider.Refresh();

            MemoryPool drv1 = provider.Pools.FirstOrDefault();
            MemoryPool sess = provider.Pools.Where(x => x.Name == "Session").FirstOrDefault();

           // Debug.WriteLine(sess.ReadAs<string>("Track"));
           // Debug.WriteLine(drv1.ReadAs<string>("Track"));
            Debug.WriteLine(drv1.ReadAs<string>("Name"));
            Debug.WriteLine(drv1.ReadAs<double>("RPM"));
            Debug.WriteLine(drv1.ReadAs<double>("Speed"));
            Debug.WriteLine(drv1.ReadAs<int>("Gear"));
            Thread.Sleep(1000);
            Console.WriteLine("ReadMemory calls: " + r.ReadCalls);

            // Speed comparisons.
            Stopwatch w = new Stopwatch();
            w.Reset();
            int[] data = new int[3500 * (Drivers + 1)];
            w.Start();
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = r.ReadInt32(0x7154C0 + i*4);
                }
            }
            w.Stop();
            Thread.Sleep(1000);
            double tickLength = w.ElapsedMilliseconds/1000.0;
            Console.WriteLine((r.ReadCalls / 1000) + "k  ReadInt32() -> ReadMemory calls: " + r.ReadCalls + " / " + w.ElapsedMilliseconds + "ms -> " + tickLength + "ms per tick");
            Console.WriteLine("CPU time from memory reading @ 100Hz -> " + Math.Round(tickLength * 100 / 10.0, 2) + "%");

            w.Reset();
            w.Start();
            for (int i = 0; i <1000; i++)
            {
                for (int j = 0; j < Drivers+1; j++)
                    r.ReadBytes(0x7154C0, 0x5F48);
            }
            w.Stop();
            Thread.Sleep(1000);
            tickLength = w.ElapsedMilliseconds/1000.0;
            Console.WriteLine("1k*(1+drivers)*ReadBytes(..., 0x5F48) -> ReadMemory calls: " + r.ReadCalls + " / " + w.ElapsedMilliseconds + "ms -> " + tickLength + "ms per tick");
            Console.WriteLine("CPU time from memory reading @ 100Hz -> " + Math.Round(tickLength * 100 / 10.0, 2) + "%");

            w.Reset();
            w.Start();
            Console.WriteLine("Reading..");
            for (int i = 0; i < 5000; i++)
            {
                //provider.Refresh(i % 10 == 0 ? MemoryRefreshLevel.SlowParameter : MemoryRefreshLevel.Parameter);
                provider.Refresh(MemoryRefreshLevel.Always);
                drv1.ReadAs<string>("Name");
                drv1.ReadAs<double>("RPM");
                drv1.ReadAs<double>("Speed");
                drv1.ReadAs<int>("Gear");
                Thread.Sleep(20);
            }
            w.Stop();
            Thread.Sleep(1000);
            tickLength = w.ElapsedMilliseconds/5000.0;
            Console.WriteLine("1k*(1+drivers)*Refresh() -> ReadMemory calls: " + r.ReadCalls + " / " + w.ElapsedMilliseconds + "ms -> " + tickLength + "ms per tick");
            Console.WriteLine("CPU time from memory reading @ 100Hz -> " + Math.Round(tickLength * 100 / 10.0, 2) + "%");
        }
    }
}
