namespace SimTelemetry.Data
{
    /// <summary>
    /// Class containing whether a simulator is running and has a session active.
    /// </summary>
    public class Telemetry_SimState
    {
        public bool Active;
        public bool Session;
        public bool Driving;
        public int Laps;
        public Telemetry_SimState()
        {
            Laps = -1;
            Driving = false;
            Active = false;
            Session = false;
        }
    }
}