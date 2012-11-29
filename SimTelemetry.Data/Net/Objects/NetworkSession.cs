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
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net.Objects
{
    public class NetworkSession : ISession
    {
        public string GameData_TrackFile { get; set; }
        public string GameDirectory { get; set; }
        public int RaceLaps { get; set; }
        public bool IsRace { get; set; }
        public string CircuitName { get; set; }
        public float TrackTemperature { get; set; }
        public float AmbientTemperature { get; set; }
        public SessionInfo Type { get; set; }
        public float Time { get; set; }
        public float TimeClock { get; set; }
        public int Cars_InPits { get; set; }
        public int Cars_OnTrack { get; set; }
        public int Cars { get; set; }
        //public bool Active { get; set; }
        public bool Active
        {
            get { return true; }
            set { }
        }
        public bool Flag_YellowFull { get; set; }
        public bool Flag_Red { get; set; }
        public bool Flag_Green { get; set; }
        public bool Flag_Finish { get; set; }
        public bool IsOffline { get; set; }
    }
}
