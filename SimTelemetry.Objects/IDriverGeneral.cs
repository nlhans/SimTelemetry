using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects
{
    public interface IDriverGeneral
    {
        bool Active { get; set;  }

        [Loggable(0.01)]
        bool IsPlayer { set; get;  }

        [Loggable(0.05)]
        string Name { set; get;  }

        [Unloggable()]
        int BaseAddress { get; set;  }

        [Loggable(2)]
        double CoordinateX { // get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x289C)); }
            get; set;  }

        [Loggable(2)]
        double CoordinateY { //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A0)); }
            get; set;  }

        [Loggable(2)]
        double CoordinateZ { //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A4)); }
            get; set;  }

        [Loggable(25)]
        double Throttle { get; set;  }

        [Loggable(25)]
        double Brake { get; set;  }

        [Loggable(2)]
        double Fuel { get; set;  }

        [Loggable(0.05)]
        string CarModel { get; set;  }

        [Loggable(0.05)]
        string CarClass { get; set;  }

        [Loggable(0.2)]
        bool Control_AI_Aid { get; set;  }

        [Loggable(0.2)]
        bool PitLimiter { get; set;  }

        [Loggable(0.2)]
        bool Pits { get; set;  }

        [Loggable(0.2)]
        bool HeadLights { get; set;  }

        [Loggable(0.05)]
        int Laps { get; set;  }

        [Loggable(0.05)]
        float LapTime_Best { get; set;  }

        [Loggable(0.05)]
        float LapTime_Last { get; set;  }

        [Loggable(0.05)]
        LevelIndicator SteeringHelp { get; set;  }

        [Loggable(0.05)]
        int PitStop_FrontWingSetting { get; set;  }

        [Loggable(0.05)]
        int PitStop_RearWingSetting { get; set;  }

        [Loggable(0.05)]
        int PitStop_FuelSetting { get; set;  }

        [Loggable(0.05)]
        double FuelSetting_Offset { get; set;  }

        [Loggable(0.05)]
        double FuelSetting_Scale { get; set;  }

        [Loggable(0.05)]
        double MassEmpty { get; set;  }

        [Loggable(0.05)]
        double Mass { get; set;  }

        [Loggable(0.05)]
        double RPM_Stationary { get; set;  }

        [Loggable(0.05)]
        double RPM_Max_Offset { get; set;  }

        [Loggable(0.05)]
        double RPM_Max_Scale { get; set;  }

        [Loggable(10)]
        double Speed { get; set;  }

        [Loggable(10)]
        double RPM { get; set;  }

        [Loggable(0.2)]
        int Position { get; set;  }

        [Loggable(2)]
        int Gear { get; set;  }

        [Loggable(0.05)]
        int Gears { get; set;  }

        [Loggable(0.05)]
        float GearRatio1 { get; set;  }

        [Loggable(0.05)]
        float GearRatio2 { get; set;  }

        [Loggable(0.05)]
        float GearRatio3 { get; set;  }

        [Loggable(0.05)]
        float GearRatio4 { get; set;  }

        [Loggable(0.05)]
        float GearRatio5 { get; set;  }

        [Loggable(0.05)]
        float GearRatio6 { get; set;  }

        [Loggable(0.05)]
        float GearRatio7 { get; set;  }

        [Loggable(0.05)]
        float GearRatioR { get; set;  }

        [Loggable(1)]
        float TyreWear_LF { get; set;  }

        [Loggable(1)]
        float TyreWear_RF { get; set;  }

        [Loggable(1)]
        float TyreWear_LR { get; set;  }

        [Loggable(1)]
        float TyreWear_RR { get; set;  }
    }
}
