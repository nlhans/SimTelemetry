using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Objects
{
    public interface ISimulator
    {
        ITelemetry Host { get; set; }

        void Initialize();
        void Deinitialize();

        string Name { get; }
        string ProcessName { get; }

        SimulatorModules Modules { get; }

        IDriverCollection Drivers { get; }
        IDriverPlayer Player { get; }
        ISession Session { get; }

        MemoryPolledReader Memory { get; }
        //bool Attached { get; }
    }
}