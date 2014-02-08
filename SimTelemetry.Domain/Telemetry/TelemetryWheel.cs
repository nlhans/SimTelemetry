using System;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryWheel : ITelemetryObject
    {
        public WheelLocation Location { get; private set; }

        public float BrakeThickness { get; private set; }
        public float BrakeTemperature { get; private set; }

        public float TyrePressure { get; private set; }
        public float TyreWear { get; private set; }
        public float TyreContactPatch { get; private set; }

        public float TemperatureMiddle { get; private set; }
        public float TemperatureInside { get; private set; }
        public float TemperatureOutside { get; private set; }

        public double LoadForce { get; private set; }

        public float Speed { get; private set; }
        public float Rideheight { get; private set; }

        private IDataNode Pool { get; set; }

        public TelemetryWheel(WheelLocation loc, IDataNode p)
        {
            Pool = p;

            Location = loc;
            
            BrakeThickness = -1;
            BrakeTemperature = -1;

            TyrePressure = -1;
            TyreWear = 0;
            TyreContactPatch = 0;
            LoadForce = 0;

            TemperatureInside = -1;
            TemperatureMiddle = -1;
            TemperatureOutside = -1;

            Speed = -1;
            Rideheight = -1;
        }

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
            var locationAsString = "";
            switch(Location)
            {
                case WheelLocation.FRONTLEFT:
                    locationAsString = "LF";
                    break;
                case WheelLocation.FRONTRIGHT:
                    locationAsString = "RF";
                    break;
                case WheelLocation.REARLEFT:
                    locationAsString = "LR";
                    break;
                case WheelLocation.REARRIGHT:
                    locationAsString = "RR";
                    break;
            }
            
            BrakeTemperature = Pool.ReadAs<float>("BrakeTemperature" + locationAsString);
            BrakeThickness = Pool.ReadAs<float>("BrakeThickness" + locationAsString);

            LoadForce = Pool.ReadAs<float>("WheelLoad" + locationAsString);

            TyreWear = Pool.ReadAs<float>("TyreWear" + locationAsString);
            TyrePressure = Pool.ReadAs<float>("TyrePressure" + locationAsString);
            TyreContactPatch = Pool.ReadAs<float>("TyreContactPatch" + locationAsString);
            
            TemperatureInside = Pool.ReadAs<float>("TyreTemperatureInside" + locationAsString);
            TemperatureMiddle = Pool.ReadAs<float>("TyreTemperatureMiddle" + locationAsString);
            TemperatureOutside = Pool.ReadAs<float>("TyreTemperatureOutside" + locationAsString);
            
            Speed = Pool.ReadAs<float>("TyreSpeed" + locationAsString);
            Rideheight = Pool.ReadAs<float>("Rideheight" + locationAsString);
        }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {

        }
    }
}