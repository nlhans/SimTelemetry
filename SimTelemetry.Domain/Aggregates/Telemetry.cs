using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Aggregates
{
    public class Telemetry
    {
        public bool ActiveGame { get; protected set; }
        public bool ActiveSession { get; protected set; }
        public bool ActiveDriving { get; protected set; }

        public IEnumerable<TelemetryDriver> Drivers { get { return _drivers; } }
        private IList<TelemetryDriver> _drivers = new List<TelemetryDriver>();

        internal MemoryProvider Memory { get; set; }

        public IPluginTelemetryProvider Provider { get; protected set; }

        public TelemetrySession Session { get; protected set; }
        public TelemetryTrack Track { get; protected set; }
        public TelemetryGame Simulator { get; protected set; }

        public TelemetryAcquisition Acquisition { get; protected set; }

        public TelemetrySupport Support { get; protected set; }

        public Telemetry(IPluginTelemetryProvider provider, Process simulatorProcess)
        {
            Provider = provider;

            // Initialize memory objects
            var mem = new MemoryReader();
            mem.Open(simulatorProcess);

            Memory = new MemoryProvider(mem);

            // Fill memory provider
            Memory.Scanner.Enable();
            Provider.Initialize(Memory);
            Memory.Refresh();
            Memory.Scanner.Disable();

            // Start outside-world telemetry objects
            Session = new TelemetrySession(this);
            Track = new TelemetryTrack(this);
            Simulator = new TelemetryGame(this);

            Acquisition = new TelemetryAcquisition(this);
            Support = new TelemetrySupport(this);
        }

         ~Telemetry()
        {
            Provider.Deinitialize();
        }

        public void Update()
        {
            // Updates memory pools and reads value to this class.
            Memory.Refresh();

            this.Support.Update();
            this.Simulator.Update();
            this.Session.Update();
            this.Track.Update();

            this.Acquisition.Update();

            foreach(var driver in Drivers)
                driver.Update();

        }

        public void SetGameStatus(bool active)
        {
            if (active)
            {
                GlobalEvents.Fire(new SimulatorStarted(), true);
            }
            else
            {
                GlobalEvents.Fire(new SimulatorStopped(), true);
            }
            ActiveGame = active;
        }

        public void SetSessionStatus(bool active)
        {
            if (active)
            {
                GlobalEvents.Fire(new SessionStarted(), true);
            }
            else
            {
                GlobalEvents.Fire(new SessionStopped(), true);
            }
            ActiveSession = active;
        }

        public void SetDrivingStatus(bool active)
        {
            if (active)
            {
                GlobalEvents.Fire(new DrivingStarted(), true);
            }
            else
            {
                GlobalEvents.Fire(new DrivingStopped(), true);
            }
            ActiveDriving = active;
        }

        public void AddDriver(TelemetryDriver driver)
        {
            if (!_drivers.Contains(driver))
                _drivers.Add(driver);
            GlobalEvents.Fire(new TelemetryDriverAdded(driver), true);
        }

        public void RemoveDriver(TelemetryDriver driver)
        {
            if (_drivers.Contains(driver))

                _drivers.Remove(driver);

            GlobalEvents.Fire(new TelemetryDriverRemoved(driver), true);
        }
    }

}