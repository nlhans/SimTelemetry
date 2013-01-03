using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects.Plugins
{
    public interface IPlugin
    {
        ITelemetry Host { get; set; }

        string PluginId { get; }
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string Description { get; }

        void Initialize();
        void Deinitialize();
    }
}
