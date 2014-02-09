using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Services
{
    public static class Computations
    {
        public static double GetTopSpeed(TelemetryDriver drv, Car c)
        {
            if (drv.Setup == null || c.Engine.MaximumPower <= 0)
                return 360;

            double maxPower = c.Engine.MaximumPower;

            // Get setup
            var frontWing = drv.Setup.FrontWing;
            var rearWing = drv.Setup.RearWing;
            var radiator = drv.Setup.Radiator;
            var brakes = drv.Setup.BrakeDuct;
            var rideheightFront = drv.Setup.RideHeightFront;
            var rideheightRear = drv.Setup.RideHeightRear;

            var drag = c.Chassis.GetAeroDrag(frontWing, rearWing, radiator, brakes, rideheightFront, rideheightRear);

            // TODO: Tyre resistance -> drag

            return 3.6*Math.Pow(maxPower*1000.0/drag, 1/3.0);
        }
    }
}
