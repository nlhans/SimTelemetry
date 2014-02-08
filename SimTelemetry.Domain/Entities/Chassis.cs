using System;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Entities
{
    public class Chassis
    {
        public float Weight { get; private set; }

        public float FuelTankSize { get; private set; }

        private float DragBody { get; set; }
        private IMathFormula DragFrontWing { get; set; }
        private IMathFormula DragRearWing { get; set; }
        private IMathFormula DragRadiator { get; set; }
        private IMathFormula DragBrakeDucts { get; set; }
        private IMathFormula DragRideHeight { get; set; }

        public IMathFormula RideheightFront { get; private set; }
        public IMathFormula RideheightRear { get; private set; }

        public IMathFormula WeightDistForAft { get; private set; }

        public Chassis(float weight, float fuelTankSize, float dragBody, IMathFormula dragFrontWing, IMathFormula dragRearWing, IMathFormula dragRadiator, IMathFormula dragBrakeDucts, IMathFormula dragRideHeight, IMathFormula rideheightFront, IMathFormula rideheightRear, IMathFormula weightDist)
        {
            Weight = weight;
            FuelTankSize = fuelTankSize;
            DragBody = dragBody;
            DragFrontWing = dragFrontWing;
            DragRearWing = dragRearWing;
            DragRadiator = dragRadiator;
            DragBrakeDucts = dragBrakeDucts;
            DragRideHeight = dragRideHeight;
            RideheightFront = rideheightFront;
            RideheightRear = rideheightRear;

            WeightDistForAft = weightDist;
        }

        public double GetAeroDrag(int frontwing, int rearwing, int radiator, int brakes, int rideheight_front, int rideheight_rear)
        {
            var fw = DragFrontWing!=null ? DragFrontWing.Get(frontwing) : 0;
            var rw = DragRearWing != null ?  DragRearWing.Get(rearwing) : 0;
            var rad = DragRearWing!=null ?  DragRadiator.Get(radiator) : 0;
            var brk = DragBrakeDucts!=null? DragBrakeDucts.Get(brakes) : 0;

            var rideHeightFInFloat = RideheightFront.Get(rideheight_front);
            var rideHeightRInFloat = RideheightRear.Get(rideheight_rear);

            var avgRideHeight = (rideHeightFInFloat + rideHeightRInFloat)/2.0f;
            var diffRideHeight = Math.Abs(rideHeightFInFloat - rideHeightRInFloat);

            var hgt = DragRideHeight.Get(avgRideHeight, diffRideHeight);

            // total drag is the sum of all.
            var drag = fw + rw + rad + brk + hgt + DragBody;
            
            return drag;
        }

        public double GetWeightFront(float mass, int weightDistSteps)
        {
            var dist = WeightDistForAft.Get(weightDistSteps);

            return dist * mass;
        }

        public double GetWeightRear(float mass, int weightDistSteps)
        {
            var dist = WeightDistForAft.Get(weightDistSteps);

            return (1-dist) * mass;
        }
    }
}