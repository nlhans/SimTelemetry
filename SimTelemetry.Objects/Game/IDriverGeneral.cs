using System.Collections.Generic;

namespace SimTelemetry.Objects
{
    public interface IDriverGeneral
    {
        double GetSplitTime(IDriverGeneral player);
        List<ILap> GetLapTimes();
        ILap GetBestLap();

        //[Loggable(1)]
        //bool Retired { get; set; }

        [Loggable(1)]
        [LogProperty("Ignition", "Indicates whether the car ignition is turned ON.")]
        bool Ignition { get; set; }

        [Loggable(1)]
        [LogProperty("[DEV]MemoryBlock", "Indicates which memory block was read to the developer.")]
        int MemoryBlock { get; set; }

        [Loggable(1)]
        [LogProperty("Sector", "The sector the car is currently driving in.")]
        int SectorsDriven { get; set; }

        [Loggable(1)]
        [LogProperty("Active", "Is the car active?")]
        bool Active { set; get; }

        [Loggable(0.01)]
        [LogProperty("Player", "Is the car the 'player'?")]
        bool IsPlayer { set; get; }

        [Loggable(0.05)]
        [LogProperty("Name", "Name of the driver.")]
        string Name { set; get; }

        [Unloggable()]
        [LogProperty("[DEV]Address", "Base memory address")]
        int BaseAddress { set; get; }

        [Loggable(5)]
        [LogProperty("Coordinate X", "Coordinate X of the car.")]
        double CoordinateX { set; get; }

        [Loggable(5)]
        [LogProperty("Coordinate Y", "Coordinate Y of the car.")]
        double CoordinateY { set; get; }

        [Loggable(5)]
        [LogProperty("Coordinate Z", "Coordinate Z of the car.")]
        double CoordinateZ { set; get; }

        [Loggable(25)]
        [LogProperty("Pedal Throttle", "Status of throttle pedal.")]
        double Throttle { set; get; }

        [Loggable(25)]
        [LogProperty("Pedal Brake", "Status of brake pedal.")]
        double Brake { set; get; }

        [Loggable(1)]
        [LogProperty("Fuel", "Litres of fuel currently in the car's fuel tank.")]
        double Fuel { set; get; }

        [Loggable(1)]
        [LogProperty("Fuel Capacity", "Maximum litres of fuel the car's fuel tank can contain.")]
        double Fuel_Max { set; get; }

        [Loggable(0.05)]
        [LogProperty("Car Model", "Specific model of the car")]
        string CarModel { set; get; }

        [Loggable(0.05)]
        [LogProperty("Car Class", "Specific class of the car")]
        string CarClass { set; get; }

        [Loggable(0.05)]
        [LogProperty("AID: AI Control", "Indicates whether the AI is driving the car.")]
        bool Control_AI_Aid { set; get; }

        [Loggable(1)]
        [LogProperty("Limiter", "Is the car's limiter ON?")]
        bool PitLimiter { set; get; }

        [Loggable(1)]
        [LogProperty("In Pit Lane", "Has the car entered the pit lane?")]
        bool Pits { set; get; }

        [Loggable(1)]
        [LogProperty("Headlights", "Are the headlights on?")]
        bool HeadLights { set; get; }

        [Loggable(25)]
        [LogProperty("Lap no.", "Current lap no. the car is on?")]
        int Laps { set; get; }

        [Loggable(0.05)]
        [LogProperty("Laptime Best Overall", "Overall best laptime of the session.")]
        float LapTime_Best { set; get; }

        [Loggable(1)]
        [LogProperty("Laptime Last", "Laptime of previous lap")]
        float LapTime_Last { set; get; }

        [Loggable(1)]
        [LogProperty("Sector 1 on Best Lap", "The sector 1 time of the best lap.")]
        float LapTime_Best_Sector1 { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 2 on Best Lap", "The sector 2 time of the best lap.")]
        float LapTime_Best_Sector2 { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 2 on Best Lap", "The sector 3 time of the best lap.")]
        float LapTime_Best_Sector3 { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 1 best", "The best sector 1 time of this session.")]
        float Sector_1_Best { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 2 best", "The best sector 2 time of this session.")]
        float Sector_2_Best { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 3 best", "The best sector 3 time of this session.")]
        float Sector_3_Best { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 1 last", "The last sector 1 time of this session.")]
        float Sector_1_Last { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 2 last", "The last sector 2 time of this session.")]
        float Sector_2_Last { get; set; }

        [Loggable(1)]
        [LogProperty("Sector 3 last", "The last sector 3 time of this session.")]
        float Sector_3_Last { get; set; }

        [Loggable(50)]
        [LogProperty("Meters Driven", "Number of meters driven from start/finish line (per lap).")]
        double MetersDriven { get; set; }

        [Loggable(1)]
        [LogProperty("Pitstops", "Numbers of pitstops made in a race.")]
        int PitStopRuns { get; set; }

        [Loggable(1)]
        [LogProperty("Retired", "Indicates a retirement.")]
        bool Retired { get; set; }

        [Loggable(1)]
        [LogProperty("TrackPosition", "The current sector being driven.")]
        TrackPosition TrackPosition { get; set; }

        [Loggable(0.05)]
        [LogProperty("AID: Steering help", "Indicates the level of steering help AID active.")]
        LevelIndicator SteeringHelp { set; get; }

        [Loggable(0.05)]
        int PitStop_FrontWingSetting { set; get; }

        [Loggable(0.05)]
        int PitStop_RearWingSetting { set; get; }

        [Loggable(0.05)]
        int PitStop_FuelSetting { set; get; }

        [Loggable(0.05)]
        double FuelSetting_Offset { set; get; }

        [Loggable(0.05)]
        double FuelSetting_Scale { set; get; }

        [Loggable(0.05)]
        double MassEmpty { set; get; }

        [Loggable(0.05)]
        double Mass { set; get; }

        [Loggable(0.05)]
        double RPM_Stationary { set; get; }

        [Loggable(0.05)]
        double RPM_Max_Offset { set; get; }

        [Loggable(0.05)]
        double RPM_Max_Scale { set; get; }

        [Loggable(10)]
        [DisplayConversion( DataConversions.SPEED_MS_TO_KMH)]
        double Speed { set; get; }

        [Loggable(10)]
        [DisplayConversion(DataConversions.ROTATION_RADS_TO_RPM)]
        double RPM { set; get; }

        [Loggable(0.2)]
        int Position { set; get; }

        [Loggable(10)]
        int Gear { set; get; }

        [Loggable(0.05)]
        int Gears { set; get; }

        [Loggable(0.05)]
        float GearRatio1 { set; get; }

        [Loggable(0.05)]
        float GearRatio2 { set; get; }

        [Loggable(0.05)]
        float GearRatio3 { set; get; }

        [Loggable(0.05)]
        float GearRatio4 { set; get; }

        [Loggable(0.05)]
        float GearRatio5 { set; get; }

        [Loggable(0.05)]
        float GearRatio6 { set; get; }

        [Loggable(0.05)]
        float GearRatio7 { set; get; }

        [Loggable(0.05)]
        float GearRatioR { set; get; }

        [Loggable(0.5)]
        float TyreWear_LF { set; get; }

        [Loggable(0.5)]
        float TyreWear_RF { set; get; }

        [Loggable(0.5)]
        float TyreWear_LR { set; get; }

        [Loggable(0.5)]
        float TyreWear_RR { set; get; }

        [Loggable(1)]
        bool Flag_Blue { get; set; }
        [Loggable(1)]
        bool Flag_Yellow { get; set; }
        [Loggable(1)]
        bool Flag_Black { get; set; }

    }
}
