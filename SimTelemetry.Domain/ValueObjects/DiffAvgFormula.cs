using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class DiffAvgFormula : IMathFormula, IValueObject<DiffAvgFormula>
    {
        public double AverageProportion { get; private set; }
        public double DifferenceProportion { get; private set; }

        public DiffAvgFormula(double averageProportion, double differenceProportion)
        {
            AverageProportion = averageProportion;
            DifferenceProportion = differenceProportion;
        }

        public double Get(double avg)
        {
            return avg*AverageProportion;
        }

        public double Get(double avg, double diff)
        {
            return avg*AverageProportion + diff*DifferenceProportion;
        }


        public bool Equals(DiffAvgFormula other)
        {
            return (other.AverageProportion == AverageProportion && other.DifferenceProportion == DifferenceProportion);
        }
    }
}