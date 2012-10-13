namespace SimTelemetry.Objects.Garage
{
    public interface IGarageTrack
    {
        string File { get; }
        string Name { get; }
        string Location { get; }
        string Type { get; }
        bool ImageCache { get; }

        double Length { get; }

        string Qualify_Day { get; }
        double Qualify_Start { get; }
        int Qualify_Laps { get; }
        int Qualify_Minutes { get; }

        string FullRace_Day { get; }
        double FullRace_Start { get; }
        int FullRace_Minutes { get; }
        int FullRace_Laps { get; }

        bool Pitlane { get; }
        int StartingGridSize { get; }
        int PitSpots { get; }

        int PitSpeed_Practice { get; }
        int PitSpeed_Race { get; }

        double Laprecord_Race_Time { get; }
        string Laprecord_Race_Driver { get; }
        double Laprecord_Qualify_Time { get; }
        string Laprecord_Qualify_Driver { get; }

        void Scan();
    }
}