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
using System.Diagnostics;
using SimTelemetry.Data;
using SimTelemetry.Objects;
using Triton.Memory;

namespace SimTelemetry.Peripherals.Dashboard
{
    public class EngineCurve
    {
        public static List<double> DoubleList = new List<double>();

        private static MemoryReader rf = new MemoryReader();

        public static double GetTorque(double RPM, double throttle, double boost)
        {

            int BaseEngineCurve = 0x00ADD94C; // TODO: MAP TO SIMULATOR MAPPING

            // get engine torque figures (at this engine RPM)
            // this is directly from memory based engine curves.
            double Rads = (RPM);
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
            bool Cached = ((DoubleList.Count <= 25) ? false : true);
            if (!Cached)
            {
                lock (rf)
                {
                    if (DoubleList.Count > 25) return 0;
                    bool rfOpen = false;
                    try
                    {
                        rf.ReadProcess = Process.GetProcessesByName(Telemetry.m.Sim.ProcessName)[0];
                        rf.OpenProcess();
                        rfOpen = true;
                        DoubleList.Clear();
                        for (offset = 0; offset < 500; offset++)
                        {

                            double curve_rpm = 0, Tl_Now = 0, Th_Now = 0;
                            // read rpm
                            // TOOD: RFACTOR ONLY
                            // TODO: these operations need to be moved to game DLL's!!!
                            curve_rpm = Rotations.Rads_RPM(rf.ReadDouble(new IntPtr(BaseEngineCurve + 0x8*3*offset)));
                            Tl_Now = rf.ReadDouble(new IntPtr(BaseEngineCurve + 0x8*1 + 0x8*3*offset));
                            Th_Now = rf.ReadDouble(new IntPtr(BaseEngineCurve + 0x8*2 + 0x8*3*offset));

                            DoubleList.Add(curve_rpm);
                            DoubleList.Add(Tl_Now);
                            DoubleList.Add(Th_Now);

                        }
                        Cached = true;
                        offset = 0;
                    }
                    catch (Exception ex)
                    {
                        Cached = false;
                        DoubleList.Clear();
                    }
                    if (rfOpen)
                        rf.CloseHandle();
                }
            }

            while (Th_H == 0)
            {

                double curve_rpm, Tl_Now, Th_Now;
                curve_rpm = DoubleList[offset*3];
                Tl_Now = DoubleList[offset*3 + 1];
                Th_Now = DoubleList[offset*3 + 2];


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
            if (Th_H == 0)
            {
                
            }
            // calculate duty cycle and determine torque.
            double RPM_Part = (Rads - R_L)/(R_H - R_L); // factor in rpm curve.
            double TorqueH = Th_L + RPM_Part*(Th_H - Th_L);
            double TorqueL = Tl_L + RPM_Part*(Tl_H - Tl_L);
            //return TorqueH * throttle * boost + TorqueL * boost;
            return (TorqueH - TorqueL)*throttle*boost + TorqueL;
        }

        public static double GetMaxHP_RPM()
        {
            double max_power = 0;
            double max_rpm = 0;
            double Engine_Max = Rotations.Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Max_Live);
            for (double rpm = 0; rpm < Engine_Max; rpm += 100)
            {
                double power = GetTorque(rpm, 1, 1) * rpm;
                if (power > max_power)
                {
                    max_power = power;
                    max_rpm = rpm;
                }

            }
            return max_rpm;
        }

        public static double GetTorque()
        {
            return GetTorque(Telemetry.m.Sim.Player.Engine_RPM, 1, 1);

        }

        public static double GetMaxHP()
        {
            double max_power = 0;
            double max_rpm = 0;
            double Engine_Max = Rotations.Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Max_Live);
            for (double rpm = 0; rpm < Engine_Max; rpm += 100)
            {
                double power = GetTorque(rpm, 1, 1) * rpm;
                if (power > max_power)
                {
                    max_power = power / 5252;
                    max_rpm = rpm;
                }

            }
            return max_power;
        }
    }
}