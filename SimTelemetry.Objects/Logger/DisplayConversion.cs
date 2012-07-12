using System;

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class DisplayConversion : Attribute
    {
        private DataConversions _Conversion = 0;
        public DataConversions Conversion { get { return _Conversion; } }
        public DisplayConversion(DataConversions conversion)
        {
            _Conversion = conversion;
        }
    }
}