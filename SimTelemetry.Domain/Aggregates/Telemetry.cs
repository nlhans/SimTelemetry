using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.Utils;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Aggregates
{
    public class Telemetry : ITelemetry
    {
        public bool IsRunning { get; protected set; }
        public bool IsDriving { get; protected set; }
        public bool IsLoading { get; protected set; }

        private MMTimer Clock;

        private MemoryProvider Memory { get; set; }
        public IPluginTelemetryProvider Provider { get; protected set; }

        public TelemetryLogger Logger { get; protected set; }

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
            if (provider == null)
                throw new Exception("Cannot accept an empty telemetry provider");

            Provider = provider;

            // Initialize memory objects
            var mem = new MemoryReader();
            mem.Open(simulatorProcess);

            Memory = new MemoryProvider(mem);

            // Initialize telemetry provider; this class gives us the address layout to read.
            Memory.Scanner.Enable();
            Provider.Initialize(Memory);
            Memory.Scanner.Disable();

            // Start outside-world telemetry objects
            Session = new TelemetrySession();
            Simulator = new TelemetryGame();
            Acquisition = new TelemetryAcquisition();
            Support = new TelemetrySupport();

            // Start 40Hz clock signal (25ms)
            Clock = new MMTimer(25);
            Clock.Tick += (o, s) => GlobalEvents.Fire(new TelemetryRefresh(this), true);
            Clock.Start();

            // Hook both events together:
            GlobalEvents.Hook<TelemetryRefresh>(Update, true);
        }

        ~Telemetry()
        {
            ResetLogger();
            Provider.Deinitialize();
        }

        public void SetLogger(TelemetryLogger logger)
        {
            Logger = logger;
            Logger.SetDatasource(Memory);
        }

        public void ResetLogger()
        {
            Logger.Close();
            Logger = null;
        }

        public void Update(TelemetryRefresh instance)
        {
            if (instance.Telemetry != this) return;

            // Updates memory pools and reads value to this class.
            Memory.Refresh();

            Session.Update(this, Memory);

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

            // Simulator environment etc.
            Support.Update(this, Memory);
            Simulator.Update(this, Memory);
            Acquisition.Update(this, Memory);

            if (isSessionActive)
            {
                // Drivers
                UpdateDrivers();
                foreach (var driver in Drivers)
                    driver.Update(this, Memory);
            }

            if (Player != null)
                foreach (var field in Player.Pool.Fields.Values.Cast<IMemoryObject>())
                    field.Read();

            if (Logger != null)
                Logger.Update((int)Math.Round(Session.Time*1000));
        }

        protected virtual void UpdateDrivers()
        {
            if (Session.Cars != _drivers.Count)
            {
                var driversAdded = new List<TelemetryDriver>();
                var driversRemoved = new List<TelemetryDriver>();

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
                    Provider.CreateDriver((MemoryPool)memPool, isPlayer);

                    Memory.Add(memPool);

                    var td = new TelemetryDriver(memPool);
                    _drivers.Add(td);
                    driversAdded.Add(td);

                    if (isPlayer)
                        _player = td;
                }


                foreach (var driver in _drivers)
                {
                    if (!validDriverList.Any(x => x == driver.BaseAddress))
                    {
                        // This driver is now invalid
                        driversRemoved.Add(driver);
                    }

                }

                for (int i = 0; i < driversRemoved.Count; i++)
                {
                    Memory.Remove(driversRemoved[i].Pool);
                    _drivers.Remove(driversRemoved[i]);

                    if (driversRemoved[i] == _player)
                        _player = null;
                }
                
                if (driversAdded.Count > 0)
                    GlobalEvents.Fire(new DriversAdded(this, driversAdded), true);
                if (driversRemoved.Count > 0)
                    GlobalEvents.Fire(new DriversRemoved(this, driversAdded), true);
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