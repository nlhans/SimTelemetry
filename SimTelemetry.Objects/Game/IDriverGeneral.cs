using System.Collections.Generic;

namespace SimTelemetry.Objects
{
    public interface IDriverGeneral
    {
        double GetSplitTime(IDriverGeneral player);
        List<ILap> GetLapTimes();
        ILap GetBestLap();
        ILap GetLapTime(int lap);

        //[Loggable(1)]
        //bool Retired { get; set; }

        /// <summary>
        /// Denotes whether the driver is in garage or is 'on track' (at the controls).
        /// </summary>
        [LogOnChange]
        [LogProperty("Driving", "Is the driver in garage(false) or on track(true) and at the controls?")]
        bool Driving { get; set; }

        [LogOnChange]
        [LogProperty("Ignition", "Indicates whether the car ignition is turned ON.")]
        bool Ignition { get; set; }

        [LogOnChange]
        [LogProperty("[DEV]MemoryBlock", "Indicates which memory block was read to the developer.")]
        int MemoryBlock { get; set; }

        [LogOnChange]
        [LogProperty("Sector", "The sector the car is currently driving in.")]
        int SectorsDriven { get; set; }

        [LogOnChange]
        [LogProperty("Active", "Is the car active?")]
        bool Active { set; get; }

        [LogOnChange]
        [LogProperty("Player", "Is the car the 'player'?")]
        bool IsPlayer { set; get; }

        [LogOnChange]
        [LogProperty("Name", "Name of the driver.")]
        string Name { set; get; }

        [Unloggable()]
        [LogProperty("[DEV]Address", "Base memory address")]
        int BaseAddress { set; get; }

        [Loggable(10)]
        [LogProperty("Coordinate X", "Coordinate X of the car.")]
        double CoordinateX { set; get; }

        [Loggable(10)]
        [LogProperty("Coordinate Y", "Coordinate Y of the car.")]
        double CoordinateY { set; get; }

        [Loggable(10)]
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

        [LogOnChange]
        [LogProperty("Fuel Capacity", "Maximum litres of fuel the car's fuel tank can contain.")]
        double Fuel_Max { set; get; }

        [LogOnChange]
        [LogProperty("Car Model", "Specific model of the car")]
        string CarModel { set; get; }

        [LogOnChange]
        [LogProperty("Car Class", "Specific class of the car")]
        string CarClass { set; get; }

        [LogOnChange]
        [LogProperty("AID: AI Control", "Indicates whether the AI is driving the car.")]
        bool Control_AI_Aid { set; get; }

        [LogOnChange]
        [LogProperty("Limiter", "Is the car's limiter ON?")]
        bool PitLimiter { set; get; }

        [LogOnChange]
        [LogProperty("In Pit Lane", "Has the car entered the pit lane?")]
        bool Pits { set; get; }

        [LogOnChange]
        [LogProperty("Headlights", "Are the headlights on?")]
        bool HeadLights { set; get; }

        [LogOnChange]
        [LogProperty("Lap no.", "Current lap no. the car is on?")]
        int Laps { set; get; }

        [LogOnChange]
        [LogProperty("Laptime Best Overall", "Overall best laptime of the session.")]
        float LapTime_Best { set; get; }

        [LogOnChange]
        [LogProperty("Laptime Last", "Laptime of previous lap")]
        float LapTime_Last { set; get; }

        [LogOnChange]
        [LogProperty("Sector 1 on Best Lap", "The sector 1 time of the best lap.")]
        float LapTime_Best_Sector1 { get; set; }

        [LogOnChange]
        [LogProperty("Sector 2 on Best Lap", "The sector 2 time of the best lap.")]
        float LapTime_Best_Sector2 { get; set; }

        [LogOnChange]
        [LogProperty("Sector 2 on Best Lap", "The sector 3 time of the best lap.")]
        float LapTime_Best_Sector3 { get; set; }

        [LogOnChange]
        [LogProperty("Sector 1 best", "The best sector 1 time of this session.")]
        float Sector_1_Best { get; set; }

        [LogOnChange]
        [LogProperty("Sector 2 best", "The best sector 2 time of this session.")]
        float Sector_2_Best { get; set; }

