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
using System;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor
{
    [Serializable]
    public class DriverPlayer : IDriverPlayer
    {
        public double Engine_Lifetime_Live
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC0AC)); }
        }

        public double Engine_Lifetime_Typical
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF23C)); }
        }

        public double Engine_Lifetime_Variation
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF244)); }
        }

        public double Engine_Lifetime_Oil_Base
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC0C4)) - 273.15; }
        }

        public double Engine_Lifetime_RPM_Base
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF21C)); }
        }

        public double Engine_Temperature_Oil
        {
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC044)) - 273.15; }
            set { }
        }
        
        public double Engine_Temperature_Water
        {
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC084)) - 273.15; }
            set { }
        }

        
        public int Engine_BoostSetting
        {
            get { return rFactor.Game.ReadByte(new IntPtr(0x00ADBF70)); }
            set { }
        }

        
        public int Gear { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00ADC280)); } }

        // Pit stop strategy
        
        public int PitStop_Number { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE1E6C)); } }
        
        public int PitStop_FuelStop1 { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE1EAC)); } }
        
        public int PitStop_FuelStop2 { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE1EEC)); } }
        
        public int PitStop_FuelStop3 { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE1F2C)); } }

        // RAM settings
        
        public double RAM_Fuel { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBFF0)); } }
        
        public double RAM_Torque { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBF48)); } }
        
        public double RAM_Power { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB8CC)); } }

        // Fuel
        
        public double Fuel { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9A40)); } }

        // Suspension
        
        public double Suspension_SpendleTorque_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA098)); } }
        
        public double Suspension_SpendleTorque_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA698)); } }

        
        public double Suspension_RideHeight_LF_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB7FC)); } }
        
        public double Suspension_RideHeight_LR_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB80C)); } }
        
        public double Suspension_RideHeight_RF_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB804)); } }
        
        public double Suspension_RideHeight_RR_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB814)); } }

        
        public double Suspension_RideHeight_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB778)); } }
        
        public double Suspension_RideHeight_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB788)); } }
        
        public double Suspension_RideHeight_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB780)); } }
        
        public double Suspension_RideHeight_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB790)); } }

        // Tyres
        
        public string Tyre_Compound_Front
        {
            set { }
            get
            {
                return rFactor.Game.ReadString(new IntPtr(0x00AE1177), 32);
            }
        }
        
        public string Tyre_Compound_Rear { set { } get { return rFactor.Game.ReadString(new IntPtr(0x00AE11B7), 32); } }

        
        public double Tyre_Grip_Forwards_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9E28)); } }

        
        public double Weight_Rearwheel
        {
            get
            {
                int WeightDistroSteps = rFactor.Game.ReadByte(new IntPtr(0x00AE122C));
                double WeightDistroBase = rFactor.Game.ReadDouble(new IntPtr(0x00AE1218));
                double WeightDistroStep = rFactor.Game.ReadDouble(new IntPtr(0x00AE1220));
                
                // Weight distro is from rear wheels.
                double WeightDistro = WeightDistroBase + WeightDistroStep*WeightDistroSteps;

                // Fuel?
                double FuelWeight = 0.7197*this.Fuel;
                double CarWeight = this.DryWeight;

                double FuelPosition = rFactor.Game.ReadDouble(new IntPtr(0x00ADD04C));
                double WheelBase = 0;

                return 2.1 * (FuelWeight + DryWeight) / 4 * 9.81;
            }set{}
        }

        
        public double DryWeight
        {
            get { return 75 + rFactor.Game.ReadDouble(new IntPtr(0x00ADCFB4)); }
            set { }
        }

        
        public double Tyre_Pressure_Optimal_LF { set { } get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00AD9F1C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(0x00AD9F24));

                return baseval + weightmul*Weight_Rearwheel;
            return rFactor.Game.ReadDouble(new IntPtr(0x00AD9F5C));
        } }
        
        public double Tyre_Pressure_Optimal_LR
        {
            set { }
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00ADA51C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(0x00ADA524));
                return baseval + weightmul * Weight_Rearwheel;
                return rFactor.Game.ReadDouble(new IntPtr(0x00ADAB5C));
            }
        }
        
        public double Tyre_Pressure_Optimal_RF
        {
            set { }
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00ADAB1C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(0x00ADAB24));
                return baseval + weightmul * Weight_Rearwheel; rFactor.Game.ReadDouble(new IntPtr(0x00ADA55C));
            }
        }
        
        public double Tyre_Pressure_Optimal_RR
        {
            set { }
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00ADB11C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(0x00ADB124));
                return baseval + weightmul * Weight_Rearwheel; return rFactor.Game.ReadDouble(new IntPtr(0x00ADB15C));
            }
        }

        
        public double Tyre_Pressure_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9F5C)); } }
        
        public double Tyre_Pressure_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAB5C)); } }
        
        public double Tyre_Pressure_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA55C)); } }
        
        public double Tyre_Pressure_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB15C)); } }

        
        public double Tyre_Pressure_LF_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9F64)); } }
        
        public double Tyre_Pressure_LR_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAB64)); } }
        
        public double Tyre_Pressure_RF_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA564)); } }
        
        public double Tyre_Pressure_RR_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB164)); } }

        
        public double Tyre_Grip_Sidewards_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9E18)); } }
        
        public double Tyre_Grip_Sidewards_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAA18)); } }
        
        public double Tyre_Grip_Sidewards_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA418)); } }
        
        public double Tyre_Grip_Sidewards_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB018)); } }

        
        public double Tyre_Speed_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9C04)); } }
        
        public double Tyre_Speed_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA804)); } }
        
        public double Tyre_Speed_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA204)); } }
        
        public double Tyre_Speed_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAE04)); } }

        
        public double Tyre_Temperature_LF_Inside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9F44)); } }
        
        public double Tyre_Temperature_LR_Inside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAB44)); } }
        
        public double Tyre_Temperature_RF_Inside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA534)); } }
        
        public double Tyre_Temperature_RR_Inside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB134)); } }


        
        public double Tyre_Temperature_LF_Middle { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9F3C)); } }
        
        public double Tyre_Temperature_LR_Middle { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAB3C)); } }
        
        public double Tyre_Temperature_RF_Middle { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA53C)); } }
        
        public double Tyre_Temperature_RR_Middle { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB13C)); } }

        
        public double Tyre_Temperature_LF_Outside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9F34)); } }
        
        public double Tyre_Temperature_LR_Outside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAB34)); } }
        
        public double Tyre_Temperature_RF_Outside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA544)); } }
        
        public double Tyre_Temperature_RR_Outside { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB144)); } }



        
        public double Tyre_Temperature_LF_Optimal { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9EF4)); } }
        
        public double Tyre_Temperature_LR_Optimal { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAAF4)); } }
        
        public double Tyre_Temperature_RF_Optimal { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA4F4)); } }
        
        public double Tyre_Temperature_RR_Optimal { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB0F4)); } }

        
        public double Tyre_Temperature_LF_Fresh { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9EEC)); } }
        
        public double Tyre_Temperature_LR_Fresh { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAAEC)); } }
        
        public double Tyre_Temperature_RF_Fresh { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA4EC)); } }
        
        public double Tyre_Temperature_RR_Fresh { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB0EC)); } }

        // Wheels
        
        public double Wheel_Radius_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9D74)); } }
        
        public double Wheel_Radius_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA974)); } }
        
        public double Wheel_Radius_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA374)); } }
        
        public double Wheel_Radius_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAF74)); } }


        // Aerodynamics
        
        public int Aerodynamics_FrontWing_Setting { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE182C)); } }
        
        public double Aerodynamics_FrontWing_Downforce { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBA94)); } }

        
        public int Aerodynamics_RearWing_Setting { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE186C)); } }
        
        public double Aerodynamics_RearWing_Downforce { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBB88)); } }

        
        public double Aerodynamics_FrontWing_Drag_Total { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBB64)); } }
        
        public double Aerodynamics_FrontWing_Drag_Base { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB8D4)); } }

        
        public double Aerodynamics_RearWing_Drag_Total { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBC58)); } }
        
        public double Aerodynamics_RearWing_Drag_Base { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB904)); } }

        
        public double Aerodynamics_LeftFender_Drag { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADD2B4)); } }
        
        public double Aerodynamics_RightFender_Drag { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADD320)); } }

        
        public double Aerodynamics_Body_Drag { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADD5C0)); } }

        
        public double Aerodynamics_Body_DragHeightDiff { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADD5D0)); } }

        
        public double Aerodynamics_Body_DragHeightAvg { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBC80)); } }

        
        public double Aerodynamics_Radiator_Drag { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB934)); } }
        
        public int Aerodynamics_Radiator_Setting { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE18AC)); } }

        
        public double Aerodynamics_BrakeDuct_Drag { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADD794)); } }
        
        public int Aerodynamics_BrakeDuct_Setting { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE18EC)); } }

        // Engine
        
        public double Engine_RPM { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBE80)); } }


        public double Engine_RPM_Max_Live { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBF08)); } }
        
        public double Engine_RPM_Max_Scale { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AE1B60)); } }
        
        public int Engine_RPM_Max_Step { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AE1B6C)); } }

        
        public double Engine_RPM_Idle_Min { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBEA0)); } }
        
        public double Engine_RPM_Idle_Max { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBEA8)); } }

        
        public double Engine_RPM_Launch_Min { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC034)); } }
        
        public double Engine_RPM_Launch_Max { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC03C)); } }

        
        public double Engine_Idle_ThrottleGain { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBE98)); } }

        
        public double Engine_Torque_Negative { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBF28)); } }
        
        public double Engine_Torque { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC224)); } }

        // Driving wheels
        
        public PowerTrainDrivenWheels Powertrain_DrivenWheels { set { } get { return (PowerTrainDrivenWheels)rFactor.Game.ReadByte(new IntPtr(0x00ADC10C)); } }
        
        public double Powertrain_DriverDistribution { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADC130)); } }

        // Brake TypicalFailure
        
        public double Brake_TypicalFailure_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF5F8)); } }
        
        public double Brake_TypicalFailure_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF730)); } }
        
        public double Brake_TypicalFailure_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF868)); } }
        
        public double Brake_TypicalFailure_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF9A0)); } }

        // Brake temperature
        
        public double Brake_Temperature_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA0E0)); } }
        
        public double Brake_Temperature_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADACE0)); } }
        
        public double Brake_Temperature_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA6E0)); } }
        
        public double Brake_Temperature_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB2E0)); } }

        // Brake thickness
        
        public double Brake_Thickness_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA110)); } }
        
        public double Brake_Thickness_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADAD10)); } }
        
        public double Brake_Thickness_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA710)); } }
        
        public double Brake_Thickness_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB310)); } }

        // Brake OptimalTemperature
        
        public double Brake_OptimalTemperature_LF_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA0B8)); } }
        
        public double Brake_OptimalTemperature_LR_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADACB8)); } }
        
        public double Brake_OptimalTemperature_RF_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA6B8)); } }
        
        public double Brake_OptimalTemperature_RR_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB2B8)); } }

        // Brake OptimalTemperature
        
        public double Brake_OptimalTemperature_LF_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA0D0)); } }
        
        public double Brake_OptimalTemperature_LR_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADACD0)); } }
        
        public double Brake_OptimalTemperature_RF_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA6D0)); } }
        
        public double Brake_OptimalTemperature_RR_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB2D0)); } }

        // Brake torque
        
        public double Brake_Torque_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA0A8)); } }
        
        public double Brake_Torque_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADACA8)); } }
        
        public double Brake_Torque_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADA6A8)); } }
        
        public double Brake_Torque_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADB2A8)); } }

        // Clutch torque
        
        public double Clutch_Torque { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADF2A8)); } }
        
        public double Clutch_Friction { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9708)); } }

        // Controls
        
        public double Pedals_Clutch { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD9744)); } }
        
        public double Pedals_Throttle { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD96F0)); } }
        
        public double Pedals_Brake { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD973C)); } }

        
        public double SteeringAngle { set { } get { return rFactor.Game.ReadDouble(new IntPtr(0x00AD972C)); } }

        // Driving help
        
        public LevelIndicator DrivingHelp_BrakingHelp
        {
            set { }
            get
            {
                switch (rFactor.Game.ReadByte(new IntPtr(0x00AD9786)))
                {
                    case 0:
                        return LevelIndicator.Off;
                        break;
                    case 1:
                        return LevelIndicator.Low;
                        break;
                    case 2:
                        return LevelIndicator.High;
                        break;
                }

                return LevelIndicator.Off;
            }
        }
        
        public LevelIndicator DrivingHelp_SteeringHelp { set { } get { return (LevelIndicator)rFactor.Game.ReadByte(new IntPtr(0x00AD9785)); } }
        
        public LevelIndicator DrivingHelp_TractionControl { set { } get { return (LevelIndicator)rFactor.Game.ReadByte(new IntPtr(0x00AD977E)); } }
        
        public bool DrivingHelp_OppositeLock { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AD9784)) == 1; } }
        
        public bool DrivingHelp_SpinRecovery { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AD9787)) == 1; } }
        
        public bool DrivingHelp_StabilityControl { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AD9780)) == 1; } }
        
        public bool DrivingHelp_AutoClutch { set { } get { return rFactor.Game.ReadByte(new IntPtr(0x00AD9782)) == 1; } }

        
        public double Speed
        {
            set { }
            get { return rFactor.Drivers.Player.Speed; }
        }

        
        public double Brake_InitialThickness_LF
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00AE0798));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(0x00AE07A0));
                int steps_LF = rFactor.Game.ReadByte(new IntPtr(0x00AE07AC));
                return baseval + stepval * steps_LF;
            }
            set { }
        }

        
        public double Brake_InitialThickness_RF
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00AE0A98));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(0x00AE0AA0));
                int steps_RF = rFactor.Game.ReadByte(new IntPtr(0x00AE0AAC));
                return baseval + stepval * steps_RF;
            }
            set { }
        }

        
        public double Brake_InitialThickness_LR
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00AE0D98));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(0x00AE0DA0));
                int steps_LR = rFactor.Game.ReadByte(new IntPtr(0x00AE0DAC));
                return baseval + stepval * steps_LR;
            }
            set { }
        }

        
        public double Brake_InitialThickness_RR
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(0x00AE1098));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(0x00AE10A0));
                int steps_RR = rFactor.Game.ReadByte(new IntPtr(0x00AE10AC));
                return baseval + stepval * steps_RR;
            }
            set { }
        }

        
        public double SpeedSlipping
        {
            get { return rFactor.Game.ReadDouble(new IntPtr(0x00ADBFE8)); }
            set { }
        }
    }
}