using System.Collections.Generic;
using System.Diagnostics;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactor : Simulator
    {
        protected override IEnumerable<Mod> ModScanner()
        {
            Debug.WriteLine("TestSimulator::rFactor::ModScanner()");
            GlobalEvents.Fire(new PluginTestSimulatorModScanner(), true);
            return new List<Mod>(new[] { new Mod("Test", "none", new List<string>(), new List<Season>(), new List<Car>()) });
        }
        protected override IEnumerable<Track> TrackScanner()
        {
            Debug.WriteLine("TestSimulator::rFactor::TrackScanner()");
            GlobalEvents.Fire(new PluginTestSimulatorTrackScanner(), true);
            return new List<Track>(new[] { new Track("test.rfm", "Test", "none", "Test", 100, 95, "1.0") });
        }
        
        public rFactor() : base("rFactor","rfactor","1.255","SimTelemetry","rFactor game plug-in",string.Empty)
        {
            
        }
    }
}