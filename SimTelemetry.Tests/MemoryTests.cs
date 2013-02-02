using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain.Memory;

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
            var DriverPtrs = new MemoryPool("Drivers", MemoryAddress.Static, 0x315284, 0x200);
            DriverPtrs.Add(new MemoryField<int>("CarViewPort",MemoryAddress.Dynamic, 0, 0, 4));
            DriverPtrs.Add(new MemoryField<int>("CarPlayer", MemoryAddress.Dynamic, 0, 0x8, 4));
            DriverPtrs.Add(new MemoryField<int>("Cars", MemoryAddress.Dynamic, 0, 0xC, 4));
            provider.Add(DriverPtrs);
            DriverPtrs.Refresh();
            var drivers = DriverPtrs.ReadAs<int>("Cars");
            for (var i = 0; i < drivers; i++)
            {
                var driver = new MemoryPool("Driver", MemoryAddress.Dynamic, DriverPtrs, 0x14 + i * 4, 0x5F48); // base, 0x5F48 size
                driver.Add(new MemoryFieldLazy<string>("Name", MemoryAddress.Dynamic, 0, 0x5B08, 32));
                driver.Add(new MemoryFieldLazy<float>("Speed", MemoryAddress.Dynamic, 0, 0x57C0, 8, (x) => x * 3.6f));
                driver.Add(new MemoryFieldLazy<float>("RPM", MemoryAddress.Dynamic, 0, 0xA4, 8));
                driver.Add(new MemoryFieldLazy<byte>("Gear", MemoryAddress.Dynamic, 0, 0x321C, 1));
                provider.Add(driver);
            }
            var session = new MemoryPool("Session", MemoryAddress.Static, 0x00309D24, 0x1000);
            session.Add(new MemoryFieldLazy<string>("Track", MemoryAddress.Dynamic, 0, 4, 128));
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
        public void TestMemory()
        {

            MemoryReader r = new MemoryReader();
            r.Diagnostic = true;
            r.Open(Process.GetProcessesByName("rfactor")[0]);

            var w = new Stopwatch();
            w.Start();
            for (int i = 0; i < 100000; i++)
                r.ReadInt32(0x7154C0);
            w.Stop();
            Debug.WriteLine(w.ElapsedMilliseconds);
            w.Reset();

            byte[] data = new byte[0x6000];
            w.Start();
            for (int i = 0; i < 5000; i++)
                r.Read(0x7154C0, data);

            w.Stop();
            Debug.WriteLine(w.ElapsedMilliseconds);
            w.Reset();

        }

        [Test]
        public void TestRfactor()
        {
            MemoryReader r = new MemoryReader();
            r.Diagnostic = true;
            r.Open(Process.GetProcessesByName("rfactor")[0]);

            // The MemoryProvider object is filled/generated inside a plugin.
            var provider = new MemoryProvider(r);

            var DriverPtrs = new MemoryPool("Drivers", MemoryAddress.Static, 0x315284, 0x200);
            DriverPtrs.Add(new MemoryFieldLazy<int>("CarViewPort", MemoryAddress.Dynamic, 0, 0, 4));
            DriverPtrs.Add(new MemoryFieldLazy<int>("CarPlayer", MemoryAddress.Dynamic, 0, 0x8, 4));
            DriverPtrs.Add(new MemoryFieldLazy<int>("Cars", MemoryAddress.Dynamic, 0, 0xC, 4));

            provider.Add(DriverPtrs);
            DriverPtrs.Refresh();

            // Add found drivers.
            DriverPtrs.ClearPools();

            var drivers = DriverPtrs.ReadAs<int>("Cars");
            for (var i = 0; i < drivers; i++)
            {
                var driver = new MemoryPool("Driver" + i, MemoryAddress.Dynamic, DriverPtrs, 0x14 + i * 4, 0x5F48);
                // base, 0x5F48 size
                driver.Add(new MemoryFieldConstant<int>("Index", i));
                driver.Add(new MemoryFieldConstant<bool>("IsActive", true));

                driver.Add(new MemoryFieldLazy<string>("Name", MemoryAddress.Dynamic, 0, 0x5B08, 32));
                driver.Add(new MemoryFieldLazy<string>("CarTeam", MemoryAddress.Dynamic, 0, 0x5C22, 64));
                driver.Add(new MemoryFieldLazy<string>("CarModel", MemoryAddress.Dynamic, 0, 0x5C62, 64));
                driver.Add(new MemoryFieldLazy<string>("CarClasses", MemoryAddress.Dynamic, 0, 0x39BC, 64));

                driver.Add(new MemoryFieldLazy<float>("Meter", MemoryAddress.Dynamic, 0, 0x3D04, 4));
                driver.Add(new MemoryFieldLazy<float>("Speed", MemoryAddress.Dynamic, 0, 0x57C0, 4));
                driver.Add(new MemoryFieldLazy<float>("Mass", MemoryAddress.Dynamic, 0, 0x28DC, 4));
                driver.Add(new MemoryFieldLazy<float>("RPM", MemoryAddress.Dynamic, 0, 0x317C, 4));
                driver.Add(new MemoryFieldLazy<float>("RPMMax", MemoryAddress.Dynamic, 0, 0x3180, 4));

                driver.Add(new MemoryFieldLazy<int>("Gear", MemoryAddress.Dynamic, 0, 0x321C, 1));

                for (int j = 0; j < 7; j++)
                    driver.Add(new MemoryFieldLazy<float>("GearRatio" + ((j == 0) ? "R" : i.ToString()),
                                                          MemoryAddress.Dynamic, 0, 0x31F8 + j * 4, 4));


                driver.Add(new MemoryFieldLazy<float>("TyreWearLF", MemoryAddress.Dynamic, 0, 0x2A34, 4));
                driver.Add(new MemoryFieldLazy<float>("TyreWearRF", MemoryAddress.Dynamic, 0, 0x2C1C, 4));
                driver.Add(new MemoryFieldLazy<float>("TyreWearLR", MemoryAddress.Dynamic, 0, 0x2E04, 4));
                driver.Add(new MemoryFieldLazy<float>("TyreWearRR", MemoryAddress.Dynamic, 0, 0x2FEC, 4));

                driver.Add(new MemoryFieldLazy<int>("Pitstops", MemoryAddress.Dynamic, 0, 0x3D2C, 4));
                driver.Add(new MemoryFieldLazy<int>("Position", MemoryAddress.Dynamic, 0, 0x3D20, 4));
                driver.Add(new MemoryFieldLazy<int>("Laps", MemoryAddress.Dynamic, 0, 0x3CF8, 1));

                driver.Add(new MemoryFieldLazy<bool>("IsRetired", MemoryAddress.Dynamic, 0, 0x4160, 1));
                driver.Add(new MemoryFieldLazy<bool>("IsLimiter", MemoryAddress.Dynamic, 0, 0x17B1, 1));
                driver.Add(new MemoryFieldLazy<bool>("IsPits", MemoryAddress.Dynamic, 0, 0x27A8, 1));
                driver.Add(new MemoryFieldLazy<bool>("IsDriving", MemoryAddress.Dynamic, 0, 0x3CBF, 1));
                //driver.Add(new MemoryFieldFunc<bool>("IsActive", (x) => { return true; }));
                /*(pool) =>
                                                                 pool.ReadAs<float>("CoordinateX") != 0 &&
                                                                 pool.ReadAs<float>("CoordinateY") != 0 &&
                                                                 pool.ReadAs<float>("CoordinateZ") != 0 &&
                                                                 pool.ReadAs<string>("Name").Length != 0     ));*/

                driver.Add(new MemoryFieldLazy<bool>("FlagYellow", MemoryAddress.Dynamic, 0, 0x104, 1, (x) => !x));
                driver.Add(new MemoryFieldLazy<bool>("FlagBlue", MemoryAddress.Dynamic, 0, 0x3E39, 1));
                driver.Add(new MemoryFieldLazy<bool>("FlagBlack", MemoryAddress.Dynamic, 0, 0x3D24, 1));
                driver.Add(new MemoryFieldLazy<bool>("Ignition", MemoryAddress.Dynamic, 0, 0xAA, 1));

                driver.Add(new MemoryFieldLazy<float>("CoordinateX", MemoryAddress.Dynamic, 0, 0x10, 4));
                driver.Add(new MemoryFieldLazy<float>("CoordinateY", MemoryAddress.Dynamic, 0, 0x18, 4));
                driver.Add(new MemoryFieldLazy<float>("CoordinateZ", MemoryAddress.Dynamic, 0, 0x14, 4));
                driver.Add(new MemoryFieldLazy<float>("Throttle", MemoryAddress.Dynamic, 0, 0x2938, 4));
                driver.Add(new MemoryFieldLazy<float>("Brake", MemoryAddress.Dynamic, 0, 0x2940, 4));
                driver.Add(new MemoryFieldLazy<float>("Fuel", MemoryAddress.Dynamic, 0, 0x315C, 4));
                driver.Add(new MemoryFieldLazy<float>("FuelCapacity", MemoryAddress.Dynamic, 0, 0x3160, 4));

                var laps = new MemoryPool("Laps", MemoryAddress.Dynamic, driver, 0x3D90, 6 * 4 * 200);
                // 200 laps, 6 floats each.
                driver.Add(laps);


                DriverPtrs.Add(driver);
            }
            var session = new MemoryPool("Session", MemoryAddress.Static, 0x00309D24, 0x1000);
            session.Add(new MemoryFieldLazy<string>("Track", MemoryAddress.Dynamic, 0, 4, 128));
            provider.Add(session);

            // From here 'application'.
            provider.Refresh();

            MemoryPool drv1 = provider.Get("Drivers").Pools["Driver0"];
            foreach(var drv in provider.Get("Drivers").Pools)
                Debug.WriteLine("#" + drv.Value.ReadAs<int>("Index") + " -> " + drv.Value.ReadAs<string>("Name"));

            MemoryPool sess = provider.Pools.Where(x => x.Name == "Session").FirstOrDefault();

            Debug.WriteLine(DriverPtrs.ReadAs<string>("Cars"));
            Debug.WriteLine(drv1.ReadAs<string>("Name"));
            Debug.WriteLine(drv1.ReadAs<double>("RPM"));
            Debug.WriteLine(drv1.ReadAs<double>("Speed"));
            Debug.WriteLine(drv1.ReadAs<int>("Gear"));
            Thread.Sleep(1000);
            Console.WriteLine("ReadMemory calls: " + r.ReadCalls);
            double tickLength = 0;
            // Speed comparisons.
            Stopwatch w = new Stopwatch();
            w.Reset();
            int[] data = new int[40 * (drivers + 2)];
            w.Start();
            for (int j = 0; j < 1000; j++) // 100Hz * 10 seconds
            {
                for (int i = 0; i < data.Length; i++)
                {
                    r.ReadInt32(0x7154C0 + i*4);
                }
            }
            w.Stop();
            Thread.Sleep(1000);
             tickLength = w.ElapsedMilliseconds/1000.0;
            Console.WriteLine(@"{0}k  ReadInt32() -> ReadMemory calls: {1} / {2}ms -> {3}ms per tick", data.Length, 1000*data.Length, w.ElapsedMilliseconds, tickLength);
            Console.WriteLine(@"CPU time from memory reading @ 100Hz -> {0}%", Math.Round(tickLength /1000.0 *10000, 2));

            w.Reset();
            w.Start();
            for (int i = 0; i < 1000; i++) // 100Hz * 10 seconds
            {
                for (int j = 0; j < drivers+2; j++)
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
            for (int i = 0; i < 1000; i++) // 100Hz * 10 seconds
            {
                provider.Refresh();

                // Thread.Sleep(20);
            }
            w.Stop();
            //Thread.Sleep(1000);
            tickLength = w.ElapsedMilliseconds/1000.0;
            Console.WriteLine(@"1k * MemoryProvider.Refresh() -> ReadMemory calls: {0} / {1}ms -> {2}ms per tick", r.ReadCalls, w.ElapsedMilliseconds, tickLength);
            Console.WriteLine(@"CPU time from memory reading @ 100Hz -> {0}%", Math.Round(tickLength * 100 / 10.0, 2));
        }

        [Test]
        public void MemoryRegions()
        {
            Stopwatch w = new Stopwatch();
            MemoryReader r = new MemoryReader();
            r.Diagnostic = true;
            r.Open(Process.GetProcessesByName("rfactor")[0]);
            var memory = new MemoryProvider(r);
            memory.Scanner.Enable();

            var pool = new MemoryPool("Test", MemoryAddress.Static, 0, 0);
            Console.WriteLine(r.Regions.Count);

            var sigs = new Dictionary<string, string>
                                                  {
                                                      {"sTimePtr", "7DXXA1????????8305"},
                                                      {"sClock", "D905????????56DD05"},
                                                      {"sSessionTime", "D81D????????DFE0F6C4017510"},
                                                      {"sPlayer Location", "A0XXXXXXXX8B0D????????F6D81BC0"},
                                                      {"dRPM", "7CD5D9XX????????518BCFD91C24E8"},
                                                      {"dMetersDriven", "D882XXXXXXXXD99E????????5E"},
                                                      {"dFlagYellow", "BC1F????????00"},
                                                      {"dLocationX", "8B4C242089XX??D9"},
                                                      {"dLocationY", "8945??8B44243089"},
                                                      {"dLocationZ", "894D??8D7DXX"},
                                                      {"dLaps", "3996????????7DXXDD"},
                                                      {"dName", "8D8418????????8DB4"},
                                                      {"dPosition", "8B8B????????5556"},
                                                      {"dGearR", "D983????????D9E1EB"}
                                                  };
            foreach(var sig in sigs)
            {
                var type = ((sig.Key.StartsWith("d")) ? MemoryAddress.Dynamic : MemoryAddress.StaticAbsolute);
                var ptr = ((sig.Key.EndsWith("Ptr")) ? new int[1] {0} : new int[0]);
                var field = new MemoryFieldSignature<float>(sig.Key.Substring(1), type, sig.Value, ptr, 32);
                pool.Add(field);
            }
            memory.Add(pool);
            w.Start();
            memory.Refresh();

            foreach(var field in memory.Get("Test").Fields)
            {
                Console.WriteLine("\"{3}\" : {0} -> {1:X}+{2:X}", field.Value.ReadAs<int>(), field.Value.Address, field.Value.Offset, field.Key);
            }
            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        private void DoAndPrintScan(MemoryReader r, MemoryRegion regionCode, string sig)
        {
            /*var results = r.Scan(regionCode, sig).Distinct(new MyQualityComprarer((a, b) => a ==  b));
            //Console.Write("Scanned "+sig + "=");
            if (results.Count() == 0) Console.WriteLine("none!");
            if (results.Count() == 1) Console.WriteLine("at 0x{0:X}", results.FirstOrDefault());
            else if(results.Count() > 1)
            {
                if (results.Count() > 10) Console.WriteLine("[" + results.Count() + "]");
                else
                {
                    Console.Write("[]");
                    foreach (var result in results)
                        Console.Write("0x{0:X}, ", result);
                    Console.WriteLine();
                }
            }*/
        }
    }

    public class MyQualityComprarer : IEqualityComparer<uint>
    {
        private Func<uint, uint, bool> func;
        public MyQualityComprarer(Func<uint, uint, bool> equality)
        {
            func = equality;
        }

        public bool Equals(uint a, uint b)
        {
            return func(a, b);
        }

        public int GetHashCode(uint obj)
        {
            return (int) obj;
        }
    }
}
