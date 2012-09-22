using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using Triton.Memory;

namespace SimTelemetry.Game.Rfactor
{
    public class Session : ISession
    {
        [Unloggable]
        public bool Active
        {
            get
            {
                return Cars > 0 && Time > 0;
            }
            set { }
        }

        [Unloggable]
        public string GameData_TrackFile
        {
            get
            {
                return rFactor.Game.ReadString(new IntPtr(0x00709D28), 256);
            }
            set { }
        }
        [Unloggable]
        public string GameDirectory
        {
            get
            {
                string meugen = rFactor.Game.ReadString(new IntPtr(0x00AEB320), 256);
                string rfactor_map = meugen.Substring(0, meugen.Length - Path.GetFileName(meugen).Length);
                return rfactor_map;
            }
            set { }
        }
        [Loggable(true)]
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

        public int RaceLaps
        {
            get { return rFactor.Game.ReadInt32(new IntPtr(0x00714654)); }
            set { }
        }

        [Loggable(true)]
        public string CircuitName
        {
            set { }
            get
            {
                return rFactor.Game.ReadString(new IntPtr(0x00709D3B), 128);
            }
        }

        [Loggable(true)]
        public float TrackTemperature
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(0x00AE2CD8)); }
        }

        [Loggable(true)]
        public float AmbientTemperature
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(0x00AE2CD4)); }
        }

        [Loggable(true)]
        public SessionInfo Type
        {
            set { }
            get
            {
                SessionInfo i = new SessionInfo();
                int val = rFactor.Game.ReadInt32((IntPtr)0x98696C);
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

                i.Length = rFactor.Game.ReadFloat((IntPtr)0x9932EC);

                return i;
            }
        }

        [Loggable(10)]
        public float Time
        {
            set { }
            get
            {
                return rFactor.Game.ReadFloat((IntPtr)0xA0022C);

            }
        }
        [Loggable(1)]
        public float TimeClock
        {
            set { }
            get
            {
                return rFactor.Game.ReadFloat((IntPtr)0xAE2CFC);

            }
        }

        [Loggable(1)]
        public int Cars_InPits
        {
            set { }
            get { return Cars - Cars_OnTrack; }
        }

        [Loggable(1)]
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
        [Loggable(true)]
        public int Cars
        {
            set { }
            get
            {

                return rFactor.Game.ReadByte((IntPtr)0x715290);
            }
        }

        public bool Flag_YellowFull
        {
            get
            {
                return false; return ((rFactor.Game.ReadByte(new IntPtr(0x00AE0458)) != 2) ? true:false); }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_Red
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_Green
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_Finish
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }
    }
}