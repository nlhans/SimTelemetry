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
            if (drv.Setup == null)
                return 360;

            double maxPower = c.Engine.MaximumPower;
            maxPower = 125;

            // Get setup
            var frontWing = drv.Setup.FrontWing;
            var rearWing = drv.Setup.RearWing;
            var radiator = drv.Setup.Radiator;
            var brakes = drv.Setup.BrakeDuct;
            var rideheightFront = drv.Setup.RideHeightFront;
            var rideheightRear = drv.Setup.RideHeightRear;

            var drag = c.Chassis.GetAeroDrag(frontWing, rearWing, radiator, brakes, rideheightFront, rideheightRear);

            var tyreDrag = c.Wheels.FirstOrDefault().RollResistance + c.Wheels.LastOrDefault().RollResistance;
            tyreDrag /= 2;

            tyreDrag *= 0.0125f; // contact patch(average)
            drag += tyreDrag;
            return 3.6*Math.Pow(maxPower*1000.0/drag, 1/3.0);
        }
    }
}
