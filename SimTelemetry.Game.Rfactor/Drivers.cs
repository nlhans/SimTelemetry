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
    [Serializable]
    public class Drivers : IDriverCollection
    {
        // rFactor has support up to 108 cars?
        const int MaxCars = 108;

        private List<IDriverGeneral> _AllDrivers = new List<IDriverGeneral>();

        private static Timer UpdateDrivers;
        private IDriverGeneral _player;
        public IDriverGeneral Player
        {
            get
            {
                return _player;
            }
        }

        public Drivers()
        {
            UpdateDrivers = new Timer();
            UpdateDrivers.Interval = 1000;
            UpdateDrivers.AutoReset = true;
            UpdateDrivers.Elapsed += new ElapsedEventHandler(UpdateDrivers_Elapsed);

            UpdateDrivers.Start();

            PrevCars = -1;

            UpdateDrivers_Elapsed(null, null);

        }

        private int PrevCars = 0;
        void UpdateDrivers_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (rFactor.Simulator.UseMemoryReader)
                {
                    // TODO: Match car to MMF instead
                    if (rFactor.Session.Cars != PrevCars || PrevCars != _AllDrivers.Count)
                    {
                        lock (_AllDrivers)
                        {
                            _AllDrivers.Clear();

                            int dpos = 0;
                            // Create XX drivers
                            for (int i = 0; i < MaxCars; i++)
                            {
                                int pos = rFactor.Game.ReadInt32(new IntPtr(0x04 * i + 0x715298));
                                int d = pos - dpos;
                                dpos = pos;
                                DriverGeneral c = new DriverGeneral(pos);
                                if (pos != 0 && c.Name != "" && c.Position > 0 && c.Position < 120)
                                    _AllDrivers.Add(c);
                            }
                            if (_AllDrivers.Count == 0)
                                _AllDrivers.Add(new DriverGeneral(0x7154C0));
                        }

                        if (_AllDrivers.Count(x => x.IsPlayer) == 1)
                            _player = _AllDrivers.Where(x => x.IsPlayer).FirstOrDefault();
                        else if (_AllDrivers.Count(x => x.BaseAddress == 0x7154C0) == 1)
                            _player = _AllDrivers.Where(x => x.BaseAddress == 0x7154C0).FirstOrDefault();
                        else if (_AllDrivers.Count > 0)
                            _player = _AllDrivers.FirstOrDefault();
                        else
                            _player = null;

                        PrevCars = rFactor.Session.Cars;
                    }

                }
            else
            {
                try
                {
                    lock (_AllDrivers)
                    {
                        if (rFactor.Session.Cars != PrevCars || PrevCars != _AllDrivers.Count)
                        {
                            _AllDrivers.Clear();

                            for (int i = 0; i < rFactor.MMF.Telemetry.Session.Cars; i++)
                            {
                                _AllDrivers.Add(new DriverGeneral(rFactor.MMF.Drivers[i]));
                            }
                            if (_AllDrivers.Count == 0)
                                _player = null;
                            else
                                _player = _AllDrivers.Where(x => x.IsPlayer).FirstOrDefault();

                            PrevCars = rFactor.Session.Cars;
                        }
                        else
                        {
                            for (int i = 0; i < rFactor.MMF.Telemetry.Session.Cars; i++)
                            {
                                _AllDrivers[i] = new DriverGeneral(rFactor.MMF.Drivers[i]);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }
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
