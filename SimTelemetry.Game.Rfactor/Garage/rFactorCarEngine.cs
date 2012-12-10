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
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using Triton;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorCarEngine : ICarEngine
    {
        private Dictionary<double, double> EngineTorque_Min;
        private Dictionary<double, double> EngineTorque_Max;
        
        // RAM
        private double ram_torque;
        private double ram_power;
        private double ram_fuel;
        private double ram_wear;

        // Engine modes
        private int modes;
        private double mode_rpm;
        private double mode_torque;
        private double mode_power;
        private double mode_fuel;
        private double mode_wear;

        private IniScanner scanner;

        private string _file;

        private double _maxRpm;
        private double _MaxRpmCurve;
        private int _maxModeIndex;
        private double _maxModeRpm;

        private double _idleRpm;

        private double _inertia;

        private Dictionary<int, string> _engineModes;

        private Dictionary<int, double> _maxRpmMode;

        private double _lifetimeAverage;

        private double _lifetimeStdDeviation;

        private double _lifetimeRpm;

        private double _lifetimeTemperatureOil;

        private double _lifetimeTemperatureWater;

        private double _maxPower;

        public rFactorCarEngine(string file, IniScanner mHdv)
        {
            EngineTorque_Min = new Dictionary<double, double>();
            EngineTorque_Max = new Dictionary<double, double>();

            // Read the engine files.
            scanner = new IniScanner{IniFile = file};
            scanner.FireEventsForKeys = new List<string>();
            scanner.FireEventsForKeys.Add("Main.rpmtorque");
            scanner.HandleCustomKeys += new Signal(HandleEngineLine);
            scanner.Read();

            _file = file;
            string[] rpm_idle = scanner.TryGetData("Main", "IdleRPMLogic");
            string[] rpm_max = scanner.TryGetData("Main", "RevLimitRange");
            _maxRpm = Convert.ToDouble(rpm_max[0]) + Convert.ToDouble(rpm_max[2]) *Convert.ToDouble(rpm_max[1]); // With maximum limits.
            _idleRpm = Convert.ToDouble(rpm_idle[0]) + Convert.ToDouble(rpm_idle[1]);
            _idleRpm /= 2.0;

            _inertia = scanner.TryGetDouble("EngineInertia");

            // TODO: add more data like cooling, radiator, etc.

            // Engine modes.

            string[] mode_range = scanner.TryGetData("Main", "EngineBoostRange");
            string[] mode_effects = scanner.TryGetData("Main", "BoostEffects");

            // Is there any EngineBoost defined?
            if (mode_range.Length == 3)
            {
                modes = (int)Convert.ToDouble(mode_range[2]);
                mode_rpm = Convert.ToDouble(mode_effects[0]);
                mode_fuel = Convert.ToDouble(mode_effects[1]);
                mode_wear = Convert.ToDouble(mode_effects[2]);
                mode_torque = scanner.TryGetDouble("BoostTorque");
                mode_power = scanner.TryGetDouble("BoostPower");
            }
            else modes = 1;

            // RAM
            string[] ram_effects = scanner.TryGetData("Main", "RamEffects");
            if (ram_effects.Length == 4)
            {
                ram_torque = Convert.ToDouble(ram_effects[0]);
                ram_power = Convert.ToDouble(ram_effects[1]);
                ram_fuel = Convert.ToDouble(ram_effects[2]);
                ram_wear = Convert.ToDouble(ram_effects[3]);
            }

            _maxRpmMode = new Dictionary<int, double>();
            _engineModes = new Dictionary<int, string>();
            for(int i = 1; i <= modes; i++)
            {
                _engineModes.Add(i,"Mode " + i);
                _maxRpmMode.Add(i, mode_rpm * i + MaxRPM);
            }

        }

        private void HandleEngineLine(object data)
        {
            object[] d = (object[]) data;
            string key = (string) d[0];
            string[] elements = (string[]) d[1];

            if (elements.Length == 3)
            {
                double rpm = Convert.ToDouble(elements[0]);

                double torq_min = Convert.ToDouble(elements[1]);
                double torq_max = Convert.ToDouble(elements[2]);

                EngineTorque_Min.Add(rpm, torq_min);
                EngineTorque_Max.Add(rpm, torq_max);

                if (_MaxRpmCurve < rpm)
                    _MaxRpmCurve = rpm;

            }
        }

        private double GetTorqueFromCurve(Dictionary<double, double> curve, double rpm)
        {
            double last_rpm = 0;

            foreach (double r in curve.Keys)
            {
                if (last_rpm != r && last_rpm <= rpm && r >= rpm)
                {
                    double v = curve[r];
                    // Closest spot!
                    double last_curve = 0;
                    if (curve.ContainsKey(last_rpm))
                        last_curve = curve[last_rpm];
                    double duty_cycle = (rpm - last_rpm) / (r - last_rpm);
                    double d = last_curve - v; // TODO: Is this the right way around?

                    return v + d * (1 - duty_cycle);
                }

                last_rpm = r;
            }
            return 0;
        }

        public double Lifetime_Temperature_Water
        {
            get { return _lifetimeTemperatureWater; }
        }

        public Dictionary<double, double> GetTorqueCurve(double speed, double throttle, int engine_mode)
        {
            double my_max_rpm = MaxRPM;
            if(MaxRPM_Mode.ContainsKey(engine_mode))
                my_max_rpm = MaxRPM_Mode[engine_mode];
            my_max_rpm = Math.Max(my_max_rpm, MaxRPMCurve);

            double torque_factor = 1+speed*ram_torque + engine_mode *mode_torque;

            Dictionary<double, double> TorqueCurve = new Dictionary<double, double>();

            for (int rpm = 0; rpm <= my_max_rpm; rpm+=100)
            {
                double rpm_dutycycle = rpm / my_max_rpm;
                double power_factor = 1 + rpm_dutycycle * (speed * ram_power + engine_mode * mode_power);

                double t_max = GetTorqueFromCurve(EngineTorque_Max, rpm);
                double t_min = GetTorqueFromCurve(EngineTorque_Min, rpm);
                double torque = t_min + throttle * (t_max - t_min);
                torque *= torque_factor;
                torque *= power_factor;
                // TODO: Mode and RAM effects

                TorqueCurve.Add(rpm, torque);
            }
            return TorqueCurve;
        }
        
        public Dictionary<double, double> GetPowerCurve(double speed, double throttle, int engine_mode)
        {
            double my_max_rpm = MaxRPM;
            if (MaxRPM_Mode.ContainsKey(engine_mode))
                my_max_rpm = MaxRPM_Mode[engine_mode];
            my_max_rpm = Math.Max(my_max_rpm, MaxRPMCurve);

            double torque_factor = 1 + speed * ram_torque + engine_mode * mode_torque;

            Dictionary<double, double> PowerCurve = new Dictionary<double, double>();

            for (int rpm = 0; rpm <= my_max_rpm; rpm += 100)
            {
                double rpm_dutycycle = rpm / my_max_rpm;
                double power_factor = 1 + rpm_dutycycle*(speed * ram_power + engine_mode * mode_power);

                double t_max = GetTorqueFromCurve(EngineTorque_Max, rpm);
                double t_min = GetTorqueFromCurve(EngineTorque_Min, rpm);
                double torque = t_min + throttle * (t_max - t_min);
                torque *= torque_factor;
                double power = rpm*torque*Math.PI*2/60000.0; // kW
                power *= power_factor;
                power /= 0.745699872;
                // TODO: Mode and RAM effects

                PowerCurve.Add(rpm, power);
            }
            return PowerCurve;

        }

        public double GetMaximumPower()
        {
            if (_maxPower == 0)
                _maxPower = GetPowerCurve(0, 1, 0).Max(x => x.Value);

            return _maxPower;
        }

        public string File
        {
            get { return _file; }
        }

        public double MaxRPM
        {
            get { return _maxRpm; }
        }

        public double MaxRPMCurve
        {
            get { return _MaxRpmCurve; }
        }

        public double IdleRPM
        {
            get { return _idleRpm; }
        }

        public double Inertia
        {
            get { return _inertia; }
        }

        public Dictionary<int, string> EngineModes
        {
            get { return _engineModes; }
        }

        public Dictionary<int, double> MaxRPM_Mode
        {
            get { return _maxRpmMode; }
        }

        public double Lifetime_Average
        {
            get { return _lifetimeAverage; }
        }

        public double Lifetime_StdDeviation
        {
            get { return _lifetimeStdDeviation; }
        }

        public double Lifetime_RPM
        {
            get { return _lifetimeRpm; }
        }

        public double Lifetime_Temperature_Oil
        {
            get { return _lifetimeTemperatureOil; }
        }
    }
}