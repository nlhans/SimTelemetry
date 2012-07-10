namespace SimTelemetry.Data.Track
{
    public struct TrackWaypoint
    {
        public double X;
        public double Y;
        public double Z;

        public TrackRoute Route;
        public int Sector;
        public double Meters;
    }
}