using System.Diagnostics;
using System.Linq;
using System.Timers;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;
using SimTelemetry.Domain.Telemetry;

namespace LiveTelemetry
{
    public static class TelemetryApplication
    {
        public static Plugins Plugins { get; private set; }


        public static Telemetry Telemetry { get; private set; }
        public static bool TelemetryAvailable { get { return Telemetry != null; } }

        public static Simulator Simulator { get; private set; }
        public static bool SimulatorAvailable { get; private set; }
        private static IPluginSimulator SimulatorPlugin { get; set; }


        public static bool NetworkHost { get; private set; }

        public static Car Car { get; private set; }
        public static bool CarAvailable { get; private set; }
        public static CarRepository Cars { get; private set; }

        public static Track Track { get; private set; }
        public static bool TrackAvailable { get; private set; }
        public static TrackRepository Tracks { get; private set; }

        public static void Init()
        {
            Plugins = new Plugins();
            Plugins.PluginDirectory = "./";
            Plugins.Load();

            // Simulator:
            GlobalEvents.Hook<SimulatorStarted>(x =>
                {
                    SimulatorPlugin = x.Sim;
                    Simulator = x.Sim.GetSimulator();
                    SimulatorAvailable = true;
                }, true);
            GlobalEvents.Hook<SimulatorStopped>(x =>
                {
                    SimulatorPlugin = null;
                    Simulator = null;
                    SimulatorAvailable = false;
                }, true);

            // TODO: Temporary
            GlobalEvents.Fire(new SimulatorStarted(Plugins.Simulators[0]), true);

            // Session:
            var rfactor = Process.GetProcessesByName("rfactor").FirstOrDefault();
            GlobalEvents.Hook<SessionStarted>(x =>
                {
                    SimulatorPlugin.SimulatorStart(rfactor);
                    Telemetry = new Telemetry(SimulatorPlugin.TelemetryProvider, rfactor);

                    Cars = new CarRepository(SimulatorPlugin.CarProvider);
                    Tracks = new TrackRepository(SimulatorPlugin.TrackProvider);

                    Timer t = new Timer {Interval = 500};
                    t.Elapsed += (s, e) =>
                        {
                            // TODO: Temporary

                            // Get track
                            if (TelemetryApplication.TelemetryAvailable)
                            {
                                Track = Tracks.GetByFile(Telemetry.Session.Track);
                                if (Track != null)
                                    TrackAvailable = true;

                                // Get car
                                Car = Cars.GetByClass(Telemetry.Player.CarClasses).FirstOrDefault();

                                if (Car != null)
                                    CarAvailable = true;
                            }
                            else
                            {
                                TrackAvailable = false;
                                CarAvailable = false;
                            }
                        };
                    t.Start();
                }, true);
            GlobalEvents.Hook<SessionStopped>(x =>
                {
                    Telemetry = null;
                }, true);
            if (rfactor != null)
            {
                GlobalEvents.Fire(new SessionStarted(), true);
            }
        }
    }
}
