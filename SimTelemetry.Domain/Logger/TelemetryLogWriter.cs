using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Memory;
using Triton.Maths;

namespace SimTelemetry.Domain.Logger
{
    public class TelemetryLogWriter : ITelemetryLogger
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }
        protected MemoryProvider Memory;

        public LogFile Log { get { return _log; } }
        protected LogFile _log;

        private int Samples = 0;
        private int lastSize = 0;
        private int sizePerSec = 0;
        private float lt = 0.0f;
        private Filter averageTime = new Filter(100);
        private float LastTime = 0;

        public ITelemetryLogConfiguration Configuration { get; protected set; }

        protected static readonly string[] TimepathFields = new[] { "Index", "IsAI", "IsPlayer", "Laps", "Meter", "Speed" };

        public void Initialize(Aggregates.Telemetry telemetry, MemoryProvider memory)
        {
            Telemetry = telemetry;
            Memory = memory;

            GlobalEvents.Hook<DrivingStarted>(Handle_StartDriving, true);
            GlobalEvents.Hook<DrivingStopped>(Handle_StopDriving, true);
            GlobalEvents.Hook<SessionStarted>(Handle_StartLogfile, true);
            GlobalEvents.Hook<SessionStopped>(Handle_StopLogfile, true);
            GlobalEvents.Hook<DriversAdded>(Handle_DriverAdd, true);
            GlobalEvents.Hook<LoadingStarted>((x) => Console.WriteLine("loading started [" + Samples + "]"), true);
            GlobalEvents.Hook<LoadingFinished>((x) => Console.WriteLine("loading finished [" + Samples + "]"), true);

            // Log nothing
            Configuration = new TelemetryLogConfiguration(false, false, false, false);
        }

        public void Deinitialize()
        {
            GlobalEvents.Unhook<DrivingStarted>(Handle_StartDriving);
            GlobalEvents.Unhook<DrivingStopped>(Handle_StopDriving);
            GlobalEvents.Unhook<SessionStarted>(Handle_StartLogfile);
            GlobalEvents.Unhook<SessionStopped>(Handle_StopLogfile);
            GlobalEvents.Unhook<DriversAdded>(Handle_DriverAdd);
        }

        public void UpdateConfiguration(ITelemetryLogConfiguration configuration)
        {
            // 
            this.Configuration = configuration;
        }

        public void UpdateData()
        {
            if (_log == null) return;
            // Log all interesting variables to disk.
            // Log the following instances:
            // Session, Simulator + All drivers

            // Get time first
            var SampleTime = Memory.Get("Session").ReadAs<float>("Time");
            if (SampleTime < LastTime) LastTime = 0;
            if (SampleTime - LastTime < 1.0f/50.0f)
                return;
            LastTime = SampleTime;
            Samples++;

            //Console.Clear();))
            foreach (var pool in Memory.Pools)
            {
                if (pool.IsTemplate) continue;

                // Obey to configuration settings.
                if (pool.Name.StartsWith("Driver"))
                {
                    bool isAI = pool.ReadAs<bool>("IsAI");

                    bool recordTimePath = false;
                    bool recordTelemetry = true;

                    if (Configuration.RecordTimePathsAll)
                        recordTimePath = true;

                    // Turn off for AI:
                    if (isAI)
                    {
                        if (!Configuration.RecordTimePathsAI)
                            recordTimePath = false;

                        if (!Configuration.DriversLogAI)
                            recordTelemetry = false;
                    }

                    if (!Configuration.DriversLogAll)
                    {
                        // Check if this index is present in the selective array
                        var index = pool.ReadAs<int>("Index");
                        if (!Configuration.DriversLogSelective.Contains(index))
                            recordTelemetry = false;
                    }
                    if (recordTimePath && !recordTelemetry)
                    {
                        // Time paths mean recording the meters, speed and lap number
                        // The time is already kept in the log file itself. (flush routine).
                        // A time path can be handy for calculating sector times, differences, and 'overview' of corner speeds etc.
                        // It may be useful to record only this for competitor cars instead of all (40+) telemetry fields.
                        // This special routine only performs when the driver is skipped from logging complete telemetry.
                        foreach (var timePathField in pool.Fields.Where(x => TimepathFields.Contains(x.Key)))
                        {
                            if (timePathField.Value.HasChanged())
                                _log.Write(pool.Name, timePathField.Value.Name,
                                           MemoryDataConverter.Rawify(timePathField.Value.Read));
                        }
                    }

                    if (!recordTelemetry)
                        continue;
                }

                foreach (var fields in pool.Fields)
                {
                    if (fields.Value.IsConstant) continue;
                    if (fields.Value.HasChanged())
                    {
                        //Console.WriteLine(fields.Value.GetType().Name + " <> " + pool.Name + "." + fields.Value.Name);
                        _log.Write(pool.Name, fields.Value.Name, MemoryDataConverter.Rawify(fields.Value.Read));
                    }
                }

            }
            if (Samples%50 == 0)
            {
                sizePerSec = _log.dataSize - lastSize;
                lastSize = _log.dataSize;

            }

            _log.Flush(Convert.ToInt32(SampleTime*1000));
            /*
#if DEBUG
            var ft = Memory.Get("Session").ReadAs<float>("Time");
            if (lt != 0.0 && ft - lt != 0.0)
                averageTime.Add(ft - lt);
            Debug.WriteLine((ft - lt).ToString("00.000") + "," + averageTime.Average.ToString("0.00000") + " - " +
                            Math.Round(_log.dataSize/1024.0/1024.0, 3) + "MB (" + Math.Round(sizePerSec/1024.0, 3) +
                            "kB/s - " + Math.Round(sizePerSec*3600/1024.0/1024, 3) + "MB 1 hour)");
            lt = ft;
#endif
             */
        }

        #region Global events
        private void Handle_DriverAdd(DriversAdded obj)
        {
            if (obj.Telemetry == Telemetry)
                // update log from top ..
                UpdateLogStructure(Memory.Pools, _log);
        }

        private void Handle_StopLogfile(SessionStopped obj)
        {
            Console.WriteLine("session stopped [" + Samples + "]");
            _log.Finish("Telemetry.zip");
            _log = null;

        }

        protected void Handle_StartLogfile(SessionStarted e)
        {
            Console.WriteLine("session started [" + Samples + "]");
            _log = new LogFile();
            UpdateLogStructure(Memory.Pools, _log);
        }

        protected void Handle_StartDriving(DrivingStarted e)
        {
            Console.WriteLine("Starting log [" + Samples + "]");
        }

        protected void Handle_StopDriving(DrivingStopped e)
        {

            Console.WriteLine("Stopping log [" + Samples + "]");
        }
        #endregion
        #region Log file structure

        protected void UpdateLogStructure(IEnumerable<MemoryPool> pools, ILogNode node)
        {
            if (node == null)
                return;

            Memory.Refresh();

            LogGroup group;
            foreach(var pool in pools)
            {
                if (pool.IsTemplate) continue;

                // Obey to configuration settings.
                if (pool.Name.StartsWith("Driver"))
                {
                    bool isAI = pool.ReadAs<bool>("IsAI");

                    bool recordTimePath = false;
                    bool recordTelemetry = true;

                    if (Configuration.RecordTimePathsAll)
                        recordTimePath = true;

                    // Turn off for AI:
                    if (isAI)
                    {
                        if (!Configuration.RecordTimePathsAI)
                            recordTimePath = false;

                        if (!Configuration.DriversLogAI)
                            recordTelemetry = false;
                    }

                    if (!Configuration.DriversLogAll)
                    {
                        // Check if this index is present in the selective array
                        var index = pool.ReadAs<int>("Index");
                        if (!Configuration.DriversLogSelective.Contains(index))
                            recordTelemetry = false;
                    }
                    if (recordTimePath && !recordTelemetry)
                    {

                        group = node.CreateGroup(pool.Name);

                        // Time paths mean recording the meters, speed and lapnumber
                        // This special routine only performs when the driver is skipped from logging complete telemetry.
                        foreach (var timePathField in pool.Fields.Where(x => TimepathFields.Contains(x.Key)))
                        {

                            group.CreateField(timePathField.Value.Name, timePathField.Value.ValueType, pool.IsConstant);
                        }
                    }

                    if (!recordTelemetry)
                        continue;
                }


                // Does this top-level group already exist?
                if (node.ContainsGroup(pool.Name))
                    group = (LogGroup) node.FindGroup(pool.Name);
                else
                    group = node.CreateGroup(pool.Name);

                foreach (var field in pool.Fields)
                {
                    if (!group.ContainsField(field.Value.Name))
                        group.CreateField(field.Value.Name, field.Value.ValueType, field.Value.IsConstant);
                }
                if(pool.Pools.Count > 0)
                    UpdateLogStructure(pool.Pools.Values, group);
            }
        }
        #endregion
    }
}

