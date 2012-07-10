using System;
using System.Collections.Generic;

namespace SimTelemetry.Data.Track
{
    public struct Lap
    {
        // Game Info
        public double Sector1;
        public double Sector2;
        public double Sector3;
        public double Total;
        public double MinTime;
        public double MaxTime;
        public double Distance;

        public int LapNo;
        public int DriverNo;

        // Our info :D
        public List<double> ApexSpeeds; // speeds at each apex
        public List<double> Checkpoints; // times at each checkpoint

        public double PrevMeters;

    }
}