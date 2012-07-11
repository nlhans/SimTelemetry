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

        //void Report_SimStart(ISimulator me);
        //void Report_SimStop(ISimulator me);
    }

    public interface ITrackParser
    {
        string Location { get;  }
        string LengthStr { get;  }
        string Type { get;  }
        string Name { get;  }


        event AnonymousSignal DriverLap;
        event AnonymousSignal PlayerLap;

        SectionsCollection Sections { get; set; }
        RouteCollection Route { get; set; }
        ApexCollection Apexes { get; set; }
        double Length { get; }
    }
}
