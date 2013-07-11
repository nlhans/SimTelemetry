using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;
using SimTelemetry.Domain.Telemetry;

namespace LiveTelemetry
{
    public static class TelemetryApplication
    {
        public static Telemetry Telemetry { get; set; }
        public static bool TelemetryAvailable { get { return Telemetry != null; } }

        public static Simulator Simulator { get; set; }
        public static bool SimulatorAvailable { get; set; }

        public static CarRepository Cars { get; set; }

        public static Plugins Plugins { get; set; }

        public static bool NetworkHost
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public static Car Car
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public static bool CarAvailable
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public static bool TrackAvailable
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public static Track Track { get; set; }

        public static void Init()
        {
            Plugins = new Plugins();
            Plugins.PluginDirectory = "./";
            Plugins.Load();
        }
    }
}
