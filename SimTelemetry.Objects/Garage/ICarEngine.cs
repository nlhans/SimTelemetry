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
