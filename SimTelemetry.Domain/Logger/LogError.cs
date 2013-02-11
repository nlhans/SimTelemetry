namespace SimTelemetry.Domain.Logger
{
    public enum LogError : int
    {
        OK = 0,
        GroupNotFound = -1,
        FieldNotFound = -2,
        GroupAlreadyExists = -3,
        FieldAlreadyExists = -4,
        ReadOnly = -5
    }
}