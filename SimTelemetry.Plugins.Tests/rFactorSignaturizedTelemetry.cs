using System;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactorSignaturizedTelemetry : IPluginTelemetryProvider
    {
        public void Initialize(MemoryProvider provider)
        {

        }

        public MemoryPool CreateDriver(MemoryPool template, bool isPlayer)
        {
            throw new NotImplementedException();
        }

        public void Deinitialize()
        {
            throw new NotImplementedException();
        }
    }
}