namespace SimTelemetry.Core.ValueObjects
{
    public class EngineMode
    {
        public int Power { get; private set; }
        public int Torque { get; private set; }
        public int RPM { get; private set; }
        public int Fuel { get; private set; }

        public EngineMode(int power, int torque, int rpm, int fuel)
        {
            Power = power;
            Torque = torque;
            RPM = rpm;
            Fuel = fuel;
        }
    }
}