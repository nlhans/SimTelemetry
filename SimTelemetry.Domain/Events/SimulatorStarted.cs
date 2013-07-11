using SimTelemetry.Domain.Plugins;

namespace SimTelemetry.Domain.Events
{
    public class SimulatorStarted
    {
        public SimulatorStarted(IPluginSimulator sim)
        {
            Sim = sim;
        }

        public IPluginSimulator Sim { get; private set; }
    }
}