using System.Collections.Generic;
using SimTelemetry.Domain.Plugins;

namespace SimTelemetry.Domain.Events
{
    public class PluginsLoaded
    {
        public IEnumerable<IPluginSimulator> Simulators { get; set; }
        public IEnumerable<IPluginBase> Widgets { get; set; }
        public IEnumerable<IPluginBase> Extensions { get; set; }

        public IEnumerable<IPluginSimulator> FailedSimulators { get; set; }
        public IEnumerable<IPluginBase> FailedWidgets { get; set; }
        public IEnumerable<IPluginBase> FailedExtensions { get; set; }
    }
}