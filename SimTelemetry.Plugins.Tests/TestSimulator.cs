using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Plugins;
namespace SimTelemetry.Game.Tests
{
    [Export(typeof(ISimulator))]
    public class TestSimulator : ISimulator
    {
        public ITelemetry Host { get; set; }

        public string PluginId
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "Test simulator"; }
        }

        public string Version
        {
            get { throw new NotImplementedException(); }
        }

        public string Author
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public void Initialize()
        {
            Debug.WriteLine("TestSimulator::Initialize()");
        }

        public void Deinitialize()
        {
            Debug.WriteLine("TestSimulator::Deinitialize()");
        }

        public string ProcessName
        {
            get { return "testsim.exe"; }
        }

        public SimulatorModules Modules
        {
            get { throw new NotImplementedException(); }
        }

        public IDriverCollection Drivers
        {
            get { throw new NotImplementedException(); }
        }

        public IDriverPlayer Player
        {
            get { throw new NotImplementedException(); }
        }

        public ISession Session
        {
            get { throw new NotImplementedException(); }
        }

        public IGarage Garage
        {
            get { throw new NotImplementedException(); }
        }

        public bool Attached
        {
            get { throw new NotImplementedException(); }
        }

        public bool UseMemoryReader
        {
            get { throw new NotImplementedException(); }
        }

        public ISetup Setup
        {
            get { throw new NotImplementedException(); }
        }

        public ICar Car
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