        [LogOnChange]
        [LogProperty("Sector 3 best", "The best sector 3 time of this session.")]
        float Sector_3_Best { get; set; }

        [LogOnChange]
        [LogProperty("Sector 1 last", "The last sector 1 time of this session.")]
        float Sector_1_Last { get; set; }

        [LogOnChange]
        [LogProperty("Sector 2 last", "The last sector 2 time of this session.")]
        float Sector_2_Last { get; set; }

        [LogOnChange]
        [LogProperty("Sector 3 last", "The last sector 3 time of this session.")]
        float Sector_3_Last { get; set; }

        [Loggable(25)]
        [LogProperty("Meters Driven", "Number of meters driven from start/finish line (per lap).")]
        double MetersDriven { get; set; }

        [LogOnChange]
        [LogProperty("Pitstops", "Numbers of pitstops made in a race.")]
        int PitStopRuns { get; set; }

        [LogOnChange]
        [LogProperty("Retired", "Indicates a retirement.")]
        bool Retired { get; set; }

        [LogOnChange]
        [LogProperty("TrackPosition", "The current sector being driven.")]
        TrackPosition TrackPosition { get; set; }

        [LogOnChange]
        [LogProperty("AID: Steering help", "Indicates the level of steering help AID active.")]
        LevelIndicator SteeringHelp { set; get; }

        [LogOnChange]
        int PitStop_FrontWingSetting { set; get; }

        [LogOnChange]
        int PitStop_RearWingSetting { set; get; }

        [LogOnChange]
        int PitStop_FuelSetting { set; get; }

        [LogOnChange]
        double FuelSetting_Offset { set; get; }

        [LogOnChange]
        double FuelSetting_Scale { set; get; }

        [LogOnChange]
        double MassEmpty { set; get; }

        [LogOnChange]
        double Mass { set; get; }

        [LogOnChange]
        double RPM_Stationary { set; get; }

        [LogOnChange]
        double RPM_Max_Offset { set; get; }

        [LogOnChange]
        double RPM_Max_Scale { set; get; }

        [Loggable(25)]
        [DisplayConversion( DataConversions.SPEED_MS_TO_KMH)]
        double Speed { set; get; }

        [Loggable(25)]
        [DisplayConversion(DataConversions.ROTATION_RADS_TO_RPM)]
        double RPM { set; get; }

        [LogOnChange]
        int Position { set; get; }

        [LogOnChange]
        int Gear { set; get; }

        [LogOnChange]
        int Gears { set; get; }

        [LogOnChange]
        float GearRatio1 { set; get; }

        [LogOnChange]
        float GearRatio2 { set; get; }

        [LogOnChange]
        float GearRatio3 { set; get; }

        [LogOnChange]
        float GearRatio4 { set; get; }

        [LogOnChange]
        float GearRatio5 { set; get; }

        [LogOnChange]
        float GearRatio6 { set; get; }

        [LogOnChange]
        float GearRatio7 { set; get; }

        [LogOnChange]
        float GearRatio8 { set; get; }

        [LogOnChange]
        float GearRatio9 { set; get; }

        [LogOnChange]
        float GearRatio10 { set; get; }

        [LogOnChange]
        float GearRatio11 { set; get; }

        [LogOnChange]
        float GearRatio12 { set; get; }

        [LogOnChange]
        float GearRatio13 { set; get; }

        [LogOnChange]
        float GearRatio14 { set; get; }

        [LogOnChange]
        float GearRatio15 { set; get; }

        [LogOnChange]
        float GearRatio16 { set; get; }

        [LogOnChange]
        float GearRatio17 { set; get; }

        [LogOnChange]
        float GearRatio18 { set; get; }

        [LogOnChange]
        float GearRatioR { set; get; }

        [Loggable(1)]
        float TyreWear_LF { set; get; }

        [Loggable(1)]
        float TyreWear_RF { set; get; }

        [Loggable(1)]
        float TyreWear_LR { set; get; }

        [Loggable(1)]
        float TyreWear_RR { set; get; }

        [LogOnChange]
        bool Flag_Blue { get; set; }
        [LogOnChange]
        bool Flag_Yellow { get; set; }
        [LogOnChange]
        bool Flag_Black { get; set; }
    }
}
