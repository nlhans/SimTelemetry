
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

            // The MemoryProvider object is filled/generated inside a plugin.
            var provider = new MemoryProvider(r);

            var DriverPtrs = new MemoryPool("Drivers", MemoryAddress.Static, 0x315284, 0x200);
            DriverPtrs.Add(new MemoryField<int>("CarViewPort", MemoryAddress.Dynamic, 0, 0, 4));
            DriverPtrs.Add(new MemoryField<int>("CarPlayer", MemoryAddress.Dynamic, 0, 0x8, 4));
            DriverPtrs.Add(new MemoryField<int>("Cars", MemoryAddress.Dynamic, 0, 0xC, 4));
            provider.Add(DriverPtrs);

            var session = new MemoryPool("Session", MemoryAddress.Static, 0x00309D24, 0x1000);
            session.Add(new MemoryFieldLazy<string>("Track", MemoryAddress.Dynamic, 0, 4, 128));
            provider.Add(session);

            // From here 'application'.
            provider.Refresh();

            int a = 0;
            int lastCars = 0;
            while(true)
            {
                provider.Refresh();

                int cars = DriverPtrs.ReadAs<int>("Cars");
                if (cars != lastCars)
                    RefreshDrivers(DriverPtrs);
                lastCars = cars;

                    /*Console.Clear();
                    foreach (MemoryPool drv in provider.Get("Drivers").Pools.OrderBy(x => x.ReadAs<int>("Position")))
                    {
                        if (drv.ReadAs<int>("Index") == 0)
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        var s = "#" + drv.ReadAs<int>("Index").ToString("000") + " -> P" + drv.ReadAs<int>("Position").ToString("000") + " | " +
                                   drv.ReadAs<int>("Gear") + "|" + drv.ReadAs<int>("Speed").ToString("000") + "| " + drv.ReadAs<int>("RPM");
                        Console.WriteLine(s);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        a += s.Length;
                    }*/

                    Thread.Sleep(20);
                }
            }
        }
    }
