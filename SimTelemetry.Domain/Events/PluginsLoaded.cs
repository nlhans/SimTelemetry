using System.Collections.Generic;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Objects.Plugins;

namespace SimTelemetry.Domain.Events
{
    public class PluginsLoaded
    {
        public IEnumerable<IPluginSimulator> Simulators { get; set; }
        public IEnumerable<IPlugin> Widgets { get; set; }
        public IEnumerable<IPlugin> Extensions { get; set; }

        public IEnumerable<IPluginSimulator> FailedSimulators { get; set; }
        public IEnumerable<IPlugin> FailedWidgets { get; set; }
        public IEnumerable<IPlugin> FailedExtensions { get; set; }
    }
}