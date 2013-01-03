using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Game.Rfactor.Versions
{
    public class rFactor_v1255b : IVersionedSimulatorMemory
    {
        public IVersionedDriver Driver { get { return new rFactor_v1255b_Driver(); } }
        public IVersionedPlayer Player { get { return new rFactor_v1255b_Player(); } }
        public IVersionedSession Session { get { return new rFactor_v1255b_Session(); } }
    }

    public class rFactor_v1255b_Driver : IVersionedDriver
    {
        public const int _MemoryBlockSize = 0x5F48;
        public const int _MemoryBlockOffset = 0x7154C0;

        public const int IsPlayer = 0; // TODO: Find this., although it is typically matched on BaseAddress.
        public const int AidAI = 0x1FB4;
        public const int Name = 0x5B08;
        public const int CarModel = 0x5C82;
        public const int CarClass = 0x39BC; //
        public const int Coordinate_X = 0x10; //
        public const int Coordinate_Y = 0x18; //
        public const int Coordinate_Z = 0x14; //

        public const int Laps = 0x3CF8;

        public const int Throttle = 0xA8;
        public const int Brake = 0x2940;

        public const int RPM = 0xA4; //
        public const int Speed = 0x57C0; // 
        public const int Fuel = 0x315C;

        public const int MetersDrivenOnLap = 0x3D04;

        public const int PitLimiter = 0x17B1;
        public const int Pits = 0x27A8;
        public const int Pitstops = 0x3D2C;
        public const int Retired = 0x4160;
        public const int Headlights = 0x17AD;
        public const int FuelTank = 0x3160;
        public const int RPM_Max = 0; // TODO: Find this.

        public const int Mass = 0x28DC;
        public const int Position = 0x3D20;
        public const int Gear = 0x321C;

        public const int Driving = 0x3CBF;
        public const int Ignition = 0xAA;

        public const int GearRatio1 = 0x31F8 + 0x4 * 1;
        public const int GearRatio2 = 0x31F8 + 0x4 * 2;
        public const int GearRatio3 = 0x31F8 + 0x4 * 3;
        public const int GearRatio4 = 0x31F8 + 0x4 * 4;
        public const int GearRatio5 = 0x31F8 + 0x4 * 5;
        public const int GearRatio6 = 0x31F8 + 0x4 * 6;
        public const int GearRatio7 = 0x31F8 + 0x4 * 7;
        public const int GearRatioR = 0x31F8 ;

        public const int TyreWear_LF = 0x2A34;
        public const int TyreWear_RF = 0x2C1C;
        public const int TyreWear_LR = 0x2E04;
        public const int TyreWear_RR = 0x2FEC;

        public const int FlagYellow = 0; // TODO: Find this.
        public const int FlagBlue = 0x3E39;
        public const int FlagBlack = 0x3D24;

        public const int LapsTable = 0x3D90;
    }

    public class rFactor_v1255b_Player : IVersionedPlayer
    {

    }
    public class rFactor_v1255b_Session : IVersionedSession
    {

    }
    public interface IVersionedSimulatorMemory
    {
        IVersionedDriver Driver { get; }
        IVersionedPlayer Player { get; }
        IVersionedSession Session { get; }

    }
    public class IVersionedPlayer
    {

    }
    public class IVersionedDriver
    {
        public int RPM;
        public int Speed;

    }
    public class IVersionedSession
    {

    }
}