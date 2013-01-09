using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Entities;

namespace SimTelemetry.Domain.Aggregates
{
    public abstract class Simulator
    {
        public string Name { get; private set; }
        public string ProcessName { get; private set; }
        public string Location { get; private set; }
        public string Version { get; private set; }

        public string Author { get; private set; }

        public IEnumerable<Mod> Mods { get; private set; }
        public IEnumerable<Track> Tracks { get; private set; }
    }

}