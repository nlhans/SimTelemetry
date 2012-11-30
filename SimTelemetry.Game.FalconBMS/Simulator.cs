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
using System.ComponentModel.Composition;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.FalconBMS
{
    [Export(typeof(ISimulator))]
    public class Simulator : ISimulator
    {
        public ITelemetry Host { get; set; }
        private SimulatorModules _Modules;
        private static MemoryPolledReader _Memory;
        public static MemoryPolledReader Game
        {
            get { return _Memory; }
        }
        public void Initialize()
        {
            _Memory = new MemoryPolledReader(this);
            new FalconBms();

            _Modules = new SimulatorModules();
            _Modules.Track_Coordinates = false;
            _Modules.Track_MapFile = false;
            _Modules.Times_LapsBasic = false;
            _Modules.Times_LastSectors = false;
            _Modules.Times_History_LapTimes = false;
            _Modules.Engine_Power = false;
            _Modules.Engine_PowerCurve = false;
            _Modules.Aero_Drag = false;
        }

        public void Deinitialize()
        {
            //rFactor.Kill();
        }

        public string ProcessName
        {
            get { return "Falcon BMS"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "FalconBMS"; }
        }

        public IDriverCollection Drivers
        {
            get { return FalconBms.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return FalconBms.Player; }
        }

        public ISession Session
        {
            get { return FalconBms.Session; }
        }

        public IGarage Garage
        {
            get { return null; }
        }

        private MemoryPolledReader Memory
        {
            get { return _Memory; }
        }
        public bool Attached { get { return Memory.Attached; } }
        public bool UseMemoryReader { get { return true; } }

        public ISetup Setup
        {
            get { return null; }
        }
    }
}
