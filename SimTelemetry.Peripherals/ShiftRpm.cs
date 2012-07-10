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
            double step = 75 / 4;
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
                    if (Telemetry.m.Sim.Drivers.Player.Gears == 5)
                        return Telemetry.m.Sim.Drivers.Player.GearRatio5 * 0.9;

                    return Telemetry.m.Sim.Drivers.Player.GearRatio6;
                    break;
                case 7:
                    if (Telemetry.m.Sim.Drivers.Player.Gears == 5)
                        return Telemetry.m.Sim.Drivers.Player.GearRatio5 * 0.9 * 0.9;
                    if (Telemetry.m.Sim.Drivers.Player.Gears == 6)
                        return Telemetry.m.Sim.Drivers.Player.GearRatio6 * 0.9;
                    return Telemetry.m.Sim.Drivers.Player.GearRatio7;
                    break;
                case 8:
                    if (Telemetry.m.Sim.Drivers.Player.Gears == 5)
                        return Telemetry.m.Sim.Drivers.Player.GearRatio5 * 0.9 * 0.9 * 0.9;
                    else if (Telemetry.m.Sim.Drivers.Player.Gears == 6)
                        return Telemetry.m.Sim.Drivers.Player.GearRatio6 * 0.9 * 0.9;
                    else if (Telemetry.m.Sim.Drivers.Player.Gears == 7)
                        return Telemetry.m.Sim.Drivers.Player.GearRatio7 * 0.9;
                    else
                    {
                        return Telemetry.m.Sim.Drivers.Player.GearRatio7;
                    }
                    break;
                case 9:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio6 * 0.9 * 0.9 * 0.9;
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
            double MaxRPM = Rotations.Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Max_Live);
            double PF = double.MinValue;
            double PF2 = double.MinValue;
            double low = MaxRPM * (0.1 + 0.9 * throttle) * 0.6;
            double Step = 75;
            if (GetTorque(Telemetry.m.Sim.Player.Engine_RPM, throttle) < 0)
                return Rotations.Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Max_Live);
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