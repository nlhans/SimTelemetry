using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using Triton.Memory;

namespace SimTelemetry.Game.Rfactor
{
    public class Session : ISession
    {
        [Loggable(0.1)]
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

        [Loggable(0.1)]
        public string CircuitName
        {
            set { }
            get
            {
                return rFactor.Game.ReadString(new IntPtr(0x00709D3B), 128);
            }
        }

        [Loggable(0.1)]
        public float TrackTemperature
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(0x00AE2CD8)); }
        }

        [Loggable(0.1)]
        public float AmbientTemperature
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(0x00AE2CD4)); }
        }

        [Loggable(0.1)]
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
                    if (d.Active && d.Pits) count++;
                }
                return count;
            }
        }
        [Loggable(0.1)]
        public int Cars
        {
            set { }
            get
            {

                return rFactor.Game.ReadByte((IntPtr)0x715290);
            }
        }
    }
}