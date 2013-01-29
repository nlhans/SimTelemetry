
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Objects;

namespace SimTelemetry.Tests
{
    class App
    {

        static void Main(string[] args)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            MemoryTests l = new MemoryTests();
            //l.TestRfactor();
            test();
            w.Stop();

#if DEBUG
            Debug.WriteLine("Time: " + w.ElapsedMilliseconds);
#else
            Console.WriteLine("Time : " +w.ElapsedMilliseconds );
            Console.ReadLine();
#endif

        }

        static void RefreshDrivers(MemoryPool DriverPtrs)
        {

            Console.WriteLine(("updating cars"));
            // Add found drivers.
            DriverPtrs.ClearPools();

            var drivers = DriverPtrs.ReadAs<int>("Cars");
            for (var i = 0; i < drivers; i++)
            {
                var driver = new MemoryPool("Driver", MemoryAddress.Dynamic, DriverPtrs, 0x14 + i * 4, 0x5F48); // base, 0x5F48 size
                driver.Add(new MemoryFieldConstant<int>("Index", i));
                for (int k = 0; k < 25; k++)
                {
                    driver.Add(new MemoryFieldLazy<string>("Name", MemoryAddress.Dynamic, 0, 0x5B08, 32));
                    driver.Add(new MemoryFieldLazy<float>("Speed", MemoryAddress.Dynamic, 0, 0x57C0, 8, (x) => x * 3.6f));
                    driver.Add(new MemoryFieldLazy<float>("RPM", MemoryAddress.Dynamic, 0, 0xA4, 8, Rotations.Rads_RPM));
                    driver.Add(new MemoryFieldLazy<int>("Position", MemoryAddress.Dynamic, 0, 0x3D20, 4));
                    driver.Add(new MemoryFieldLazy<int>("Gear", MemoryAddress.Dynamic, 0, 0x321C, 1));
                }
                DriverPtrs.Add(driver);
            }
        }

        static void test()
        {

            MemoryReader r = new MemoryReader();
            r.Diagnostic = true;
            r.Open(Process.GetProcessesByName("rfactor")[0]);

            MemoryDataConverter.AddProvider(new MemoryDataConverterProvider<SessionType>
                                                ((data, index) =>
                                                {
                                                    var iData = data[0];

                                                    if (iData < 4 && iData != 0) return SessionType.PRACTICE;
                                                    if (iData == 5) return SessionType.QUALIFY;
                                                    if (iData == 6) return SessionType.WARMUP;
                                                    if (iData == 7) return SessionType.RACE;
                                                    return SessionType.TEST_DAY;
                                                },
                                                 (data) =>
                                                 {
                                                     var iData = data is int ? (int)data : 0;
                                                     if (iData < 4 && iData != 0) return SessionType.PRACTICE;
                                                     if (iData == 5) return SessionType.QUALIFY;
                                                     if (iData == 6) return SessionType.WARMUP;
                                                     if (iData == 7) return SessionType.RACE;
                                                     return SessionType.TEST_DAY;
                                                 }));

            // The MemoryProvider object is filled/generated inside a plugin.
            var provider = new MemoryProvider(r);

            // Driver pool
            var driverPtrs = new MemoryPool("Drivers", MemoryAddress.Static, 0x315284, 0x200);
            driverPtrs.Add(new MemoryField<int>("CarViewPort", MemoryAddress.Dynamic, 0, 0, 4));
            driverPtrs.Add(new MemoryField<int>("CarPlayer", MemoryAddress.Dynamic, 0, 0x8, 4));
            driverPtrs.Add(new MemoryField<int>("Cars", MemoryAddress.Dynamic, 0, 0xC, 4));
            provider.Add(driverPtrs);

            // Session data
            var session = new MemoryPool("Session", MemoryAddress.Static, 0x00309D24, 0x1000);
            session.Add(new MemoryFieldLazy<string>("Track", MemoryAddress.Dynamic, 0, 4, 128));
            session.Add(new MemoryFieldLazy<float>("Time", MemoryAddress.Static, 0x60022C, 4));
            session.Add(new MemoryFieldLazy<float>("Clock", MemoryAddress.Static, 0x6E2CD8, 4));

            session.Add(new MemoryField<float>("TemperatureAmbient", MemoryAddress.Static, 0x006E2CD4, 4));
            session.Add(new MemoryField<float>("TemperatureTrack", MemoryAddress.Static, 0x006E2CD4, 4));

            session.Add(new MemoryFieldLazy<float>("SessionTime", MemoryAddress.Static, 0x5932EC, 4, (x) => Math.Min(48*3600, x)));
            session.Add(new MemoryFieldLazy<int>("SessionType", MemoryAddress.Static, 0x58696C, 4));
            session.Add(new MemoryFieldLazy<int>("SessionIndex", MemoryAddress.Static, 0x58696C, 4, (x) => ((x > 4) ? 1 : x)));
            session.Add(new MemoryFieldLazy<int>("SessionLaps", MemoryAddress.Static, 0x00314654, 4));
            session.Add(new MemoryFieldLazy<bool>("IsOffline", MemoryAddress.Static, 0x00315444, 1, (x) => !x));
            session.Add(new MemoryFieldLazy<bool>("IsActive", MemoryAddress.Static, 0x30FEE4, 1));
            
            session.Add(new MemoryFieldLazy<bool>("IsReplay", MemoryAddress.Static, 0x315444, 1));

            provider.Add(session);

            provider.Refresh();

            int a = 0;
            int lastCars = 0;

            while (true)
            {
                provider.Refresh();

                var sessionData = provider.Get("Session");
                var driverPool = provider.Get("Drivers");
                var cars = driverPool.ReadAs<int>("Cars");
                if (cars != lastCars)
                    RefreshDrivers(driverPool);
                lastCars = cars;

                bool sessionActive = session.ReadAs<bool>("IsActive");
                bool sessionLoading = !sessionActive && cars > 0;
                bool sessionReplay = session.ReadAs<bool>("IsReplay");

                string status = (session.ReadAs<bool>("IsOffline") ? "OFFLINE" : " ONLINE");
                if (sessionReplay) status = "REPLAY";
                if (sessionActive == false) status = "INACTIVE";
                if (sessionLoading == true) status = "LOADING";

                Console.Write(status);
                Console.Write(" | Cars: " + driverPool.ReadAs<int>("Cars") + " | Time: " + sessionData.ReadAs<float>("Time"));
                Console.Write(" | Length: " + sessionData.ReadAs<int>("SessionTime"));
                Console.WriteLine(" | Type: " + sessionData.ReadAs<SessionType>("SessionType") + " " +sessionData.ReadAs<string>("SessionIndex") + " | " + r.ReadCalls);


                Thread.Sleep(20);
            }
        }
    }
}