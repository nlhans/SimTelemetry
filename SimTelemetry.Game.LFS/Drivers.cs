using System;
using System.Collections.Generic;
using System.Timers;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.LFS
{
    public class Drivers : IDriverCollection
    {
        public long ListPtr;
        private List<IDriverGeneral> _Drivers = new List<IDriverGeneral>();
        public List<IDriverGeneral> AllDrivers
        {
            get { return _Drivers; }
        }

        public IDriverGeneral Player
        {
            get { IDriverGeneral drv = _Drivers.Find(delegate(IDriverGeneral idg) { return idg.IsPlayer; });
            if (drv == null)
                return new DriverGeneral { BaseAddress = 0 };
            else
                return drv;
            }
        }

        public Drivers()
        {
            Timer t = new Timer {Interval = 50};
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            ListPtr = LFS.Game.ReadInt32(new IntPtr(0x0086DE90));
            int cars = LFS.Game.ReadInt32(new IntPtr(ListPtr + 0x04));

            lock (_Drivers)
            {
                List<IDriverGeneral> _DriversNew = new List<IDriverGeneral>();

                for (int i = 1; i <= cars; i++)
                {
                    int addr_ptr = LFS.Game.ReadInt32(new IntPtr(ListPtr + 0x04 + 0x04 * i));
                    IDriverGeneral drv =
                        _Drivers.Find(delegate(IDriverGeneral idg) { return idg.BaseAddress.Equals(addr_ptr); });
                    if (drv == null)
                    {
                        _DriversNew.Add(new DriverGeneral { BaseAddress = addr_ptr });
                    }
                    else
                    {
                        _DriversNew.Add(drv);
                    }
                }
                _Drivers = new List<IDriverGeneral>(_DriversNew);
            }
        }
    }
}