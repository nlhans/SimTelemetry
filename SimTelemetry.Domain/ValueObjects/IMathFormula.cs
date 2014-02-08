namespace SimTelemetry.Domain.ValueObjects
{
    public interface IMathFormula
    {
        double Get(double input1);
        double Get(double input1, double input2);
    }
}