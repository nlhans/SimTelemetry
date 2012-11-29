/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
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

        private List<IDriverGeneral> _AllDrivers = new List<IDriverGeneral>();

        private static Timer UpdateDrivers;
        public IDriverGeneral Player
        {
            get
            {
                try
                {
                    foreach (IDriverGeneral g in AllDrivers)
                    {
                        if (g.IsPlayer) return g;
                    }
                }catch(Exception ex)
                {
                }
                if (AllDrivers.Count == 0)
                    _AllDrivers.Add(new DriverGeneral(0x7154C0));
                return new DriverGeneral(0x7154C0);
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
            if(rFactor.Session.Cars != PrevCars || PrevCars!= _AllDrivers.Count)
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
                    if(_AllDrivers.Count == 0)
                        _AllDrivers.Add(new DriverGeneral(0x7154C0));
                }

                PrevCars = rFactor.Session.Cars;
            }
        }

        public List<IDriverGeneral> AllDrivers
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
