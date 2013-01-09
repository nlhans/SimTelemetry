using System;
using System.Collections.Generic;
using SimTelemetry.Core.Entities;

namespace SimTelemetry.Core.Aggregates
{
    public class Simulator
    {
        public string Name { get; private set; }
        public string ProcessName { get; private set; }
        public string Location { get; private set; }
        public string Version { get; private set; }

        public string Author { get; private set; }

        public IEnumerable<Mod> Mods { get; private set; }
        public IEnumerable<Track> Tracks { get; private set; }
    }

    public class TelemetrySupport
    {
        public bool Timing { get; private set; }

        // TODO: Add more support variables.
    }

    public class TelemetryAcquisition
    {
        public bool UseMemory { get; private set; }
        public bool SupportMemory { get; private set; }

        public bool UseDLL { get; private set; }
        public bool SupportDLL { get; private set; }
    }

    public class TelemetryGame
    {
    }

    public class TelemetryTrack
    {
    }

    public class TelemetrySession
    {
    }

    public class TelemetryDriver : IEquatable<TelemetryDriver>
    {
        public bool Equals(TelemetryDriver other)
        {
            throw new NotImplementedException();
        }
    }

    public class TelemetryPlayer
    {
    }

}