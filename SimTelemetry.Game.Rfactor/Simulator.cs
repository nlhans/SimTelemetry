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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.Rfactor
{
    [Export(typeof(ISimulator))]
    public class Simulator : ISimulator
    {
        private SimulatorModules _Modules;
        public ITelemetry Host { get; set; }

        public void Initialize()
        {
            new rFactor(this);
            _Modules = new SimulatorModules();
            _Modules.Time_Available = true;             // The plug-in knows the session time.
            _Modules.Track_Coordinates = true;
            _Modules.Track_MapFile = true;
            _Modules.Times_LapsBasic = true;
            _Modules.Times_LastSectors = true;
            _Modules.Times_History_LapTimes = true;
            _Modules.Times_History_SectorTimes = true;
            _Modules.Engine_Power = true;
            _Modules.Engine_PowerCurve = true;
            _Modules.Aero_Drag = true;
        }

        public void Deinitialize()
        {
            rFactor.Kill();
        }

        public string ProcessName
        {
            get { return "rfactor"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "rFactor"; }
        }

        public IDriverCollection Drivers
        {
            get { return rFactor.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return rFactor.Player; }
        }

        public ISession Session
        {
            get { return rFactor.Session; }
        }

        public IGarage Garage
        {
            get { return rFactor.Garage; }
        }

        private MemoryPolledReader Memory
        {
            get { return rFactor.Game; }
        }
        public bool Attached { get { return Memory.Attached; } }
        public bool UseMemoryReader { get { return true; } }
    }
}
