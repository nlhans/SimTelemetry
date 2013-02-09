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
            simulator.Add(new MemoryFieldLazy<int>("CarPlayer", MemoryAddress.Static, 0x31528c, 4));
            simulator.Add(new MemoryFieldLazy<int[]>("Drivers", MemoryAddress.Static, 0x315298, 0x19C));

            simulator.Add(new MemoryFieldLazy<string>("LocationGame", MemoryAddress.Static, 0x6EB320, 0, 256));

            MemoryPool session = new MemoryPool("Session", MemoryAddress.Static, 0, 0);
            session.Add(new MemoryFieldLazy<int>("Cars", MemoryAddress.Static, 0x315290, 4));
            session.Add(new MemoryFieldLazy<float>("Time", MemoryAddress.Static, 0x60022C, 4));
            session.Add(new MemoryFieldLazy<float>("Clock", MemoryAddress.Static, 0x6E2CD8, 4));

            session.Add(new MemoryFieldLazy<string>("LocationTrack", MemoryAddress.Static, 0x309D28, 0, 256));

            session.Add(new MemoryFieldLazy<bool>("IsOffline", MemoryAddress.Static, 0x315444, 1, (x) => !x));
            session.Add(new MemoryFieldLazy<bool>("IsActive", MemoryAddress.Static, 0x30FEE4, 1));
            session.Add(new MemoryFieldLazy<bool>("IsReplay", MemoryAddress.Static, 0x315444, 1));
            session.Add(new MemoryFieldFunc<bool>("IsLoading", (pool) => !pool.ReadAs<bool>("IsActive") && pool.ReadAs<int>("Cars") > 0 && pool.ReadAs<string>("LocationTrack").Length != 0 ));

            MemoryPool templateDriver = new MemoryPool("DriverTemplate", MemoryAddress.StaticAbsolute, 0, 0x5F48); // base, 0x5F48 size
            templateDriver.Add(new MemoryFieldConstant<bool>("IsActive", true));

            templateDriver.Add(new MemoryFieldLazy<int>("Index", MemoryAddress.Dynamic, 0, 0x8, 32));
            
            templateDriver.Add(new MemoryFieldLazy<string>("Name", MemoryAddress.Dynamic, 0, 0x5B08, 32));
            templateDriver.Add(new MemoryFieldLazy<string>("CarTeam", MemoryAddress.Dynamic, 0, 0x5C22, 64));
            templateDriver.Add(new MemoryFieldLazy<string>("CarModel", MemoryAddress.Dynamic, 0, 0x5C62, 64));
            templateDriver.Add(new MemoryFieldLazy<string>("CarClasses", MemoryAddress.Dynamic, 0, 0x39BC, 64));

            templateDriver.Add(new MemoryFieldLazy<float>("Meter", MemoryAddress.Dynamic, 0, 0x3D04, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("Speed", MemoryAddress.Dynamic, 0, 0x57C0, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("RPM", MemoryAddress.Dynamic, 0, 0x317C, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("RPMMax", MemoryAddress.Dynamic, 0, 0x3180, 4));
            templateDriver.Add(new MemoryFieldLazy<int>("Gear", MemoryAddress.Dynamic, 0, 0x321C, 1));

            templateDriver.Add(new MemoryFieldLazy<float>("Mass", MemoryAddress.Dynamic, 0, 0x28DC, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("Fuel", MemoryAddress.Dynamic, 0, 0x315C, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("FuelCapacity", MemoryAddress.Dynamic, 0, 0x3160, 4));

            templateDriver.Add(new MemoryFieldLazy<float>("TyreWearLF", MemoryAddress.Dynamic, 0, 0x2A34, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("TyreWearRF", MemoryAddress.Dynamic, 0, 0x2C1C, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("TyreWearLR", MemoryAddress.Dynamic, 0, 0x2E04, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("TyreWearRR", MemoryAddress.Dynamic, 0, 0x2FEC, 4));
            
            templateDriver.Add(new MemoryFieldLazy<float>("InputThrottle", MemoryAddress.Dynamic, 0, 0x2938, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("InputBrake", MemoryAddress.Dynamic, 0, 0x2940, 4));

            templateDriver.Add(new MemoryFieldLazy<float>("CoordinateX", MemoryAddress.Dynamic, 0, 0x289C, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("CoordinateY", MemoryAddress.Dynamic, 0, 0x28A4, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("CoordinateZ", MemoryAddress.Dynamic, 0, 0x28A0, 4));

            templateDriver.Add(new MemoryFieldLazy<float>("CoordinateX-Replay", MemoryAddress.Dynamic, 0, 0x10, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("CoordinateY-Replay", MemoryAddress.Dynamic, 0, 0x18, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("CoordinateZ-Replay", MemoryAddress.Dynamic, 0, 0x14, 4));

            templateDriver.Add(new MemoryFieldLazy<float>("RotationX", MemoryAddress.Dynamic, 0, 0x40, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("RotationY", MemoryAddress.Dynamic, 0, 0x48, 4));
            templateDriver.Add(new MemoryFieldLazy<float>("RotationZ", MemoryAddress.Dynamic, 0, 0x44, 4));
            templateDriver.Add(new MemoryFieldFunc<float>("Yaw", (pool) => (float)Math.Atan2(pool.ReadAs<float>("RotationX"), pool.ReadAs<float>("RotationY")), true));
            templateDriver.Add(new MemoryFieldLazy<float>("AccelerationX", MemoryAddress.Dynamic, 0, 0x57C8, 4));
           
            templateDriver.Add(new MemoryFieldLazy<int>("Pitstops", MemoryAddress.Dynamic, 0, 0x3D2C, 4));
            templateDriver.Add(new MemoryFieldLazy<int>("Position", MemoryAddress.Dynamic, 0, 0x3D20, 4));
            templateDriver.Add(new MemoryFieldLazy<int>("Laps", MemoryAddress.Dynamic, 0, 0x3CF8, 1));

            templateDriver.Add(new MemoryFieldLazy<bool>("IsAI", MemoryAddress.Dynamic, 0, 0x59C8, 1));
            templateDriver.Add(new MemoryFieldLazy<bool>("IsRetired", MemoryAddress.Dynamic, 0, 0x4160, 1));
            templateDriver.Add(new MemoryFieldLazy<bool>("IsLimiter", MemoryAddress.Dynamic, 0, 0x17B1, 1));
            templateDriver.Add(new MemoryFieldLazy<bool>("IsPits", MemoryAddress.Dynamic, 0, 0x27A8, 1));
            templateDriver.Add(new MemoryFieldLazy<bool>("IsDriving", MemoryAddress.Dynamic, 0, 0x3CBF, 1));
            templateDriver.Add(new MemoryFieldLazy<byte>("IsDisqualified", MemoryAddress.Dynamic, 0x3D24, 1,
                                                         (v) => (byte)((v == 3) ? 1 : 0)));

            templateDriver.Add(new MemoryFieldLazy<bool>("FlagYellow", MemoryAddress.Dynamic, 0, 0x104, 1, (x) => !x));
            templateDriver.Add(new MemoryFieldLazy<bool>("FlagBlue", MemoryAddress.Dynamic, 0, 0x3E39, 1));
            templateDriver.Add(new MemoryFieldLazy<bool>("FlagBlack", MemoryAddress.Dynamic, 0, 0x3D24, 1));
            templateDriver.Add(new MemoryFieldLazy<bool>("Ignition", MemoryAddress.Dynamic, 0, 0xAA, 1));

            var laps = new MemoryPool("Laps", MemoryAddress.Dynamic, templateDriver, 0x3D90, 6 * 4 * 200);
            // 200 laps, 6 floats each.
            templateDriver.Add(laps);


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
                //pool.Add(new MemoryFieldLazy<float>("", MemoryAddress.Static, 0, 4));
                // I can now add tyre temperatures, pressure, brake info etc.
                pool.Add(new MemoryFieldConstant<bool>("IsPlayer", true));
                pool.Add(new MemoryFieldLazy<double>("EngineLifetime", MemoryAddress.Static, 0x006DC0AC, 8));
                pool.Add(new MemoryFieldLazy<double>("EngineOil", MemoryAddress.Static, 0x006DC044, 8, Conversions.Kelvin2Celsius));
                pool.Add(new MemoryFieldLazy<double>("EngineWater", MemoryAddress.Static, 0x006DC084, 8, Conversions.Kelvin2Celsius));
                pool.Add(new MemoryFieldLazy<byte>("EngineMode", MemoryAddress.Static, 0x006DBF70, 1));
                pool.Add(new MemoryFieldLazy<byte>("EngineTorque", MemoryAddress.Static, 0x006DC224, 1));

                pool.Add(new MemoryFieldLazy<int>("FuelStop1", MemoryAddress.Static, 0x006E1EAC, 4));
                pool.Add(new MemoryFieldLazy<int>("FuelStop1", MemoryAddress.Static, 0x006E1EEC, 4));
                pool.Add(new MemoryFieldLazy<int>("FuelStop1", MemoryAddress.Static, 0x006E1F2C, 4));

                pool.Add(new MemoryFieldLazy<double>("RideheightLF", MemoryAddress.Static, 0x006DB778, 8));
                pool.Add(new MemoryFieldLazy<double>("RideheightRF", MemoryAddress.Static, 0x006DB780, 8));
                pool.Add(new MemoryFieldLazy<double>("RideheightLR", MemoryAddress.Static, 0x006DB788, 8));
                pool.Add(new MemoryFieldLazy<double>("RideheightRR", MemoryAddress.Static, 0x006DB790, 8));

                pool.Add(new MemoryFieldLazy<string>("TyreCompoundFront", MemoryAddress.Static, 0x006E1177, 16));
                pool.Add(new MemoryFieldLazy<string>("TyreCompoundRear", MemoryAddress.Static, 0x006E11B7, 16));

                pool.Add(new MemoryFieldLazy<double>("TyrePressureLF", MemoryAddress.Static, 0x006D9F5C, 8));
                pool.Add(new MemoryFieldLazy<double>("TyrePressureRF", MemoryAddress.Static, 0x006DA55C, 8));
                pool.Add(new MemoryFieldLazy<double>("TyrePressureLR", MemoryAddress.Static, 0x006DAB5C, 8));
                pool.Add(new MemoryFieldLazy<double>("TyrePressureRR", MemoryAddress.Static, 0x006DB15C, 8));

                pool.Add(new MemoryFieldLazy<double>("TyreSpeedLF", MemoryAddress.Static, 0x006D9C04, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreSpeedRF", MemoryAddress.Static, 0x006DA204, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreSpeedLR", MemoryAddress.Static, 0x006DA804, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreSpeedRR", MemoryAddress.Static, 0x006DAE04, 8));

                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureInsideLF", MemoryAddress.Static, 0x006D9F44, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureInsideRF", MemoryAddress.Static, 0x006DA534, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureInsideLR", MemoryAddress.Static, 0x006DAB44, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureInsideRR", MemoryAddress.Static, 0x006DB134, 8));

                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureMiddleLF", MemoryAddress.Static, 0x006D9F3C, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureMiddleRF", MemoryAddress.Static, 0x006DA53C, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureMiddleLR", MemoryAddress.Static, 0x006DAB3C, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureMiddleRR", MemoryAddress.Static, 0x006DB13C, 8));

                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureOutsideLF", MemoryAddress.Static, 0x006D9F34, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureOutsideRF", MemoryAddress.Static, 0x006DA544, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureOutsideLR", MemoryAddress.Static, 0x006DAB34, 8));
                pool.Add(new MemoryFieldLazy<double>("TyreTemperatureOutsideRR", MemoryAddress.Static, 0x006DB144, 8));

                pool.Add(new MemoryFieldLazy<int>("AerodyanmicFrontwingSetting", MemoryAddress.Static, 0x006E182C, 4));
                pool.Add(new MemoryFieldLazy<int>("AerodyanmicRearwingSetting", MemoryAddress.Static, 0x006E186C, 4));
                pool.Add(new MemoryFieldLazy<int>("AerodyanmicRadiatorSetting", MemoryAddress.Static, 0x006E18AC, 4));
                pool.Add(new MemoryFieldLazy<int>("AerodyanmicBrakeductSetting", MemoryAddress.Static, 0x006E18EC, 4));

                pool.Add(new MemoryFieldLazy<double>("BrakeTemperatureLF", MemoryAddress.Static, 0x006DA0E0, 8));
                pool.Add(new MemoryFieldLazy<double>("BrakeTemperatureRF", MemoryAddress.Static, 0x006DA6E0, 8));
                pool.Add(new MemoryFieldLazy<double>("BrakeTemperatureLR", MemoryAddress.Static, 0x006DACE0, 8));
                pool.Add(new MemoryFieldLazy<double>("BrakeTemperatureRR", MemoryAddress.Static, 0x006DB2E0, 8));

                pool.Add(new MemoryFieldLazy<double>("BrakeThicknessLF", MemoryAddress.Static, 0x006DA110, 8));
                pool.Add(new MemoryFieldLazy<double>("BrakeThicknessRF", MemoryAddress.Static, 0x006DA710, 8));
                pool.Add(new MemoryFieldLazy<double>("BrakeThicknessLR", MemoryAddress.Static, 0x006DAD10, 8));
                pool.Add(new MemoryFieldLazy<double>("BrakeThicknessRR", MemoryAddress.Static, 0x006DB310, 8));

                pool.Add(new MemoryFieldLazy<double>("InputClutch", MemoryAddress.Static, 0x006D9744, 8));
                pool.Add(new MemoryFieldLazy<double>("InputSteering", MemoryAddress.Static, 0x006D972C, 8));

                pool.Add(new MemoryFieldLazy<byte>("HelpsABS", MemoryAddress.Static, 0x006D9786, 1));
                pool.Add(new MemoryFieldLazy<byte>("HelpsTC", MemoryAddress.Static, 0x006D977E, 1));
                pool.Add(new MemoryFieldLazy<byte>("HelpsSteer", MemoryAddress.Static, 0x006D9785, 1));

                pool.Add(new MemoryFieldLazy<bool>("HelpsLock", MemoryAddress.Static, 0x006D9784, 1));
                pool.Add(new MemoryFieldLazy<bool>("HelpsSpin", MemoryAddress.Static, 0x006D9787, 1));
                pool.Add(new MemoryFieldLazy<bool>("HelpsStability", MemoryAddress.Static, 0x006D9780, 1));
                pool.Add(new MemoryFieldLazy<bool>("HelpsClutch", MemoryAddress.Static, 0x006D9785, 1));
                //pool.Add(new MemoryFieldLazy<byte>("HelpsShift", MemoryAddress.Static, 0x006D9782, 1));

            }
            else
                pool.Add(new MemoryFieldConstant<bool>("IsPlayer", false));
        }

        public void Deinitialize()
        {
            //

        }

        public bool CheckDriverQuick(MemoryProvider provider, int ptr)
        {
            var position = provider.Reader.ReadInt32(ptr + DriverPositionOffset);
            var name = provider.Reader.ReadString(ptr + NamePositionOffset, 8);
            //Debug.WriteLine(ptr +  " = P" +position.ToString("000"));
            return position>0 && name != "";
        }
    }

    public static class Conversions
    {
        public static float Kelvin2Celsius(float data) { return data - 273.15f; }
        public static double Kelvin2Celsius(double data) { return data - 273.15; }
    }
}