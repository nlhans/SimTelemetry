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
using SimTelemetry.Objects.Game;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Objects
{
    public interface ISimulator
    {
        ITelemetry Host { get; set; }

        void Initialize();
        void Deinitialize();

        string Name { get; }
        string ProcessName { get; }

        SimulatorModules Modules { get; }

        IDriverCollection Drivers { get; }
        IDriverPlayer Player { get; }
        ISession Session { get; }

        IGarage Garage { get; }


        //MemoryPolledReader Memory { get; }
        bool Attached { get; }
        bool UseMemoryReader { get; }
        ISetup Setup { get; }
        ICar Car { get; set; }
    }
}