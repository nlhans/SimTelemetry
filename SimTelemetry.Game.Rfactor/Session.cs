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

namespace SimTelemetry.Game.Rfactor
{
    public class Session : ISession
    {
        public bool Active
        {
            get
            {
                // In addition to checking obvious Active Session remarks, check some game data
                // to verify a session is actual running.
                // Problem: during loading rFactor may seem to run a Practice session for 1.5 seconds
                // Addresses tried:
                // 0x0070FE9C  0x0070FEA4 0x00AE396C - Int32 In game - still bugs during loading
                // 0x0070F0FC - Int32 Bugs
                // 0x0070D7D8, 0x0070FEE4 - Int8, AND combination seems to work?

                int b = rFactor.Game.ReadByte((IntPtr)(rFactor.Game.Base + 0x0030D7D8));
                int c = rFactor.Game.ReadByte((IntPtr)(rFactor.Game.Base + 0x0030FEE4));
                bool a = (Cars > 0 && Time > 0 && b != 0 && c != 0);

                return a;
            }
            set { }
        }

        public string GameData_TrackFile
        {
            get
            {
                return rFactor.Game.ReadString((IntPtr)(rFactor.Game.Base + 0x00309D28), 256);
            }
            set { }
        }

        public string GameDirectory
        {
            get
            {
                string rfactor_exe = rFactor.Game.ReadString((IntPtr)(rFactor.Game.Base + 0x006EB320), 256);
                string rfactor_map = rfactor_exe.Substring(0, rfactor_exe.Length - Path.GetFileName(rfactor_exe).Length);
                return rfactor_map;
            }
            set { }
        }

        public bool IsRace
        {
            set { }
            get
            {
                SessionInfo i = this.Type;
                if (i.Type == SessionType.RACE)
                    return true;
                return false;
            }
        }

        public bool IsOffline
        {
            // option 2: 0xA77912 
            set { }
            get { return ((rFactor.Game.ReadByte((IntPtr)(rFactor.Game.Base + 0x00315444)) == 0) ? true : false); }
        }

        public int RaceLaps
        {
            get { return rFactor.Game.ReadInt32((IntPtr)(rFactor.Game.Base + 0x00314654)); }
            set { }
        }

        public string CircuitName
        {
            set { }
            get { return rFactor.Game.ReadString((IntPtr) (rFactor.Game.Base + 0x00309D3B), 128); }
        }

        public float TrackTemperature
        {
            set { }
            get { return rFactor.Game.ReadFloat((IntPtr)(rFactor.Game.Base + 0x006E2CD8)); }
        }

        public float AmbientTemperature
        {
            set { }
            get { return rFactor.Game.ReadFloat((IntPtr)(rFactor.Game.Base + 0x006E2CD4)); }
        }

        public SessionInfo Type
        {
            set { }
            get
            {
                SessionInfo i = new SessionInfo();
                int val = rFactor.Game.ReadInt32((IntPtr)(rFactor.Game.Base + 0x68696C));
                switch (val)
                {
                    case 0x00:
                        i.Type = SessionType.TEST_DAY;
                        i.Number = 1;
                        break;

                    case 0x01:
                        i.Type = SessionType.PRACTICE;
                        i.Number = 1;
                        break;

                    case 0x02:
                        i.Type = SessionType.PRACTICE;
                        i.Number = 2;
                        break;

                    case 0x03:
                        i.Type = SessionType.PRACTICE;
                        i.Number = 3;
                        break;

                    case 0x04:
                        i.Type = SessionType.PRACTICE;
                        i.Number = 4;
                        break;

                    case 0x05:
                        i.Type = SessionType.QUALIFY;
                        i.Number = 1;
                        break;

                    case 0x06:
                        i.Type = SessionType.WARMUP;
                        i.Number = 1;
                        break;

                    case 0x07:
                        i.Type = SessionType.RACE;
                        i.Number = 1;
                        break;

                }

                i.Length = rFactor.Game.ReadFloat((IntPtr)(rFactor.Game.Base + 0x6932EC));

                return i;
            }
        }

        public float Time
        {
            set { }
            get { return rFactor.Game.ReadFloat((IntPtr) (rFactor.Game.Base + 0x60022C)); }
        }

        public float TimeClock
        {
            set { }
            get { return rFactor.Game.ReadFloat((IntPtr)(rFactor.Game.Base + 0x6E2CFC)); }
        }

        public int Cars_InPits
        {
            set { }
            get { return Cars - Cars_OnTrack; }
        }

        public int Cars_OnTrack
        {
            set { }
            get
            {
                int count = 0;
                foreach (DriverGeneral d in rFactor.Drivers.AllDrivers)
                {
                    if (d != null && d.Active && d.Pits) count++;
                }
                return count;
            }
        }

        public int Cars
        {
            set { }
            get { return rFactor.Game.ReadByte((IntPtr)(rFactor.Game.Base + 0x315290)); }

        }

        public bool Flag_YellowFull
        {
            get
            {
                return false;
                return ((rFactor.Game.ReadByte(new IntPtr(0x00AE0458)) != 2) ? true : false);
            }
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
    }
}