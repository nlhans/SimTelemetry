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

        public void CreateDriver(MemoryPool pool, bool isPlayer)
        {
            throw new NotImplementedException();
        }

        public void Deinitialize()
        {
            throw new NotImplementedException();
        }

        public bool CheckDriverQuick(MemoryProvider provider, int ptr)
        {
            return false;
        }
    }
}