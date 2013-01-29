using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Aggregates
{
    public class Telemetry
    {
        private IList<ITelemetryDriver> _drivers = new List<ITelemetryDriver>();

        public bool ActiveGame { get; private set; }
        public bool ActiveSession { get; private set; }
        public bool ActiveDriving { get; private set; }

        public IEnumerable<ITelemetryDriver> Drivers { get { return _drivers; } }

        public MemoryProvider Memory { get; private set; }

        public ITelemetrySession Session { get; private set; }
        public ITelemetryTrack Track { get; private set; }
        public ITelemetryGame Simulator { get; private set; }

        public ITelemetryAcquisition Acquisition { get; private set; }

        public ITelemetrySupport Support;

        public Telemetry(ITelemetrySupport support, ITelemetryPlayer player, ITelemetrySession session, ITelemetryTrack track, ITelemetryGame simulator, ITelemetryAcquisition acquisition)
        {
            Support = support;
            Session = session;
            Track = track;
            Simulator = simulator;
            Acquisition = acquisition;
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

        public void AddDriver(ITelemetryDriver driver)
        {
            if (!_drivers.Contains(driver))
                _drivers.Add(driver);
            GlobalEvents.Fire(new TelemetryDriverAdded(driver), true);
        }

        public void RemoveDriver(ITelemetryDriver driver)
        {
            if (_drivers.Contains(driver))
                _drivers.Remove(driver);

            GlobalEvents.Fire(new TelemetryDriverRemoved(driver), true);
        }
    }

    public interface ITelemetryPlugin
    {
        MemoryProvider InitPools();
        MemoryPool CreateDriver(int reference);
    }

    public interface ITelemetrySupport
    {
    }

    public interface ITelemetryAcquisition
    {
    }

    public interface ITelemetryGame
    {
    }

    public interface ITelemetrySession
    {
    }

    public interface ITelemetryTrack
    {
    }

    public interface ITelemetryPlayer
    {
    }

    public interface ITelemetryDriver : IEquatable<ITelemetryDriver>
    {
    }
}