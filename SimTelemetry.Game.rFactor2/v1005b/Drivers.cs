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