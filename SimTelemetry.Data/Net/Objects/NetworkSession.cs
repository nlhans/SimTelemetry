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
