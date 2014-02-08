using System.Diagnostics;
using System.Linq;
using System.Timers;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Repositories;

namespace SimTelemetry.Domain.Services
{
    public static class TelemetryApplication
    {
        public static Plugins.Plugins Plugins { get; private set; }

        public static bool SessionAvailable { get { return Data != null && Data.IsRunning; } }

        public static Aggregates.Telemetry Data { get; private set; }
        public static bool TelemetryAvailable { get { return Data != null; } }

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

        private static Timer delayedSessionStartHandler = new Timer { Interval = 500 };

        public static void Init()
        {
            delayedSessionStartHandler.Elapsed += t_Elapsed;

            Plugins = new Plugins.Plugins();
            Plugins.PluginDirectory = "./";
            Plugins.Load();

            // Session:
            var rfactor = Process.GetProcessesByName("rfactor").FirstOrDefault();

            // Simulator:
            GlobalEvents.Hook<SimulatorStarted>(x =>
            {
                SimulatorPlugin = x.Sim;
                Simulator = x.Sim.GetSimulator();
                SimulatorAvailable = true;

                Cars = new CarRepository(SimulatorPlugin.CarProvider);
                Tracks = new TrackRepository(SimulatorPlugin.TrackProvider);


                if (rfactor != null)
                {
                    SimulatorPlugin.SimulatorStart(rfactor);
                    Data = new Aggregates.Telemetry(SimulatorPlugin.TelemetryProvider, rfactor);
                }

            }, true);
            GlobalEvents.Hook<SimulatorStopped>(x =>
            {
                SimulatorPlugin.SimulatorStop();
                SimulatorPlugin = null;
                Simulator = null;
                SimulatorAvailable = false;
                Data = null;
            }, true);

            // TODO: Temporary testing with rFactor [only]
            GlobalEvents.Fire(new SimulatorStarted(Plugins.Simulators[0]), true);
            GlobalEvents.Hook<SessionStarted>(x => delayedSessionStartHandler.Start(), true);
            //GlobalEvents.Hook<DrivingStarted>(x => t_Elapsed(0, null), true);
            GlobalEvents.Hook<SessionStopped>(x =>
            {
                delayedSessionStartHandler.Stop();

                CarAvailable = false;
                TrackAvailable = false;


                GlobalEvents.Fire(new CarUnloaded(Car), true);
                GlobalEvents.Fire(new TrackUnloaded(Track), true);

                Car = null;
                Track = null;

            }, true);

            if (rfactor != null)
            {
                //GlobalEvents.Fire(new SessionStarted(), true);
            }
        }

        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            var trackAvail = false;
            var carAvail = false;
            // Get track
            if (TelemetryAvailable)
            {
                Track = Tracks.GetByFile(Data.Session.Track);
                if (Track != null)
                    trackAvail = true;
            }
            else
            {
                Track = null;
                trackAvail = false;
            }

            if (TelemetryAvailable && Data.Player != null)
            {
                // Get car
                // TODO Provide an interface/abstraction for searching the right car.
                // based on priorities and different set of rules, 
                // TelemetryDriver, and others 'drivers', should implement this interface to let the CarRepo search the right car.
                if (!string.IsNullOrEmpty(Data.Player.CarFile) &&
                    Cars.AnyByFile(Data.Player.CarFile))
                {
                    carAvail = true;
                    Car = Cars.GetByFile(Data.Player.CarFile);
                }
                else
                {
                    var cars = Cars.GetByClasses(Data.Player.CarClasses);

                    if (cars.Any())
                    {
                        if (cars.Count() == 1)
                        {
                            carAvail = true;
                            Car = cars.FirstOrDefault();
                        }
                        else
                        {
                            cars = cars.Where(c => c.StartNumber == Data.Player.CarNumber);

                            if (!cars.Any())
                            {
                                Debug.WriteLine("Could not find car for this session.");
                            }
                            else
                            {
                                carAvail = true;
                                Car = cars.FirstOrDefault();
                            }
                        }
                    }
                    else
                    {
                        Car = Cars.GetByClass(Data.Player.CarModel).FirstOrDefault();
                        if (Car != null)
                            carAvail = true;
                    }
                }
            }
            else
            {
                Car = null;
            }

            if (carAvail != CarAvailable)
            {
                CarAvailable = carAvail;
                if (carAvail)
                {
                    GlobalEvents.Fire(new CarLoaded(Car), true);
                }
                else
                {
                    GlobalEvents.Fire(new CarUnloaded(Car), true);
                }
            }
            if (trackAvail != TrackAvailable)
            {
                TrackAvailable = trackAvail;
                if (carAvail)
                {
                    GlobalEvents.Fire(new TrackLoaded(Track), true);
                }
                else
                {
                    GlobalEvents.Fire(new TrackUnloaded(Track), true);
                }
            }
        }
    }

}
