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
using System.IO;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.GTR2
{
    public class Session : ISession
    {
        public bool Active
        {
            get { return Cars > 0 && Time > 1; }
            set { }
        }

        public bool Flag_YellowFull
        {
            get { return false; }
            set { }
        }

        public bool Flag_Red
        {
            get { return false; }
            set { }
        }

        public bool Flag_Green
        {
            get { return false; }
            set { }
        }

        public bool Flag_Finish
        {
            get { return false; }
            set { }
        }

        public string GameData_TrackFile
        {
            get
            {
                return GTR2.Game.ReadString(new IntPtr(0x0091BCBC), 256);
            }
            set { }
        }

        public string GameDirectory
        {
            get
            {
                string exeat = GTR2.Game.ReadString(new IntPtr(0x00FB8510), 256);
                string map = exeat.Substring(0, exeat.Length - Path.GetFileName(exeat).Length);
                return map;
            }
            set { }
        }

        public int RaceLaps
        {
            get { return 0; }
            set { }
        }

        public bool IsRace
        {
            get { return false; }
            set { }
        }

        public string CircuitName
        {
            get { return ""; }
            set { }
        }

        public float TrackTemperature
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(0x00FB1F38)); }
            set { }
        }

        public float AmbientTemperature
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(0x00FB1F34)); }
            set { }
        }

        public SessionInfo Type
        {
            get
            {
                // TODO: Search memory addresses
                SessionInfo inf = new SessionInfo();
                inf.Type = SessionType.TEST_DAY;
                inf.Number = 1;
                inf.Length = 3600;
                return inf;
            }
            set { }
        }

        public float Time
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(0x0082F4A8)); }
            set { }
        }

        public float TimeClock
        {
            get { return 0.0f; }
            set { }
        }

        public int Cars_InPits
        {
            get { return 0; }
            set { }
        }

        public int Cars_OnTrack
        {
            get { return 0; }
            set { }
        }

        public int Cars
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(0x00757C10)); }
            set { }
        }

        public bool IsOffline
        {
            get { return true; }
            set { }
        }
    }
}