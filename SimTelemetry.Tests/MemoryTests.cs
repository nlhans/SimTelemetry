using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Objects;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class MemoryTests
    {

        [Test]
        public void testPerformance()
        {
            var r = new MemoryReader();
            r.Diagnostic = true;
            r.Open( Process.GetProcessesByName("rfactor")[0]);

            // The MemoryProvider object is filled/generated inside a plugin.
            var provider = new MemoryProvider(r);
            var DriverPtrs = new MemoryPool("Drivers", MemoryAddress.STATIC, 0x315284, 0x200);
            DriverPtrs.Add(new MemoryField<int>("CarViewPort",MemoryAddress.DYNAMIC, 0, 0, 4));
            DriverPtrs.Add(new MemoryField<int>("CarPlayer", MemoryAddress.DYNAMIC, 0, 0x8, 4));
            DriverPtrs.Add(new MemoryField<int>("Cars", MemoryAddress.DYNAMIC, 0, 0xC, 4));
            provider.Add(DriverPtrs);
            DriverPtrs.Refresh();
            var drivers = DriverPtrs.ReadAs<int>("Cars");
            for (var i = 0; i < drivers; i++)
            {
                var driver = new MemoryPool("Driver", MemoryAddress.DYNAMIC, DriverPtrs, 0x14 + i * 4, 0x5F48); // base, 0x5F48 size
                driver.Add(new MemoryLazyField<string>("Name", MemoryAddress.DYNAMIC, 0, 0x5B08, 32));
                driver.Add(new MemoryLazyField<float>("Speed", MemoryAddress.DYNAMIC, 0, 0x57C0, 8, (x) => x * 3.6f));
                driver.Add(new MemoryLazyField<float>("RPM", MemoryAddress.DYNAMIC, 0, 0xA4, 8, Rotations.Rads_RPM));
                driver.Add(new MemoryLazyField<byte>("Gear", MemoryAddress.DYNAMIC, 0, 0x321C, 1));
                provider.Add(driver);
            }
            var session = new MemoryPool("Session", MemoryAddress.STATIC, 0x00309D24, 0x1000);
            session.Add(new MemoryLazyField<string>("Track", MemoryAddress.DYNAMIC, 0, 4, 128));
            provider.Add(session);

            // From here 'application'.
            provider.Refresh();

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
                provider.Refresh();
                /*drv1.ReadAs<string>("Name");
                drv1.ReadAs<double>("RPM");
                drv1.ReadAs<double>("Speed");
                drv1.ReadAs<int>("Gear");*/
            }
        }

        [Test]
        public void TestRfactor()
        {
            MemoryReader r = new MemoryReader();
            r.Diagnostic = true;
            r.Open(Process.GetProcessesByName("rfactor")[0]);

            // The MemoryProvider object is filled/generated inside a plugin.
            var provider = new MemoryProvider(r);
            var DriverPtrs = new MemoryPool("Drivers", MemoryAddress.STATIC, 0x315284, 0x200);
            DriverPtrs.Add(new MemoryField<int>("CarViewPort", MemoryAddress.DYNAMIC, 0, 0, 4));
            DriverPtrs.Add(new MemoryField<int>("CarPlayer", MemoryAddress.DYNAMIC, 0, 0x8, 4));
            DriverPtrs.Add(new MemoryField<int>("Cars", MemoryAddress.DYNAMIC, 0, 0xC, 4));
            provider.Add(DriverPtrs);
            DriverPtrs.Refresh();
            var drivers = DriverPtrs.ReadAs<int>("Cars");
            for (var i = 0; i < drivers; i++)
            {
                var driver = new MemoryPool("Driver", MemoryAddress.DYNAMIC, DriverPtrs, 0x14 + i * 4, 0x5F48); // base, 0x5F48 size
                driver.Add(new MemoryLazyField<string>("Name", MemoryAddress.DYNAMIC, 0, 0x5B08, 32));
                driver.Add(new MemoryLazyField<float>("Speed", MemoryAddress.DYNAMIC, 0, 0x57C0, 8, (x) => x * 3.6f));
                driver.Add(new MemoryLazyField<float>("RPM", MemoryAddress.DYNAMIC, 0, 0xA4, 8, Rotations.Rads_RPM));
                driver.Add(new MemoryLazyField<byte>("Gear", MemoryAddress.DYNAMIC, 0, 0x321C, 1));
                provider.Add(driver);
            }
            var session = new MemoryPool("Session", MemoryAddress.STATIC, 0x00309D24, 0x1000);
            session.Add(new MemoryLazyField<string>("Track", MemoryAddress.DYNAMIC, 0, 4, 128));
            provider.Add(session);

            // From here 'application'.
            provider.Refresh();

            MemoryPool drv1 = provider.Pools.Where(x => x.Name == "Driver").FirstOrDefault();
            MemoryPool sess = provider.Pools.Where(x => x.Name == "Session").FirstOrDefault();

            Debug.WriteLine(DriverPtrs.ReadAs<string>("Cars"));
            Debug.WriteLine(drv1.ReadAs<string>("Name"));
            Debug.WriteLine(drv1.ReadAs<double>("RPM"));
            Debug.WriteLine(drv1.ReadAs<double>("Speed"));
            Debug.WriteLine(drv1.ReadAs<int>("Gear"));
            Thread.Sleep(1000);
            Console.WriteLine("ReadMemory calls: " + r.ReadCalls);

            // Speed comparisons.
            Stopwatch w = new Stopwatch();
            w.Reset();
            int[] data = new int[3500 * (drivers + 2)];
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
            Console.WriteLine(@"{0}k  ReadInt32() -> ReadMemory calls: {1} / {2}ms -> {3}ms per tick", (r.ReadCalls / 1000), r.ReadCalls, w.ElapsedMilliseconds, tickLength);
            Console.WriteLine(@"CPU time from memory reading @ 100Hz -> {0}%", Math.Round(tickLength * 100 / 10.0, 2));

            w.Reset();
            w.Start();
            for (int i = 0; i <1000; i++)
            {
                for (int j = 0; j < drivers+1; j++)
                    r.ReadBytes(0x7154C0, 0x5F48);
            }
            w.Stop();
            Thread.Sleep(1000);
            tickLength = w.ElapsedMilliseconds/1000.0;
            Console.WriteLine(@"1k*(Drivers+1) Memory.ReadBytes(..., 0x5F48) -> ReadMemory calls: {0} / {1}ms -> {2}ms per tick", r.ReadCalls, w.ElapsedMilliseconds, tickLength);
            Console.WriteLine(@"CPU time from memory reading @ 100Hz -> {0}%", Math.Round(tickLength * 100 / 10.0, 2));

            w.Reset();
            w.Start();
            Console.WriteLine("Reading..");
            for (int i = 0; i < 1000; i++)
            {
                provider.Refresh();
                drv1.ReadAs<string>("Name");
                drv1.ReadAs<double>("RPM");
                drv1.ReadAs<double>("Speed");
                drv1.ReadAs<int>("Gear");
               // Thread.Sleep(20);
            }
            w.Stop();
            Thread.Sleep(1000);
            tickLength = w.ElapsedMilliseconds/1000.0;
            Console.WriteLine(@"1k * MemoryProvider.Refresh() -> ReadMemory calls: {0} / {1}ms -> {2}ms per tick", r.ReadCalls, w.ElapsedMilliseconds, tickLength);
            Console.WriteLine(@"CPU time from memory reading @ 100Hz -> {0}%", Math.Round(tickLength * 100 / 10.0, 2));
        }
    }
}
