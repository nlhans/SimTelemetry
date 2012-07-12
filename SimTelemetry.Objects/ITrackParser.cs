using Triton;

namespace SimTelemetry.Objects
{
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