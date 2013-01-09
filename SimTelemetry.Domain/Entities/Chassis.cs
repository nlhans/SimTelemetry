using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Entities
{
    public class Chassis
    {
        public float Weight { get; private set; }

        public float FuelTankSize { get; private set; }

        private float DragBody { get; set; }
        private Polynomial DragFrontWing { get; set; }
        private Polynomial DragRearWing { get; set; }
        private Polynomial DragRadiator { get; set; }
        private Polynomial DragBrakeDucts { get; set; }

        public float RideheightFront { get; set; }
        // Rideheight?

        public Chassis(float weight, float fuelTankSize, float dragBody, Polynomial dragFrontWing, Polynomial dragRearWing, Polynomial dragRadiator, Polynomial dragBrakeDucts, float rideheightFront)
        {
            Weight = weight;
            FuelTankSize = fuelTankSize;
            DragBody = dragBody;
            DragFrontWing = dragFrontWing;
            DragRearWing = dragRearWing;
            DragRadiator = dragRadiator;
            DragBrakeDucts = dragBrakeDucts;
            RideheightFront = rideheightFront;
        }

        public float GetAeroDrag(int frontwing, int rearwing, int radiator, int brakes, int rideheight_front, int rideheight_rear)
        {
            return 1.0f;
        }

    }
}