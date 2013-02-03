using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
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

        public IPluginTelemetryProvider TelemetryProvider { get; protected set; }

        public ILazyRepositoryDataSource<Car, string> CarProvider { get; private set; }
        public ILazyRepositoryDataSource<Track, string> TrackProvider { get; private set; }

        private readonly Lazy<Simulator> Simulator = new Lazy<Simulator>(() => new rFactor());

        public TestSimulator()
        {
            GlobalEvents.Fire(new PluginTestSimulatorConstructor(), false);
        }

        public Simulator GetSimulator()
        {
            return Simulator.Value;
        }

        public void Initialize()
        {
            Debug.WriteLine("TestSimulator::Initialize()");

            CarProvider = new rFactorCarRepository();
            TrackProvider = new rFactorTrackRepository();

        }

        public void Deinitialize()
        {
            Debug.WriteLine("TestSimulator::Deinitialize()");

            CarProvider.Clear();
            TrackProvider.Clear();

            CarProvider = null;
            TrackProvider = null;
        }

        public void SimulatorStart(Process p)
        {
            var rfactorExe = new FileInfo(p.MainModule.FileName);

            if(rfactorExe.Length == 2210305)
                TelemetryProvider = new rFactorSignaturizedTelemetry();
            else
                TelemetryProvider = new rFactorStandardizedTelemetry();
        }

        public void SimulatorStop()
        {
            TelemetryProvider = null;
        }


        public bool Equals(IPluginBase other)
        {
            return other.ID == ID;
        }
    }
}
