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
using SimTelemetry.Game.Rfactor.MMF;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Plugins;
using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.Rfactor
{
    [Export(typeof(ISimulator))]
    public class Simulator : ISimulator
    {
        private SimulatorModules _Modules;
        public ITelemetry Host { get; set; }


        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string PluginId
        {
            get { return ""; }
        }

        public void Initialize()
        {
            new rFactor(this); // old v1.255 only, not v1.255 patch F!!!
            _Modules = new SimulatorModules();
            _Modules.DistanceOnLap = true;
            _Modules.Time_Available = true;             // The plug-in knows the session time.
            _Modules.Track_Coordinates = true;
            _Modules.Track_MapFile = true;
            _Modules.Times_LapsBasic = true;
            _Modules.Times_LastSectors = true;
            _Modules.Times_History_LapTimes = UseMemoryReader;
            _Modules.Times_History_SectorTimes = UseMemoryReader;
            _Modules.Engine_Power = UseMemoryReader;
            _Modules.Engine_PowerCurve = true;
            _Modules.Aero_Drag = UseMemoryReader;
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

        public string Version
        {
            get { return "v1.255 no-cd"; }
        }

        public string Author
        {
            get { return "H. de Jong"; }
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
        public bool Attached
        {
            get
            {
                if (UseMemoryReader)
                {
                    return Memory.Attached;
                }
                else
                {
                    return rFactor.MMF.Hooked;
                }
            }
        }
        public bool UseMemoryReader { get { return true; } }

        public ISetup Setup
        {
            get { return new rFactorSetup(); }
        }

        public ICar Car
        {
            get
            {
                if(rFactor.Drivers.Player == null)
                    return null;
                if (UseMemoryReader)
                {
                    ICar c = rFactor.Garage.SearchCar(rFactor.Drivers.Player.CarClass, rFactor.Drivers.Player.CarModel);

                    return c;
                }else
                {
                    //return rFactor.Garage.SearchCar(rFactor.MMF.Telemetry.Player.VehicleName, rFactor.Drivers.Player.CarModel);
                    return rFactor.Garage.SearchCar(rFactor.Drivers.Player.CarClass, rFactor.Drivers.Player.CarModel);
                }
            }
            set { throw new NotImplementedException(); }
        }
    }
}
