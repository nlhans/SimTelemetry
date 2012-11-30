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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor
{
    // TODO: The functions in this class are not scalable.

    public class Computations
    {
        public static double GetAeroDrag()
        {
            // Frontwing
            double Drag_FW = rFactor.Player.Aerodynamics_FrontWing_Drag_Total;

            // L/R Fenders
            double Drag_Fenders = rFactor.Player.Aerodynamics_LeftFender_Drag +
                                  rFactor.Player.Aerodynamics_RightFender_Drag;

            // Rear wing
            double Drag_RW = rFactor.Player.Aerodynamics_RearWing_Drag_Total;

            // Body
            double Drag_Body = rFactor.Player.Aerodynamics_Body_Drag;

            double Body_Height_Front = 0.5 *
                                       (rFactor.Player.Suspension_RideHeight_LF_G +
                                        rFactor.Player.Suspension_RideHeight_RF_G);
            double Body_Height_Rear = 0.5 *
                                       (rFactor.Player.Suspension_RideHeight_LR_G +
                                        rFactor.Player.Suspension_RideHeight_RR_G);
            
            double Body_Height_Diff = Body_Height_Front + Body_Height_Rear;
            double Drag_BodyHeight = (Body_Height_Front+Body_Height_Rear)*0.5 * rFactor.Player.Aerodynamics_Body_DragHeightAvg + Body_Height_Diff* rFactor.Player.Aerodynamics_Body_DragHeightDiff;

            // Radiator
            double Drag_Radiator = rFactor.Player.Aerodynamics_Radiator_Drag*
                                   rFactor.Player.Aerodynamics_Radiator_Setting;

            // Brake ducts

            double Drag_Brakes = rFactor.Player.Aerodynamics_BrakeDuct_Drag *
                                   rFactor.Player.Aerodynamics_BrakeDuct_Setting;
            // general formula: BodyDragBase + BrakeDuctSetting*BrakeDuctDrag + RadiatorSetting*RadiatorDrag + BodyDragHeightAvg*ARH + BodyDragHeightDiff*Rake
            // http://isiforums.net/f/showthread.php/287-Differences-in-aero-calculations-in-CarFactory-vs-rFactor-telemetry
            // http://koti.mbnet.fi/tspartan/gp1975/airoopas/index.php?id=functions.php
            return Drag_FW + Drag_BodyHeight + Drag_Body + Drag_RW + Drag_Fenders + Drag_Radiator + Drag_Brakes;

        }

        public static double GetAirDensity()
        {
            return 1.22;
        }


        public static double Get_Engine_CurrentHP()
        {
            double rpm = Rotations.Rads_RPM(rFactor.Player.Engine_RPM);
            double throttle = rFactor.Player.Pedals_Throttle;
            double Torq_Min = rFactor.Game.ReadDouble(new IntPtr(0x00ADBF28));
            double Torq_Max = rFactor.Game.ReadDouble(new IntPtr(0x00ADBF30));
            double torque = Torq_Max * throttle + Torq_Min;
            torque = rFactor.Game.ReadDouble(new IntPtr(0x00ADC224)); // This seems to be the *real* torque figure.
            double HP = Power.HP_KW(rpm*torque/5252);
            return HP;

        }

        private static double Get_Engine_Hp(double rpm)
        {
            return Get_Engine_Torque(rpm, 1, 1)*rpm/5252;
        }

        public static double Get_Engine_MaxHP()
        {
            double rpm = 0;
            double rpm_max = Rotations.Rads_RPM(rFactor.Player.Engine_RPM_Max_Live);
            double power = 0;
            for (rpm = 0; rpm < rpm_max; rpm+= 50)
            {
                double tmp = Get_Engine_Hp(rpm);
                if(double.IsNaN(tmp) == false && double.IsInfinity(tmp) == false)
                power = Math.Max(power, tmp);
            }
            return power;

        }

        public static double Get_Engine_Torque()
        {
            return Get_Engine_Torque(rFactor.Player.Engine_RPM, 1, 1);

        }

        public static double Get_Engine_Torque(double RPM, double throttle, double boost)
        {
            int BaseEngineCurve = 0x00ADD94C; // TODO: Map to simulator plug-in, add local torque curve read-outs.

            // get engine torque figures (at this engine RPM)
            // this is directly from memory based engine curves.
            double Rads = Rotations.RPM_Rads(RPM);
            double Th_L = 0;
            double Th_H = 0;
            double Tl_L = 0;
            double Tl_H = 0;
            double R_L = 0;
            double R_H = 0;
            int offset = 0;

            double Th_Prev = 0;
            double Tl_Prev = 0;
            double R_Prev = 0;
            while (Th_H == 0)
            {
                // read rpm
                double curve_rpm = rFactor.Game.ReadDouble(new IntPtr(BaseEngineCurve + 0x8 * 3 * offset));
                double Tl_Now = rFactor.Game.ReadDouble(new IntPtr(BaseEngineCurve + 0x8 * 1 + 0x8 * 3 * offset));
                double Th_Now = rFactor.Game.ReadDouble(new IntPtr(BaseEngineCurve + 0x8 * 2 + 0x8 * 3 * offset));

                if (curve_rpm >= Rads)
                {
                    Th_L = Th_Prev;
                    Th_H = Th_Now;

                    Tl_L = Tl_Prev;
                    Tl_H = Tl_Now;


                    R_H = curve_rpm;
                    R_L = R_Prev;
                    break;
                }

                R_Prev = curve_rpm;
                Th_Prev = Th_Now;
                Tl_Prev = Tl_Now;
                offset++;

            }

            // calculate duty cycle and determine torque.
            double RPM_Part = (Rads - R_L) / (R_H - R_L); // factor in rpm curve.
            if (double.IsNaN(RPM_Part)) RPM_Part = 0;
            double TorqueH = Th_L + RPM_Part * (Th_H - Th_L);
            double TorqueL = Tl_L + RPM_Part * (Tl_H - Tl_L);
            //return TorqueH * throttle * boost + TorqueL * boost;
            return (TorqueH - TorqueL) * throttle * boost + TorqueL;
        }

        public static double GetTheoraticalTopSpeed()
        {
            return 0;
        }

    }
}
