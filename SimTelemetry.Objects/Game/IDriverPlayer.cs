/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
namespace SimTelemetry.Objects
{
    public interface IDriverPlayer
    {
        [Loggable(1)]
        double Engine_Lifetime { set; get; }

        [Loggable(1)]
        double Engine_Temperature_Oil { get; set; }

        [Loggable(1)]
        double Engine_Temperature_Water { get; set; }

        [LogOnChange]
        int EngineMode { get; set; }

        [LogOnChange]
        int Gear { set; get; }

        [LogOnChange]
        int PitStop_Number { set; get; }

        [Loggable(1)]
        double Fuel { set; get; }

        [LogOnChange]
        double Suspension_SpendleTorque_LF { set; get; }

        [LogOnChange]
        double Suspension_SpendleTorque_RF { set; get; }

        // TODO: Create DriverPlayerWheel
        [Loggable(25)]
        [LogOnChange]
        double Suspension_RideHeight_LF { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Suspension_RideHeight_LR { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Suspension_RideHeight_RF { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Suspension_RideHeight_RR { set; get; }

        [LogOnChange]
        string Tyre_Compound_Front { set; get; }

        [LogOnChange]
        string Tyre_Compound_Rear { set; get; }

        [LogOnChange]
        double DryWeight { get; set; }

        [Loggable(1)]
        [LogOnChange]
        double Tyre_Pressure_LF { set; get; }

        [Loggable(1)]
        [LogOnChange]
        double Tyre_Pressure_LR { set; get; }

        [Loggable(1)]
        [LogOnChange]
        double Tyre_Pressure_RF { set; get; }

        [Loggable(1)]
        [LogOnChange]
        double Tyre_Pressure_RR { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Tyre_Speed_LF { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Tyre_Speed_LR { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Tyre_Speed_RF { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Tyre_Speed_RR { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_LF_Inside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_LR_Inside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_RF_Inside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_RR_Inside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_LF_Middle { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_LR_Middle { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_RF_Middle { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_RR_Middle { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_LF_Outside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_LR_Outside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_RF_Outside { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Tyre_Temperature_RR_Outside { set; get; }

        [LogOnChange]
        double Tyre_Temperature_LF_Optimal { set; get; }

        [LogOnChange]
        double Tyre_Temperature_LR_Optimal { set; get; }

        [LogOnChange]
        double Tyre_Temperature_RF_Optimal { set; get; }

        [LogOnChange]
        double Tyre_Temperature_RR_Optimal { set; get; }

        /*
        [LogOnChange]
        double Aerodynamics_FrontWing_Downforce { set; get; }

        [LogOnChange]
        double Aerodynamics_RearWing_Downforce { set; get; }

        [LogOnChange]
        double Aerodynamics_FrontWing_Drag_Total { set; get; }

        [LogOnChange]
        double Aerodynamics_FrontWing_Drag_Base { set; get; }

        [LogOnChange]
        double Aerodynamics_RearWing_Drag_Total { set; get; }

        [LogOnChange]
        double Aerodynamics_RearWing_Drag_Base { set; get; }

        [LogOnChange]
        double Aerodynamics_LeftFender_Drag { set; get; }

        [LogOnChange]
        double Aerodynamics_RightFender_Drag { set; get; }

        [LogOnChange]
        double Aerodynamics_Body_Drag { set; get; }

        [LogOnChange]
        double Aerodynamics_Body_DragHeightDiff { set; get; }

        [LogOnChange]
        double Aerodynamics_Body_DragHeightAvg { set; get; }

        [LogOnChange]
        double Aerodynamics_Radiator_Drag { set; get; }

        [LogOnChange]
        double Aerodynamics_BrakeDuct_Drag { set; get; }
        */

        [Loggable(25)]
        [LogOnChange]
        double Engine_RPM { set; get; }

        [LogOnChange]
        double Engine_RPM_Max_Live { set; get; }

        [LogOnChange]
        double Engine_RPM_Max_Scale { set; get; }

        [LogOnChange]
        int Engine_RPM_Max_Step { set; get; }

        [Loggable(10)]
        [LogOnChange]
        double Engine_Torque { set; get; }

        [LogOnChange]
        double Powertrain_DriverDistribution { set; get; }

        [Loggable(5)]
        [LogOnChange]
        double Brake_Temperature_LF { set; get; }

        [Loggable(5)]
        [LogOnChange]
        double Brake_Temperature_LR { set; get; }

        [Loggable(5)]
        [LogOnChange]
        double Brake_Temperature_RF { set; get; }

        [Loggable(5)]
        [LogOnChange]
        double Brake_Temperature_RR { set; get; }

        [LogOnChange]
        double Brake_Thickness_LF { set; get; }

        [LogOnChange]
        double Brake_Thickness_LR { set; get; }

        [LogOnChange]
        double Brake_Thickness_RF { set; get; }

        [LogOnChange]
        double Brake_Thickness_RR { set; get; }

        [LogOnChange]
        double Brake_Torque_LF { set; get; }

        [LogOnChange]
        double Brake_Torque_LR { set; get; }

        [LogOnChange]
        double Brake_Torque_RF { set; get; }

        [LogOnChange]
        double Brake_Torque_RR { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Pedals_Clutch { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Pedals_Throttle { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Pedals_Brake { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double SteeringAngle { set; get; }

        [LogOnChange]
        LevelIndicator DrivingHelp_BrakingHelp { set; get; }

        [LogOnChange]
        LevelIndicator DrivingHelp_SteeringHelp { set; get; }

        [LogOnChange]
        LevelIndicator DrivingHelp_TractionControl { set; get; }

        [LogOnChange]
        bool DrivingHelp_OppositeLock { set; get; }

        [LogOnChange]
        bool DrivingHelp_SpinRecovery { set; get; }

        [LogOnChange]
        bool DrivingHelp_StabilityControl { set; get; }
        
        [LogOnChange]
        bool DrivingHelp_AutoClutch { set; get; }

        [Loggable(25)]
        [LogOnChange]
        double Speed { set; get; }
    }

}