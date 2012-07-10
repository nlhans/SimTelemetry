using System;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.rFactor2.v1005b
{
    public class Session : ISession
    {
        public string GameData_TrackFile
        {
            get
            {
                return "F:\\rf\\PORTUGAL_GT.AIW";
                //return rFactor2.Game.ReadString(new IntPtr(0x00D3CF9C), 128);

            }
            set { throw new NotImplementedException(); }
        }

        public string GameDirectory
        {
            get
            {
                string location = rFactor2.Game.ReadString(new IntPtr(0x00410280), 128);
                location = location.Replace("Core\\rFactor2.exe", "");
                return location;
            }
            set { throw new NotImplementedException(); }
        }

        public int RaceLaps
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public bool IsRace
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public string CircuitName
        {
            get { return rFactor2.Game.ReadString(new IntPtr(0x004EED0F8), 128); }
            set { throw new NotImplementedException(); }
        }

        public float TrackTemperature
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float AmbientTemperature
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public SessionInfo Type
        {
            get { return new SessionInfo(); }
            set { throw new NotImplementedException(); }
        }

        public float Time
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x00D4BB8C)); }
            set { throw new NotImplementedException(); }
        }

        public float TimeClock
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x00D4BB8C)); }
            set { throw new NotImplementedException(); }
        }

        public int Cars_InPits
        {
            get
            {
                return rFactor2.Drivers.AllDrivers.FindAll(delegate(IDriverGeneral d)
                                                               {
                                                                   return d.Pits;
                                                               }).Count;
            }
            set { throw new NotImplementedException(); }
        }

        public int Cars_OnTrack
        {
            get
            {
                return rFactor2.Drivers.AllDrivers.FindAll(delegate(IDriverGeneral d)
                {
                    return !d.Pits;
                }).Count;
            }
            set { throw new NotImplementedException(); }
        }

        public int Cars
        {
            get { return rFactor2.Game.ReadInt32(new IntPtr(0x00D444D8)); }
            set { throw new NotImplementedException(); }
        }

        public bool Active
        {
            get { return rFactor2.Game.Attached && rFactor2.Drivers.Player != null; }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_YellowFull
        {
            get { return false; }
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