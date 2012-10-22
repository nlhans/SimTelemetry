using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects.Garage
{
    public class CarEngineTools
    {
        public ICar Car { get; set; }

        public double MaxPower_HP { get; set; }
        public double MaxPower_RPM { get; set; }
        public double MaxTorque_NM { get; set; }
        public double MaxTorque_RPM { get; set; }


        public CarEngineTools(ICar c)
        {
            if (c != null)
            {
                Car = c;
                Car.ScanEngine();
            }
        }

        public void Scan()
        {
            Scan(0, 1, 0);
        }

        public void Scan(double speed, double throttle, int engine_mode)
        {
            if (Car != null && Car.Engine != null)
            {
                // Maximum power/torque figures
                Dictionary<double, double> torque = Car.Engine.GetTorqueCurve(speed, throttle, engine_mode);
                Dictionary<double, double> power = Car.Engine.GetPowerCurve(speed, throttle, engine_mode);

                MaxPower_HP = 0;
                MaxTorque_NM = 0;

                foreach (KeyValuePair<double, double> torque_kvp in torque)
                {
                    if (torque_kvp.Value > MaxTorque_NM)
                    {
                        MaxTorque_NM = torque_kvp.Value;
                        MaxTorque_RPM = torque_kvp.Key;
                    }
                }

                foreach (KeyValuePair<double, double> power_kvp in power)
                {
                    if (power_kvp.Value > MaxPower_HP)
                    {
                        MaxPower_HP = power_kvp.Value;
                        MaxPower_RPM = power_kvp.Key;
                    }
                }
            }
        }
    }
}
