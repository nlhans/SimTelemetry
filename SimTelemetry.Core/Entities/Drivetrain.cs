using System.Collections.Generic;
using SimTelemetry.Core.Enumerations;

namespace SimTelemetry.Core.Entities
{
    public class Drivetrain
    {
        public float ClutchSlipTorque { get; private set; }
        public float ClutchFriction { get; private set; }

        public float UpshiftTime { get; private set; }
        public float DownshiftTime { get; private set; }

        public int Gears { get; private set; }
        public IEnumerable<float> GearRatios { get; private set; }
        public IEnumerable<float> FinalRatios { get; private set; }

        public IEnumerable<int> StockRatios { get; private set; }
        public int StockFinal { get; private set; }

        public DriveTrainSetup Drive { get; private set; }
        public float DriveDistribution { get; private set; }

        // Differential information?
        public Drivetrain(float clutchSlipTorque, float clutchFriction, float upshiftTime, float downshiftTime, int gears, IEnumerable<float> gearRatios, IEnumerable<float> finalRatios, IEnumerable<int> stockRatios, int stockFinal, DriveTrainSetup drive, float driveDistribution)
        {
            ClutchSlipTorque = clutchSlipTorque;
            ClutchFriction = clutchFriction;
            UpshiftTime = upshiftTime;
            DownshiftTime = downshiftTime;
            Gears = gears;
            GearRatios = gearRatios;
            FinalRatios = finalRatios;
            StockRatios = stockRatios;
            StockFinal = stockFinal;
            Drive = drive;
            DriveDistribution = driveDistribution;
        }
    }
}