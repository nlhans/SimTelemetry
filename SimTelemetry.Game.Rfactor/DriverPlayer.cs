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
        public double Engine_Lifetime
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return 0;
                //return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC0AC));
                else return 0; // TODO: Make our own lifetime tracker or something?
            }
        }

        public double Engine_Lifetime_Typical
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DF23C)); }
        }

        public double Engine_Lifetime_Variation
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DF244)); }
        }

        public double Engine_Lifetime_Oil_Base
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC0C4)) - 273.15; }
        }

        public double Engine_Lifetime_RPM_Base
        {
            set { }
            get
            {
                return Engine_RPM_Max_Live; return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DF21C)); }
        }

        public double Engine_Temperature_Oil
        {
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.EngineTemp_Oil; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC044)) - 273.15;
            }
            set { }
        }
        
        public double Engine_Temperature_Water
        {
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.EngineTemp_Water;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC084)) - 273.15; }
            set { }
        }

        
        public int EngineMode
        {
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006DBF70));
            }
            set { }
        }


        public int Gear { set { } get
            {
            //    if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Gear; 
            return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006DC280));
            }
        }

        // Pit stop strategy

        public int PitStop_Number
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E1E6C));
            }
        }

        public int PitStop_FuelStop1 { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E1EAC)); } }

        public int PitStop_FuelStop2 { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E1EEC)); } }

        public int PitStop_FuelStop3 { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E1F2C)); } }

        // RAM settings

        public double RAM_Fuel { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBFF0)); } }

        public double RAM_Torque { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBF48)); } }

        public double RAM_Power { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB8CC)); } }

        // Fuel

        public double Fuel { set { } get
        {
            //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Fuel; 
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9A40));
           } }

        // Suspension

        public double Suspension_SpendleTorque_LF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA098));
            }
        }

        public double Suspension_SpendleTorque_RF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA698));
            }
        }


        public double Suspension_RideHeight_LF_G
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB7FC));
            }
        }

        public double Suspension_RideHeight_LR_G
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB80C));
            }
        }

        public double Suspension_RideHeight_RF_G
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB804));
            }
        }

        public double Suspension_RideHeight_RR_G
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB814));
            }
        }


        public double Suspension_RideHeight_LF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB778));
            }
        }

        public double Suspension_RideHeight_LR
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB788));
            }
        }

        public double Suspension_RideHeight_RF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB780));
            }
        }

        public double Suspension_RideHeight_RR
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB790));
            }
        }

        // Tyres
        
        public string Tyre_Compound_Front
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return "Tyre";
                return rFactor.Game.ReadString(new IntPtr(rFactor.Game.Base + 0x006E1177), 32);
            }
        }

        public string Tyre_Compound_Rear
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return "Tyre"; 
                return rFactor.Game.ReadString(new IntPtr(rFactor.Game.Base + 0x006E11B7), 32);
            }
        }


        public double Tyre_Grip_Forwards_LF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 1;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9E28));
            }
        }

        
        public double Weight_Rearwheel
        {
            get
            {
                int WeightDistroSteps = rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E122C));
                double WeightDistroBase = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006E1218));
                double WeightDistroStep = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006E1220));
                
                // Weight distro is from rear wheels.
                double WeightDistro = WeightDistroBase + WeightDistroStep*WeightDistroSteps;

                // Fuel?
                double FuelWeight = 0.7197*this.Fuel;
                double CarWeight = this.DryWeight;

                double FuelPosition = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DD04C));
                double WheelBase = 0;

                return 2.1 * (FuelWeight + DryWeight) / 4 * 9.81;
            }set{}
        }

        
        public double DryWeight
        {
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 1000;
                return 75 + rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DCFB4));
            }
            set { }
        }

        
        public double Tyre_Pressure_Optimal_LF { set { } get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006D9F1C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006D9F24));

                return baseval + weightmul*Weight_Rearwheel;
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006D9F5C));
        } }
        
        public double Tyre_Pressure_Optimal_LR
        {
            set { }
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA51C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA524));
                return baseval + weightmul * Weight_Rearwheel;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAB5C));
            }
        }
        
        public double Tyre_Pressure_Optimal_RF
        {
            set { }
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAB1C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAB24));
                return baseval + weightmul * Weight_Rearwheel; rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA55C));
            }
        }
        
        public double Tyre_Pressure_Optimal_RR
        {
            set { }
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB11C));
                double weightmul = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB124));
                return baseval + weightmul * Weight_Rearwheel; return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB15C));
            }
        }


        public double Tyre_Pressure_LF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LF.TyrePressure; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9F5C));
            }
        }

        public double Tyre_Pressure_LR
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LR.TyrePressure; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAB5C));
            }
        }

        public double Tyre_Pressure_RF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RF.TyrePressure; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA55C));
            }
        }

        public double Tyre_Pressure_RR
        {
            set { }
            get
            {
               // if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RR.TyrePressure; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB15C));
            }
        }

        
        public double Tyre_Pressure_LF_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006D9F64)); } }
        
        public double Tyre_Pressure_LR_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAB64)); } }
        
        public double Tyre_Pressure_RF_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA564)); } }
        
        public double Tyre_Pressure_RR_G { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB164)); } }

        
        public double Tyre_Grip_Sidewards_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006D9E18)); } }
        
        public double Tyre_Grip_Sidewards_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAA18)); } }
        
        public double Tyre_Grip_Sidewards_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA418)); } }
        
        public double Tyre_Grip_Sidewards_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB018)); } }


        public double Tyre_Speed_LF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LF.Speed; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9C04));
            }
        }

        public double Tyre_Speed_LR
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LR.Speed; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA804));
            }
        }

        public double Tyre_Speed_RF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RF.Speed; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA204));
            }
        }

        public double Tyre_Speed_RR
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RR.Speed; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAE04));
            }
        }


        public double Tyre_Temperature_LF_Inside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LF.TyreTemp_Inner; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9F44));
            }
        }

        public double Tyre_Temperature_LR_Inside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LR.TyreTemp_Inner; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAB44));
            }
        }

        public double Tyre_Temperature_RF_Inside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RF.TyreTemp_Inner; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA534));
            }
        }

        public double Tyre_Temperature_RR_Inside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RR.TyreTemp_Inner; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB134));
            }
        }



        public double Tyre_Temperature_LF_Middle
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LF.TyreTemp_Middle; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9F3C));
            }
        }

        public double Tyre_Temperature_LR_Middle
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LR.TyreTemp_Middle; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAB3C));
            }
        }

        public double Tyre_Temperature_RF_Middle
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RF.TyreTemp_Middle; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA53C));
            }
        }

        public double Tyre_Temperature_RR_Middle
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RR.TyreTemp_Middle; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB13C));
            }
        }


        public double Tyre_Temperature_LF_Outside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LF.TyreTemp_Outer; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9F34));
            }
        }

        public double Tyre_Temperature_LR_Outside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_LR.TyreTemp_Outer; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAB34));
            }
        }

        public double Tyre_Temperature_RF_Outside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RF.TyreTemp_Outer; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA544));
            }
        }

        public double Tyre_Temperature_RR_Outside
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Wheel_RR.TyreTemp_Outer; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB144));
            }
        }




        public double Tyre_Temperature_LF_Optimal
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 95 + 273; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9EF4));
            }
        }

        public double Tyre_Temperature_LR_Optimal
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 95 + 273; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAAF4));
            }
        }

        public double Tyre_Temperature_RF_Optimal
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 95 + 273; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA4F4));
            }
        }

        public double Tyre_Temperature_RR_Optimal
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 95 + 273; //TODO: Get from car.tyres 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB0F4));
            }
        }


        public double Tyre_Temperature_LF_Fresh
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 75 + 273; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9EEC));
            }
        }

        public double Tyre_Temperature_LR_Fresh
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 75 + 273; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DAAEC));
            }
        }

        public double Tyre_Temperature_RF_Fresh
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 75 + 273; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DA4EC));
            }
        }

        public double Tyre_Temperature_RR_Fresh
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 75 + 273; //TODO: Get from car.tyres
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB0EC));
            }
        }

        // Wheels

        public double Wheel_Radius_LF
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 3;//TODO: Get from car.tyres
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9D74));
            }
        }
        
        public double Wheel_Radius_LR { set { } get { 
                //if (!rFactor.Simulator.UseMemoryReader) return 3;//TODO: Get from car.tyres
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA974)); } }
        
        public double Wheel_Radius_RF { set { } get {
                //if (!rFactor.Simulator.UseMemoryReader) return 3;//TODO: Get from car.tyres
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA374)); } }
        
        public double Wheel_Radius_RR { set { } get {
                //if (!rFactor.Simulator.UseMemoryReader) return 3;//TODO: Get from car.tyres
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAF74)); } }


        // Aerodynamics

        public int Aerodynamics_FrontWing_Setting
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E182C));
            }
        }

        public double Aerodynamics_FrontWing_Downforce
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBA94));
            }
        }


        public int Aerodynamics_RearWing_Setting
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E186C));
            }
        }

        public double Aerodynamics_RearWing_Downforce
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBB88));
            }
        }


        public double Aerodynamics_FrontWing_Drag_Total
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBB64));
            }
        }

        public double Aerodynamics_FrontWing_Drag_Base
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB8D4));
            }
        }


        public double Aerodynamics_RearWing_Drag_Total
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBC58));
            }
        }

        public double Aerodynamics_RearWing_Drag_Base
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB904));
            }
        }


        public double Aerodynamics_LeftFender_Drag
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DD2B4));
            }
        }

        public double Aerodynamics_RightFender_Drag
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DD320));
            }
        }


        public double Aerodynamics_Body_Drag
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DD5C0));
            }
        }


        public double Aerodynamics_Body_DragHeightDiff
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DD5D0));
            }
        }


        public double Aerodynamics_Body_DragHeightAvg
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBC80));
            }
        }


        public double Aerodynamics_Radiator_Drag
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DB934));
            }
        }

        public int Aerodynamics_Radiator_Setting
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E18AC));
            }
        }


        public double Aerodynamics_BrakeDuct_Drag
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DD794));
            }
        }

        public int Aerodynamics_BrakeDuct_Setting
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E18EC));
            }
        }

        // Engine
        
        public double Engine_RPM { set { } get
        {
            //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.EngineRPM;
            return Rotations.Rads_RPM(rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBE80)));
             } }


        public double Engine_RPM_Max_Live { set { } get
        {
            //if (rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.EngineRPM_Max;
                return Rotations.Rads_RPM(rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBF08)));
             } }
        
        public double Engine_RPM_Max_Scale { set { } get {
                //if (!rFactor.Simulator.UseMemoryReader) return 0;
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E1B60)); } }

        public int Engine_RPM_Max_Step
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base + 0x006E1B6C));
            }
        }


        public double Engine_RPM_Idle_Min
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 1000; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBEA0));
            }
        }
        
        public double Engine_RPM_Idle_Max { set { } get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 1500;
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DBEA8));
        } }


        public double Engine_RPM_Launch_Min
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 4000; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC034));
            }
        }

        public double Engine_RPM_Launch_Max
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 5000; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC03C));
            }
        }


        public double Engine_Idle_ThrottleGain
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 1; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBE98));
            }
        }


        public double Engine_Torque_Negative
        {
            set { }
            get
            {
                //if (!rFactor.Simulator.UseMemoryReader) return 0; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DBF28));
            }
        }

        public double Engine_Torque { set { } get
            {
                if (rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006DC224));
            return 0;  } }

        // Driving wheels
        
        public PowerTrainDrivenWheels Powertrain_DrivenWheels { set { } get { return (PowerTrainDrivenWheels)rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006DC10C)); } }
        
        public double Powertrain_DriverDistribution { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DC130)); } }

        // Brake TypicalFailure
        
        public double Brake_TypicalFailure_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DF5F8)); } }
        
        public double Brake_TypicalFailure_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DF730)); } }
        
        public double Brake_TypicalFailure_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DF868)); } }
        
        public double Brake_TypicalFailure_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DF9A0)); } }

        // Brake temperature
        
        public double Brake_Temperature_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA0E0)); } }
        
        public double Brake_Temperature_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DACE0)); } }
        
        public double Brake_Temperature_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA6E0)); } }
        
        public double Brake_Temperature_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB2E0)); } }

        // Brake thickness
        
        public double Brake_Thickness_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA110)); } }
        
        public double Brake_Thickness_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DAD10)); } }
        
        public double Brake_Thickness_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA710)); } }
        
        public double Brake_Thickness_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB310)); } }

        // Brake OptimalTemperature
        
        public double Brake_OptimalTemperature_LF_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA0B8)); } }
        
        public double Brake_OptimalTemperature_LR_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DACB8)); } }
        
        public double Brake_OptimalTemperature_RF_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA6B8)); } }
        
        public double Brake_OptimalTemperature_RR_Low { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB2B8)); } }

        // Brake OptimalTemperature
        
        public double Brake_OptimalTemperature_LF_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA0D0)); } }
        
        public double Brake_OptimalTemperature_LR_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DACD0)); } }
        
        public double Brake_OptimalTemperature_RF_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA6D0)); } }
        
        public double Brake_OptimalTemperature_RR_High { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB2D0)); } }

        // Brake torque
        
        public double Brake_Torque_LF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA0A8)); } }
        
        public double Brake_Torque_LR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DACA8)); } }
        
        public double Brake_Torque_RF { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DA6A8)); } }
        
        public double Brake_Torque_RR { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DB2A8)); } }

        // Clutch torque
        
        public double Clutch_Torque { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DF2A8)); } }
        
        public double Clutch_Friction { set { } get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006D9708)); } }

        // Controls

        public double Pedals_Clutch
        {
            set { }
            get
            {

                //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Pedals_Clutch;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9744));
                
            }
        }
        
        public double Pedals_Throttle { set { } get
        {

            //if (!rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Pedals_Throttle;
            return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D96F0)); 
           } }

        public double Pedals_Brake
        {
            set { }
            get
            {

                //if (rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Pedals_Brake; 
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D973C));
                
            }
        }


        public double SteeringAngle
        {
            set { }
            get
            {
                //if (rFactor.Simulator.UseMemoryReader) return rFactor.MMF.Telemetry.Player.Pedals_Steering;
                return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D972C));
                
            }
        }

        // Driving help
        
        public LevelIndicator DrivingHelp_BrakingHelp
        {
            set { }
            get
            {
                switch (rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D9786)))
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
        
        public LevelIndicator DrivingHelp_SteeringHelp { set { } get { return (LevelIndicator)rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D9785)); } }
        
        public LevelIndicator DrivingHelp_TractionControl { set { } get { return (LevelIndicator)rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D977E)); } }
        
        public bool DrivingHelp_OppositeLock { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D9784)) == 1; } }
        
        public bool DrivingHelp_SpinRecovery { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D9787)) == 1; } }
        
        public bool DrivingHelp_StabilityControl { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D9780)) == 1; } }
        
        public bool DrivingHelp_AutoClutch { set { } get { return rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006D9782)) == 1; } }

        
        public double Speed
        {
            set { }
            get
            {
                return 0;// return rFactor.MMF.Telemetry.Player.Speed; 
            }
        }

        
        public double Brake_InitialThickness_LF
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E0798));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E07A0));
                int steps_LF = rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006E07AC));
                return baseval + stepval * steps_LF;
            }
            set { }
        }

        
        public double Brake_InitialThickness_RF
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E0A98));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E0AA0));
                int steps_RF = rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006E0AAC));
                return baseval + stepval * steps_RF;
            }
            set { }
        }

        
        public double Brake_InitialThickness_LR
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E0D98));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E0DA0));
                int steps_LR = rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006E0DAC));
                return baseval + stepval * steps_LR;
            }
            set { }
        }

        
        public double Brake_InitialThickness_RR
        {
            get
            {
                double baseval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E1098));
                double stepval = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006E10A0));
                int steps_RR = rFactor.Game.ReadByte(new IntPtr(rFactor.Game.Base +0x006E10AC));
                return baseval + stepval * steps_RR;
            }
            set { }
        }

        
        public double SpeedSlipping
        {
            get { return rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base +0x006DBFE8)); }
            set { }
        }
    }
}