using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor
{
    public class Drivers : IDriverCollection
    {
        // rFactor has support up to 108 cars?
        const int MaxCars = 108;

        private List<DriverGeneral> _AllDrivers = new List<DriverGeneral>();

        private static Timer UpdateDrivers;
        public DriverGeneral Player
        {
            get
            {
                foreach(DriverGeneral g in AllDrivers)
                {
                    if (g.IsPlayer) return g;
                }
                return null;
            }
        }

        public Drivers()
        {
            UpdateDrivers = new Timer();
            UpdateDrivers.Interval = 1000;
            UpdateDrivers.AutoReset = true;
            UpdateDrivers.Elapsed += new ElapsedEventHandler(UpdateDrivers_Elapsed);

            UpdateDrivers.Start();

            // Create XX drivers
            /*for (int i = 0; i < MaxCars; i++)
            {
                DriverGeneral c = new DriverGeneral(rFactor.Game.ReadInt32(new IntPtr(0x04 * i + 0x715298)));

                _AllDrivers.Add(c);
            }*/

            UpdateDrivers_Elapsed(null, null);

        }

        private int PrevCars = 0;
        void UpdateDrivers_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(rFactor.Session.Cars != PrevCars)
            {
                lock (_AllDrivers)
                {
                    _AllDrivers.Clear();

                    int dpos = 0;
                    // Create XX drivers
                    for (int i = 0; i < MaxCars; i++)
                    {
                        int pos = rFactor.Game.ReadInt32(new IntPtr(0x04*i + 0x715298));
                        int d = pos - dpos;
                        dpos = pos;
                        DriverGeneral c = new DriverGeneral(pos);
                        if (c.Name != "" && c.Position > 0 && c.Position < 120)
                            _AllDrivers.Add(c);
                    }
                }

                PrevCars = rFactor.Session.Cars;
            }
        }

        public List<DriverGeneral> AllDrivers
        {
         get { return _AllDrivers; }   

        }
    }

    public class Player
    {
        public DriverGeneral Generic
        {
            get { return new DriverGeneral(0); }

        }

        public DriverPlayer Specific
        {
            get { return new DriverPlayer(); }
        }
    }
}
