using System;
using Triton;

namespace SimTelemetry.Objects
{
    public interface ITelemetry
    {
        ITrackParser Track { get; set; }
        ISimulator Sim { get; }
        bool Active_Sim { get; }
        bool Active_Session { get; }

        event Signal Sim_Start;
        event Signal Sim_Stop;

        event Signal Session_Start;
        event Signal Session_Stop;

        event Signal Track_Load;
    }
}
