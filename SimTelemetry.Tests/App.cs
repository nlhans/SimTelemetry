
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Logger;
using SimTelemetry.Tests.Telemetry;

namespace SimTelemetry.Tests
{
    class App
    {

        static void Main(string[] args)
        {
            LogFileWriterTests tests = new LogFileWriterTests();
            tests.Create();

            return;
            Stopwatch w = new Stopwatch();
            w.Start();

            TelemetryTests t = new TelemetryTests();
            t.ReadFile();
            t.Continous();
            //l.TestRfactor();
            //l.TestRfactor();
            //test();
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
                var driver = new MemoryPool("Driver" + i, MemoryAddress.Dynamic, DriverPtrs, 0x14 + i*4, 0x5F48);
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
                                                          MemoryAddress.Dynamic, 0, 0x31F8 + j*4, 4));


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

                //var laps = new MemoryPool("Laps", MemoryAddress.Dynamic, driver, 0x3D90, 6*4*200);
                // 200 laps, 6 floats each.
                //driver.Add(laps);


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
            driverPtrs.Add(new MemoryFieldLazy<int>("CarViewPort", MemoryAddress.Dynamic, 0, 0, 4));
            driverPtrs.Add(new MemoryFieldLazy<int>("CarPlayer", MemoryAddress.Dynamic, 0, 0x8, 4));
            driverPtrs.Add(new MemoryFieldLazy<int>("Cars", MemoryAddress.Dynamic, 0, 0xC, 4));
            provider.Add(driverPtrs);

            // Session data
            var session = new MemoryPool("Session", MemoryAddress.Static, 0, 0);
            session.Add(new MemoryFieldLazy<bool>("IsOffline", MemoryAddress.Static, 0x315444, 1, (x) => !x));
            session.Add(new MemoryFieldLazy<bool>("IsActive", MemoryAddress.Static, 0x30FEE4, 1));
            session.Add(new MemoryFieldLazy<bool>("IsReplay", MemoryAddress.Static, 0x315444, 1));

            //session.Add(new MemoryFieldLazy<float>("Time", MemoryAddress.Static, 0x60022C, 4));
            session.Add(new MemoryFieldSignature<float>("Time", MemoryAddress.StaticAbsolute, "7DXXA1????????8305", new[] { 0 }, 4));
            session.Add(new MemoryFieldLazy<float>("Clock", MemoryAddress.Static, 0x6E2CD8, 4));

            session.Add(new MemoryFieldLazy<float>("SessionTime", MemoryAddress.Static, 0x5932EC, 4, (x) => Math.Min(48 * 3600, x)));
            session.Add(new MemoryFieldLazy<int>("SessionType", MemoryAddress.Static, 0x58696C, 4));
            session.Add(new MemoryFieldLazy<int>("SessionIndex", MemoryAddress.Static, 0x58696C, 4, (x) => ((x > 4) ? 1 : x)));
            session.Add(new MemoryFieldLazy<int>("SessionLaps", MemoryAddress.Static, 0x314654, 4));

            session.Add(new MemoryFieldLazy<string>("LocationGame", MemoryAddress.Static, 0x6EB320, 0, 256));
            session.Add(new MemoryFieldLazy<string>("LocationTrack", MemoryAddress.Static, 0x309D28, 0, 256));

            session.Add(new MemoryFieldLazy<float>("TemperatureAmbient", MemoryAddress.Static, 0x6E2CD4, 4));
            session.Add(new MemoryFieldLazy<float>("TemperatureTrack", MemoryAddress.Static, 0x6E2CD4, 4));


            provider.Add(session);
            provider.Scanner.Enable();
            provider.Refresh();
            provider.Scanner.Disable();

            int a = 0;
            int lastCars = 0;

            var sessionData = provider.Get("Session");
            var driverPool = provider.Get("Drivers");
            while (true)
            {
                Thread.Sleep(1000);
                provider.Refresh();

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
                Console.Write(" | Cars: " + driverPool.ReadAs<int>("Cars") + " | Time: " +
                              sessionData.ReadAs<int>("Time"));
                Console.Write(" | Length: " + sessionData.ReadAs<int>("SessionTime"));
                Console.WriteLine(" | Type: " + sessionData.ReadAs<SessionType>("SessionType") + " " +
                                  sessionData.ReadAs<string>("SessionIndex") + " | " + r.ReadCalls);

            }
        }
    }
}