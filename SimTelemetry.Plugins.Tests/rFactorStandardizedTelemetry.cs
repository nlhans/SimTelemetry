using System;
using System.Diagnostics;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Objects;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactorStandardizedTelemetry : IPluginTelemetryProvider
    {
        private int DriverPositionOffset = 0;

        public void Initialize(MemoryProvider provider)
        {
            MemoryPool simulator = new MemoryPool("Simulator", MemoryAddress.Static, 0, 0);
            simulator.Add(new MemoryFieldSignature<int>("Player", MemoryAddress.StaticAbsolute, "A0XXXXXXXX8B0D????????F6D81BC0", new int[1] { 0 }, 4));
            simulator.Add(new MemoryField<int[]>("Drivers", MemoryAddress.StaticAbsolute, 0x715298, 0x19C));

            MemoryPool session = new MemoryPool("Session", MemoryAddress.Static, 0, 0);
            session.Add(new MemoryFieldLazy<int>("Cars", MemoryAddress.Static, 0, 0xC + 0x315284, 4));
            session.Add(new MemoryFieldSignature<float>("Time", MemoryAddress.StaticAbsolute, "7DXXA1????????8305", new int[1] { 0 }, 4));
            session.Add(new MemoryFieldSignature<float>("Clock", MemoryAddress.StaticAbsolute, "D905????????56DD05", new int[0], 4, (x) => x * 3600));

            MemoryPool templateDriver = new MemoryPool("DriverTemplate", MemoryAddress.StaticAbsolute, 0, 0x5F48);

            templateDriver.Add(new MemoryFieldSignature<int>("Position", MemoryAddress.Dynamic, "8B8B????????5556", new int[0], 4));
            templateDriver.Add(new MemoryFieldSignature<float>("RPM", MemoryAddress.Dynamic, "7CD5D9XX????????518BCFD91C24E8", new int[0], 4, Rotations.Rads_RPM));
            templateDriver.Add(new MemoryFieldSignature<float>("Speed", MemoryAddress.Dynamic, "D88EXXXXXXXXDEC1D99E????????0F85XXXXXXXX8B8E", new int[0], 4));

            provider.Add(simulator);
            provider.Add(session);
            provider.Add(templateDriver);

            provider.Refresh();
            templateDriver.SetTemplate(true);

            DriverPositionOffset = templateDriver.Fields["Position"].Offset;
        }

        public void CreateDriver(MemoryPool pool, bool isPlayer)
        {
            if (isPlayer)
            {
                // I can now add tyre temperatures, pressure, brake info etc.
            }
        }

        public void Deinitialize()
        {
            //

        }

        public bool CheckDriverQuick(MemoryProvider provider, int ptr)
        {
            var position = provider.Reader.ReadInt32(ptr + DriverPositionOffset);
            Debug.WriteLine(ptr +  " = P" +position.ToString("000"));
            return position>0;
        }
    }
}