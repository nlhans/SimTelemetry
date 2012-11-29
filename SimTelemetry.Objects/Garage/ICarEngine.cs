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

namespace SimTelemetry.Objects.Garage
{
    public interface ICarEngine
    {
        string File { get; }
        
        /// <summary>
        /// Maximum Engine speed (in RPM!)
        /// Value at stock settings/mode.
        /// </summary>
        double MaxRPM { get; }

        /// <summary>
        /// Maximum engine speed from the engine curve.
        /// </summary>
        double MaxRPMCurve { get; }

        /// <summary>
        /// Idle Engine speed (RPM)
        /// </summary> 
        double IdleRPM { get; }

        /// <summary>
        /// Engine inertia 
        /// </summary>
        double Inertia { get; }

        /// <summary>
        /// Different available engine modes.
        /// Can be given as a int<>string combination in case the game supports entitled engine modes.
        /// The integer value is the one which will be parsed to GetTorqueCurve calls.
        /// 0 must be STOCK engine mode
        /// </summary>
        Dictionary<int, string> EngineModes { get; }

        /// <summary>
        /// Maximum RPM dependant on engine mode.
        /// </summary>
        Dictionary<int, double> MaxRPM_Mode { get; }

        double Lifetime_Average { get; }
        double Lifetime_StdDeviation { get; }
        double Lifetime_RPM { get; }
        double Lifetime_Temperature_Oil { get; }
        double Lifetime_Temperature_Water { get; }

        /// <summary>
        /// Torque curve on Engine RPM (key) vs Torque (Nm).
        /// Optional variables of influence can be speed (in some sims), engine mode and throttle (ofcourse).
        /// The curve should be accurate to steps of 100 rpm from 0 to Maximum RPM. 
        /// </summary>
        /// <param name="speed">Speed in m/s</param>
        /// <param name="throttle">Throttle 0.000-1.000 (0-100%)</param>
        /// <param name="engine_mode">Engine mode.</param>
        /// <returns>RPM vs torque map</returns>
        Dictionary<double, double> GetTorqueCurve(double speed, double throttle, int engine_mode);

        /// <summary>
        /// Torque curve on Engine RPM (key) vs Power (hp).
        /// Optional variables of influence can be speed (in some sims), engine mode and throttle (ofcourse).
        /// The curve should be accurate to steps of 100 rpm from 0 to Maximum RPM. 
        /// </summary>
        /// <param name="speed">Speed in m/s</param>
        /// <param name="throttle">Throttle 0.000-1.000 (0-100%)</param>
        /// <param name="engine_mode">Engine mode.</param>
        /// <returns>RPM vs power map</returns>
        Dictionary<double, double> GetPowerCurve(double speed, double throttle, int engine_mode);
    }
}
