using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Plugins.Tests
{
    [Export(typeof(IPluginSimulator))]
    public class TestSimulator : IPluginSimulator
    {
        public int ID { get { return 0; } }
        public string Name { get { return "Test simulator"; } }
        public string Version { get { return "0.1 alpha"; } }
        public string Author { get { return "SimTelemetry"; } }
        public string Description { get { return "Test simulator object"; } }

        public DateTime CompilationTime { get { return DateTime.Now; } }


        private Lazy<Simulator> Simulator = new Lazy<Simulator>(() => new rFactor());

        public TestSimulator()
        {
            GlobalEvents.Fire(new PluginTestSimulatorConstructor(), false);
        }

        public Simulator GetSimulator()
        {
            return Simulator.Value;
        }

        public Telemetry GetTelemetry()
        {
            throw new NotImplementedException();
        }

        public ICarDataProvider GetCarDataProvider()
        {
            throw new NotImplementedException();
        }

        public ITrackDataProvider GetTrackDataProvider()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            Debug.WriteLine("TestSimulator::Initialize()");
        }

        public void Deinitialize()
        {
            Debug.WriteLine("TestSimulator::Deinitialize()");
        }

        public bool Equals(IPluginBase other)
        {
            return other.ID == ID;
        }
    }

    public class TelemetrySupport : ITelemetrySupport
    {
        public bool Timing { get; private set; }

        // TODO: Add more support variables.
    }

    public class TelemetryAcquisition : ITelemetryAcquisition
    {
        public bool UseMemory { get; private set; }
        public bool SupportMemory { get; private set; }

        public bool UseDll { get; private set; }
        public bool SupportDll { get; private set; }
    }

    public class TelemetryGame : ITelemetryGame
    {
    }

    public class TelemetryTrack : ITelemetryTrack
    {
    }

    public class TelemetrySession : ITelemetrySession
    {
    }

    public class TelemetryDriver : ITelemetryDriver
    {
        public bool Equals(ITelemetryDriver other)
        {
            throw new NotImplementedException();
        }
    }

    public class TelemetryPlayer : ITelemetryPlayer
    {

    }

}
