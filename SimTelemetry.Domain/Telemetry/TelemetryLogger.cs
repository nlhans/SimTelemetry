using System;
using System.Linq;
using System.Text;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryLogger
    {
        public TelemetryLoggerConfiguration Configuration { get; private set; }

        private LogFileWriter _writer;
        private IDataProvider _dataSource;

        protected static readonly string[] TimepathFields = new[] { "Index", "IsAI", "IsPlayer", "Laps", "Meter", "Speed" };

        public TelemetryLogger(TelemetryLoggerConfiguration config)
        {
            Configuration = config;

            GlobalEvents.Hook<SessionStarted>(LogStart, true);
            GlobalEvents.Hook<SessionStopped>(LogStop, true);

            GlobalEvents.Hook<DriversAdded>(UpdateStructure, true);
            GlobalEvents.Hook<DriversRemoved>(UpdateStructure, true);
        }

        public void SetDatasource(IDataProvider source)
        {
            _dataSource = source;
        }

        public void UpdateStructure(object driversAction)
        {
            if (_dataSource == null) return;

            foreach(var node in _dataSource.GetAll())
            {
                //
                var logLevel = Qualifies(node);

                if (logLevel == TelemetryLoggerLevel.None)
                {
                    continue;
                }
                if (logLevel == TelemetryLoggerLevel.Full)
                {
                    _writer.Subscribe(node);
                }
                if(logLevel == TelemetryLoggerLevel.Timepath)
                {
                    _writer.Subscribe(node, TimepathFields);
                }
            }

            _dataSource.MarkDirty();
        }

        public void LogStart(SessionStarted e)
        {
            if (_writer == null)
            {
                _writer = new LogFileWriter("tmp.zip");
            }
        }

        public void LogStop(SessionStopped e)
         {
             if (_writer != null)
             {
                 _writer.Save();
                 GlobalEvents.Fire(new LogFinished(_writer, Configuration), true);
                 _writer = null;
             }
         }

        public TelemetryLoggerLevel Qualifies(IDataNode node)
        {
            if (Configuration == null)
                return TelemetryLoggerLevel.None;

            var pool = (MemoryPool) node;
            if (pool.IsTemplate) 
                return TelemetryLoggerLevel.None;

            var isAI = pool.ReadAs<bool>("IsAI");

            var recordTimePath = false;
            var recordTelemetry = true;

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
                return TelemetryLoggerLevel.Timepath;

            if (!recordTelemetry)
                return TelemetryLoggerLevel.None;

            return TelemetryLoggerLevel.Full;
        }

        private int lastTime = 0;
        public void Update(int time)
        {
            if (_writer != null)
            {
                var dt = time - lastTime;

                if (dt > 0)
                    _writer.Update(time);

                lastTime = time;
            }
        }

        public void Close()
        {
            LogStop(null);
        }
    }
}
