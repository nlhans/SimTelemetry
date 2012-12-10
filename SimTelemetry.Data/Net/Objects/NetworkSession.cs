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
    [Serializable]
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

        public static NetworkSession Create(ISession sess)
        {
            NetworkSession nwSess = new NetworkSession();

            nwSess.GameData_TrackFile = sess.GameData_TrackFile;
            nwSess.GameDirectory = sess.GameDirectory;
            nwSess.RaceLaps = sess.RaceLaps;
            nwSess.IsRace = sess.IsRace;
            nwSess.CircuitName = sess.CircuitName;
            nwSess.TrackTemperature = sess.TrackTemperature;
            nwSess.AmbientTemperature = sess.AmbientTemperature;
            nwSess.Type = sess.Type;
            nwSess.Time = sess.Time;
            nwSess.TimeClock = sess.TimeClock;
            nwSess.Cars_InPits = sess.Cars_InPits;
            nwSess.Cars_OnTrack = sess.Cars_OnTrack;
            nwSess.Cars = sess.Cars;
            nwSess.Flag_YellowFull = sess.Flag_YellowFull;
            nwSess.Flag_Red = sess.Flag_Red;
            nwSess.Flag_Green = sess.Flag_Green;
            nwSess.Flag_Finish = sess.Flag_Finish;
            nwSess.IsOffline = sess.IsOffline;

            return nwSess;
        }
    }
}
