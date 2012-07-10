using System;
using System.Collections.Generic;
using System.Timers;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.rFactor2.v1005b
{
    public class Drivers : IDriverCollection
    {
        private List<IDriverGeneral> driver = new List<IDriverGeneral>();
        public List<IDriverGeneral> AllDrivers
        {
            get { return driver; }
        }

        public IDriverGeneral Player
        {
            get
            {
                lock(driver)
                {
                    return driver.Find(delegate(IDriverGeneral d) { return d.IsPlayer; });
                }
                
            }
        }

        private static Timer UpdateDrivers;

        public Drivers()
        {
            UpdateDrivers = new Timer();
            UpdateDrivers.Interval = 1000;
            UpdateDrivers.AutoReset = true;
            UpdateDrivers.Elapsed += new ElapsedEventHandler(UpdateDrivers_Elapsed);

            UpdateDrivers.Start();
            UpdateDrivers_Elapsed(null, null);
        }
        void UpdateDrivers_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (driver)
            {
                driver.Clear();
                for (int i = 0; i < 32; i++)
                {
                    // rfactor2.exe + 7D2648
                    int addr = rFactor2.Game.ReadInt32(new IntPtr(0x17B2648 + i*4));
                    if (addr > 0)
                    {
                        Driver d = new Driver(addr);
                        if (d.Name != "" && driver.FindAll(delegate(IDriverGeneral d2)
                                                               { return d2.BaseAddress == addr; }).Count == 0)
                        driver.Add(d);
                    }
                }
            }
        }
    }
}