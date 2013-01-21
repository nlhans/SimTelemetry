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
using System.Collections.Generic;
using SimTelemetry.Data;
using SimTelemetry.Objects;

namespace SimTelemetry.Peripherals.Dashboard
{
    public class ShiftRpm
    {
        public double GetTorque(double rpm, double throttle)
        {
            //double t= (-1*rpm*rpm/9000.0/9000*1820 + throttle*(-9.5*Math.Pow(rpm - 3000.0, 2)/2000.0 + 50.0*rpm + 5000.0)/200.0);
            return EngineCurve.GetTorque(rpm, throttle, 1);
            //return t;
        }

        public double TorqueInt(double low, double high, double throttle)
        {
            double i = 0;
            double step = 25;
            // Stepsize = 25rpm

            for (double rpm = low; rpm < high; rpm += step)
                i += GetTorque(rpm, throttle) * step;
            return i;
        }

        public static double GetRatio(int g)
        {
            switch (g)
            {
                case 1:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio1;
                    break;
                case 2:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio2;
                    break;
                case 3:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio3;
                    break;
                case 4:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio4;
                    break;
                case 5:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio5;
                    break;
                case 6:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio6;
                    break;
                case 7:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio7;
                    break;
                case 8:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio8;
                    break;
                case 9:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio9;
                    break;
                case 10:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio10;
                    break;
                case 11:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio11;
                    break;
                case 12:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio12;
                    break;
                case 13:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio13;
                    break;
                case 14:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio14;
                    break;
                case 15:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio15;
                    break;
                case 16:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio16;
                    break;
                case 17:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio17;
                    break;
                case 18:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio18;
                    break;
                default:
                    return 1;
                    break;
            }
        }

        public double GetRatioBetween(int gear)
        {

            //int gear = Telemetry.m.Sim.Player.Gear;
            double Ratio1 = ShiftRpm.GetRatio(gear);
            double Ratio2 = ShiftRpm.GetRatio(gear + 1);
            return Ratio2 / Ratio1;
        }

        public double Get(double throttle, double ratio)
        {
            double MaxRPM = Telemetry.m.Sim.Player.Engine_RPM_Max_Live;
            double PF = double.MinValue;
            double PF2 = double.MinValue;
            double low = MaxRPM * (0.1 + 0.9 * throttle) * 0.5;
            double Step = 25;
            if (GetTorque(Telemetry.m.Sim.Player.Engine_RPM, throttle) < 0)
                return Telemetry.m.Sim.Player.Engine_RPM_Max_Live;
            // Search the point
            Dictionary<double, double> ShiftPoints = new Dictionary<double, double>();
            for (double rpm = low; rpm < MaxRPM; rpm += Step)
            {
                // Calculate.
                double F = TorqueInt(low, rpm, throttle) + TorqueInt(rpm * ratio, MaxRPM, throttle);
                if (PF2 != double.MinValue)
                {
                    //Console.WriteLine(F);
                    if (PF2 - PF > 0)
                    {
                        ShiftPoints.Add(rpm - 2 * Step, PF);
                    }
                }
                PF2 = PF;
                PF = F;

            }
            if (ShiftPoints.Count > 0)
            {
                double bestF = 0;
                double related_rpm = 0;
                foreach (KeyValuePair<double, double> kvp in ShiftPoints)
                {
                    if (bestF < kvp.Value)
                    {
                        bestF = kvp.Value;
                        related_rpm = kvp.Key;
                    }
                }
                return related_rpm;
            }
            return MaxRPM;

        }
    }
}