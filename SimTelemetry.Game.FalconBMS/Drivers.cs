using System;
using System.Collections.Generic;
using System.Timers;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.FalconBMS
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
            get
            {
                IDriverGeneral drv = _Drivers.Find(delegate(IDriverGeneral idg) { if (idg == null) return false;
                    return idg.IsPlayer; });
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
            lock (_Drivers)
            {
                
            _Drivers = new List<IDriverGeneral>();
                _Drivers.Add(new DriverGeneral());
            }
        }
    }
}