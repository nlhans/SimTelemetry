using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Entities;

namespace SimTelemetry.Domain.Aggregates
{
    public abstract class Simulator
    {
        private readonly Lazy<IEnumerable<Mod>> _mods;
        private readonly Lazy<IEnumerable<Track>> _tracks;

        public string Name { get; private set; }
        public string ProcessName { get; private set; }
        public string Version { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }

        public IEnumerable<Mod> Mods { get { return _mods.Value; } }
        public IEnumerable<Track> Tracks { get { return _tracks.Value; } }

        protected Simulator(string name, string processName, string version, string author, string description, string location)
        {
            _mods = new Lazy<IEnumerable<Mod>>(ModScanner);
            _tracks = new Lazy<IEnumerable<Track>>(TrackScanner);

            Name = name;
            ProcessName = processName;
            Version = version;
            Author = author;
            Description = description;
            Location = location;
        }

        protected virtual IEnumerable<Mod> ModScanner()
        {
            return new List<Mod>();
        }
        protected virtual IEnumerable<Track> TrackScanner()
        {
            return new List<Track>();
        }


    }

}