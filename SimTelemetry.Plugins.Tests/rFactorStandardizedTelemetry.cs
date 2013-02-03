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
        private int NamePositionOffset = 0;

        public void Initialize(MemoryProvider provider)
        {
            MemoryPool simulator = new MemoryPool("Simulator", MemoryAddress.Static, 0, 0);
            //simulator.Add(new MemoryFieldSignature<int>("Player", MemoryAddress.StaticAbsolute, "A0XXXXXXXX8B0D????????F6D81BC0", new int[1] { 0 }, 4));
            simulator.Add(new MemoryFieldLazy<int>("CarPlayer", MemoryAddress.StaticAbsolute, 0, 0x715328C, 4));
            simulator.Add(new MemoryField<int[]>("Drivers", MemoryAddress.StaticAbsolute, 0x715298, 0x19C));

            MemoryPool session = new MemoryPool("Session", MemoryAddress.Static, 0, 0);
            session.Add(new MemoryField<int>("Cars", MemoryAddress.StaticAbsolute, 0x715290, 4));
            session.Add(new MemoryFieldLazy<float>("Time", MemoryAddress.Static, 0x60022C, 4));
            session.Add(new MemoryFieldLazy<float>("Clock", MemoryAddress.Static, 0x6E2CD8, 4));
            //session.Add(new MemoryFieldSignature<float>("Time", MemoryAddress.StaticAbsolute, "7DXXA1????????8305", new int[1] { 0 }, 4));
            //session.Add(new MemoryFieldSignature<float>("Clock", MemoryAddress.StaticAbsolute, "D905????????56DD05", new int[0], 4, (x) => x * 3600));

            MemoryPool templateDriver = new MemoryPool("DriverTemplate", MemoryAddress.StaticAbsolute, 0, 0x5F48);

            //templateDriver.Add(new MemoryFieldSignature<int>("Position", MemoryAddress.Dynamic, "8B8B????????5556", new int[0], 4));
            //templateDriver.Add(new MemoryFieldSignature<float>("RPM", MemoryAddress.Dynamic, "7CD5D9XX????????518BCFD91C24E8", new int[0], 4, Rotations.Rads_RPM));
            //templateDriver.Add(new MemoryFieldSignature<float>("Speed", MemoryAddress.Dynamic, "D88EXXXXXXXXDEC1D99E????????0F85XXXXXXXX8B8E", new int[0], 4));
            templateDriver.Add(new MemoryFieldLazy<int>("Position", MemoryAddress.Dynamic, 0, 0x3D20, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("RPM", MemoryAddress.Dynamic, 0, 0x317C, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("Meter", MemoryAddress.Dynamic, 0, 0x3D04, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("Speed", MemoryAddress.Dynamic, 0, 0x57C0, 4));
            templateDriver.Add(new MemoryFieldLazy<int>("Gear", MemoryAddress.Dynamic, 0, 0x321C, 1));
            templateDriver.Add(new MemoryFieldLazy<string>("Name", MemoryAddress.Dynamic, 0, 0x5B08, 32));

            provider.Add(simulator);
            provider.Add(session);
            provider.Add(templateDriver);

            provider.Refresh();
            templateDriver.SetTemplate(true);

            DriverPositionOffset = templateDriver.Fields["Position"].Offset;
            NamePositionOffset = templateDriver.Fields["Name"].Offset;
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
            var name = provider.Reader.ReadString(ptr + NamePositionOffset, 8);
            Debug.WriteLine(ptr +  " = P" +position.ToString("000"));
            return position>0 && name != "";
        }
    }
}