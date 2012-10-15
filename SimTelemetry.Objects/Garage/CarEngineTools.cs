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
            Car = c;
            Car.ScanEngine();
        }

        public void Scan()
        {
            // Maximum power/torque figures
            Dictionary<double, double> torque = Car.Engine.GetTorqueCurve(0, 1, 0);
            Dictionary<double, double> power = Car.Engine.GetPowerCurve(0, 1, 0);

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
