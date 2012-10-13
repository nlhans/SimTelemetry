using SimTelemetry.Objects.Garage;
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

        IGarage Garage { get; }

        MemoryPolledReader Memory { get; }
        //bool Attached { get; }
    }
}