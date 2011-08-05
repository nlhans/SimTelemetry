using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Channels;
using SimTelemetry.Data;

namespace SimTelemetry
{
    public class DataChannels
    {
        public static double Parse(string key, DataSample sample)
        {
            switch(key)
            {
                case "RPM":
                    return Rotations.Rads_RPM((double)sample.Player.Engine_RPM);
                    break;

                case "Speed":
                    return Speed.Ms_Kmh(sample.Player.Speed);
                    break;

                case "Steering":
                    return sample.Player.SteeringAngle;
                    break;

                case "Throttle":
                    return sample.Player.Pedals_Throttle;
                    break;

                case "Brake":
                    return sample.Player.Pedals_Brake;
                    break;
            }

            return 0;

        }
    }
}
