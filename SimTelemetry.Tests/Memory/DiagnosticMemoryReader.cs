using System;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Tests.Memory
{
    public class DiagnosticMemoryReader : MemoryReader
    {
        public override bool Read(int memoryAddress, byte[] buffer)
        {
            GlobalEvents.Fire(new MemoryReadAction(memoryAddress, buffer.Length), true);
            return base.Read(memoryAddress, buffer);
        }
        public override bool Read(IntPtr memoryAddress, byte[] buffer)
        {
            GlobalEvents.Fire(new MemoryReadAction(memoryAddress.ToInt32(), buffer.Length), true);
            return base.Read(memoryAddress, buffer);
        }

        public override byte[] Read(IntPtr memoryAddress, uint bytesToRead)
        {
            GlobalEvents.Fire(new MemoryReadAction(memoryAddress.ToInt32(), (int)bytesToRead), true);
            return base.Read(memoryAddress, bytesToRead);
        }
    }
}