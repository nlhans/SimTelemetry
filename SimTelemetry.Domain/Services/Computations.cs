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
            if (drv.AeroSetup == null)
                return 360;

            double maxPower = c.Engine.MaximumPower;
            maxPower = 125;

            // Get setup
            var frontWing = drv.AeroSetup.FrontWing;
            var rearWing = drv.AeroSetup.RearWing;
            var radiator = drv.AeroSetup.Radiator;
            var brakes = drv.AeroSetup.BrakeDuct;
            var rideheightFront = drv.AeroSetup.RideHeightFront;
            var rideheightRear = drv.AeroSetup.RideHeightRear;

            var drag = c.Chassis.GetAeroDrag(frontWing, rearWing, radiator, brakes, rideheightFront, rideheightRear);

            var tyreDrag = c.Wheels.FirstOrDefault().RollResistance + c.Wheels.LastOrDefault().RollResistance;
            tyreDrag /= 2;

            tyreDrag *= 0.0125f; // contact patch(average)
            drag += tyreDrag;
            return 3.6*Math.Pow(maxPower*1000.0/drag, 1/3.0);
        }
    }
}
