using System;
using System.Collections.Generic;
using SimTelemetry.Data;
using SimTelemetry.Objects;

namespace SimTelemetry.Peripherals.Dashboard
{
    public class EngineCurve
    {
        public static List<double> DoubleList = new List<double>();

        public static double GetTorque(double RPM, double throttle, double boost)
        {
            int BaseEngineCurve = 0x00ADD94C; // TODO: MAP TO SIMULATOR MAPPING

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
            bool Cached = ((DoubleList.Count == 0) ? false : true);
            if (!Cached)
            {
                for (offset = 0; offset < 500; offset++)
                {

                    double curve_rpm, Tl_Now, Th_Now;
                    // read rpm
                    // TOOD: RFACTOR ONLY
                    // TODO: these operations need to be moved to game DLL's!!!
                    curve_rpm = Telemetry.m.Sim.Memory.ReadDouble(new IntPtr(BaseEngineCurve + 0x8 * 3 * offset));
                    Tl_Now = Telemetry.m.Sim.Memory.ReadDouble(new IntPtr(BaseEngineCurve + 0x8 * 1 + 0x8 * 3 * offset));
                    Th_Now = Telemetry.m.Sim.Memory.ReadDouble(new IntPtr(BaseEngineCurve + 0x8 * 2 + 0x8 * 3 * offset));

                    DoubleList.Add(curve_rpm);
                    DoubleList.Add(Tl_Now);
                    DoubleList.Add(Th_Now);

                }
                Cached = true;
                offset = 0;

            }
            while (Th_H == 0)
            {

                double curve_rpm, Tl_Now, Th_Now;
                curve_rpm = DoubleList[offset * 3];
                Tl_Now = DoubleList[offset * 3 + 1];
                Th_Now = DoubleList[offset * 3 + 2];


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
            double TorqueH = Th_L + RPM_Part * (Th_H - Th_L);
            double TorqueL = Tl_L + RPM_Part * (Tl_H - Tl_L);
            //return TorqueH * throttle * boost + TorqueL * boost;
            return (TorqueH - TorqueL) * throttle * boost + TorqueL;
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