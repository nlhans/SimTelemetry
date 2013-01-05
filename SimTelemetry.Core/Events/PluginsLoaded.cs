using System;
using System.Collections;
using System.Collections.Generic;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Plugins;

namespace SimTelemetry.Core.Events
{
    public class PluginsLoaded
    {
        public IEnumerable<IPlugin> Simulators { get; set; }
        public IEnumerable<IPlugin> Widgets { get; set; }
        public IEnumerable<IPlugin> Extensions { get; set; }

        public IEnumerable<IPlugin> FailedSimulators { get; set; }
        public IEnumerable<IPlugin> FailedWidgets { get; set; }
        public IEnumerable<IPlugin> FailedExtensions { get; set; }
    }
}