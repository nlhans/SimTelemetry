namespace SimTelemetry.Objects.Garage
{
    public interface IGarageCarGearbox
    {
        string File { get; }
        
        double ShiftTime_Up { get; }
        double ShiftTime_Down { get; }
        
        int Gears { get; }

        double[] Ratios { get; }
        double[] Finals { get; }

        double[] Ratio_Setup { get; }
        double[] Ratio_Stock { get; }

        double Final_Setup { get; }
        double Final_Stock{ get; }
    }
}