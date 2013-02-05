using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Domain.Aggregates
{
    public class Telemetry
    {
        public bool IsRunning { get; protected set; }
        public bool IsDriving { get; protected set; }
        public bool IsLoading { get; protected set; }

        private MMTimer Clock;

        internal MemoryProvider Memory { get; set; }
        public IPluginTelemetryProvider Provider { get; protected set; }

        public ITelemetryLogger Logger { get; protected set; }

        #region Telemetry data yard
        public TelemetrySession Session { get; protected set; }
        public TelemetryGame Simulator { get; protected set; }

        public TelemetryAcquisition Acquisition { get; protected set; }

        public TelemetrySupport Support { get; protected set; }

        public IEnumerable<TelemetryDriver> Drivers { get { return _drivers; } }
        protected readonly IList<TelemetryDriver> _drivers = new List<TelemetryDriver>();

        public TelemetryDriver Player { get { return _player; } }
        protected TelemetryDriver _player;
        #endregion

        public Telemetry(IPluginTelemetryProvider provider, Process simulatorProcess)
        {
            Provider = provider;

            // Initialize memory objects
            var mem = new MemoryReader();
            mem.Open(simulatorProcess);

            Memory = new MemoryProvider(mem);
#if DEBUG
            Memory.Reader.Diagnostic = true;
#endif

            // Fill memory provider
            Memory.Scanner.Enable();
            Provider.Initialize(Memory);
            Memory.Scanner.Disable();

            // Start outside-world telemetry objects
            Session = new TelemetrySession(this);
            Simulator = new TelemetryGame(this);

            Acquisition = new TelemetryAcquisition(this);
            Support = new TelemetrySupport(this);

#if DEBUG
            Clock = new MMTimer(25);
#else
            Clock = new MMTimer(20);
#endif
            Clock.Tick += (o, s) => GlobalEvents.Fire(new TelemetryRefresh(this), true);
            Clock.Start();

            GlobalEvents.Hook<TelemetryRefresh>(Update, true);
        }

        ~Telemetry()
        {
            RemoveLogger();
            Provider.Deinitialize();
        }

        public void SetLogger(ITelemetryLogger logger)
        {
            Logger = logger;
            Logger.Initialize(this, Memory);
        }

        public void RemoveLogger()
        {
            Logger.Deinitialize();
            Logger = null;
        }

        public void Update(TelemetryRefresh instance)
        {
            // Updates memory pools and reads value to this class.
            Memory.Refresh();

            // Simulator environment etc.
            Support.Update();
            Simulator.Update();
            Session.Update();
            Acquisition.Update();

            // Drivers
            UpdateDrivers();
            foreach(var driver in Drivers)
                driver.Update();

            // Update game status reports.
            var isSessionLoading = Session.IsLoading;
            var sessionLoadingChanged = isSessionLoading != IsLoading;
            if (sessionLoadingChanged)
                SetLoadingStatus(isSessionLoading);

            var isSessionActive = Session.IsActive;
            var sessionActiveChanged = isSessionActive != IsRunning;
            if (sessionActiveChanged)
                SetSessionStatus(isSessionActive);

            if (isSessionActive == false || Player == null)
            {
                if (IsDriving)
                    SetDrivingStatus(false);
            }
            else
            {
                var isDriving = Player.IsDriving;
                if (isDriving != IsDriving)
                    SetDrivingStatus(isDriving);
            }
            if (Player != null)
            foreach (var field in Player.Pool.Fields)
                field.Value.Read();

            if (Logger != null)
                Logger.Update();
        }

        protected virtual void UpdateDrivers()
        {
            if (this.Session.Cars != _drivers.Count){

                // Get a new list of drivers
                var driverList = Memory.Get("Simulator").ReadAs<int[]>("Drivers");
                var validDriverList = driverList.Where(x => Provider.CheckDriverQuick(Memory, x)).ToArray();
                var playerDriverPtr = Memory.Get("Simulator").ReadAs<int>("CarPlayer");
                // Update the drivers.))
                foreach (var validDriverPtr in validDriverList)
                {
                    if (Drivers.Any(x => x.BaseAddress == validDriverPtr))
                        continue;

                    var isPlayer = validDriverPtr == playerDriverPtr;
                    var memPool = Memory.Get("DriverTemplate").Clone("Driver " + validDriverPtr, validDriverPtr);
                    Provider.CreateDriver(memPool, isPlayer);

                    Memory.Add(memPool);

                    var td = new TelemetryDriver(this, memPool);
                    _drivers.Add(td);

                    if (isPlayer)
                        _player = td;
                }

                var driverRemovalList = new List<TelemetryDriver>();

                foreach (var driver in _drivers)
                {
                    if (!validDriverList.Any(x => x == driver.BaseAddress))
                    {
                        // This driver is now invalid
                        driverRemovalList.Add(driver);
                    }

                }

                for (int i = 0; i < driverRemovalList.Count; i++)
                {
                    Memory.Remove(driverRemovalList[i].Pool);
                    _drivers.Remove(driverRemovalList[i]);

                    if (driverRemovalList[i] == _player)
                        _player = null;
                }
            }
        }

        public void SetSessionStatus(bool active)
        {
            if (active)
            {
                GlobalEvents.Fire(new SessionStarted(), true, 500);
            }
            else
            {
                GlobalEvents.Fire(new SessionStopped(), true, 500);
            }
            IsRunning = active;
        }

        public void SetDrivingStatus(bool active)
        {
            if (active)
            {
                GlobalEvents.Fire(new DrivingStarted(), true, 500);
            }
            else
            {
                GlobalEvents.Fire(new DrivingStopped(), true, 500);
            }
            IsDriving = active;
        }

        public void SetLoadingStatus(bool active)
        {
            if (active)
            {
                GlobalEvents.Fire(new LoadingStarted(), true, 500);
            }
            else
            {
                GlobalEvents.Fire(new LoadingFinished(), true, 500);
            }
            IsLoading = active;
        }
    }
}