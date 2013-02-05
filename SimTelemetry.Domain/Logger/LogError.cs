namespace SimTelemetry.Domain.Logger
{
    public enum LogError : uint
    {
        OK = 0,
        GroupNotFound = 0xFFFFFFFF,
        FieldNotFound = 0xFFFFFFFE,
        GroupAlreadyExists = 0xFFFFFFFD,
        FieldAlreadyExists = 0xFFFFFFFC,
    }
}