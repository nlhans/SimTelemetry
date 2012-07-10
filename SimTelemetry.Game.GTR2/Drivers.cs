using System;
using System.Collections.Generic;
using System.Timers;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.GTR2
{
    public class Drivers : IDriverCollection
    {
        const int MaxCars = 108;

        private List<IDriverGeneral> _AllDrivers = new List<IDriverGeneral>();
        public List<IDriverGeneral> AllDrivers
        {
            get { return _AllDrivers; }
        }

        public IDriverGeneral Player
        {
            get { return new Driver(0x9204B0); }
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
        private int PrevCars = 0;
        void UpdateDrivers_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (GTR2.Session.Cars != PrevCars || PrevCars != _AllDrivers.Count)
            {
                lock (_AllDrivers)
                {
                    _AllDrivers.Clear();

                    List<int> addrs = new List<int>();
                    int dpos = 0;
                    // Create XX drivers
                    for (int i = 0; i < MaxCars; i++)
                    {
                        int pos = GTR2.Game.ReadInt32(new IntPtr(0x04 * i + 0xBE23E0));
                        if (addrs.Contains(pos) == false)
                        {
                            addrs.Add(pos);
                            int d = pos - dpos;
                            dpos = pos;
                            IDriverGeneral c = new Driver(pos);
                            //if (c.Name != "Hans") continue;
                            if (c.Name != "" && c.Position > 0 && c.Position < 120)
                                _AllDrivers.Add(c);
                        }
                    }
                    if (_AllDrivers.Count == 0)
                        _AllDrivers.Add(new Driver(0x9204B0));
                }

                PrevCars = GTR2.Session.Cars;
            }
        }

    }
}