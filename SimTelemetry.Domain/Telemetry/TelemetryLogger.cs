using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryLogger
    {
        public TelemetryLoggerConfiguration Configuration { get; private set; }
        public IFileAnnotater Annotater { get; protected set; }
        public string Simulator { get; private set; }

        public IEnumerable<int> TimeLine { get { return _TimeLine; } }
        private readonly IList<int> _TimeLine = new List<int>();

        private LogFileWriter _writer;
        private IDataProvider _dataSource;

        protected static readonly string[] TimepathFields = new[] { "Index", "IsAI", "IsPlayer", "Laps", "Meter", "Speed" };

        private int lastTime = -1;

        public Track Track;
        public Session Session;

        public TelemetryLogger(string simulator, TelemetryLoggerConfiguration config)
        {
            Simulator = simulator;
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
            if (_writer != null) return;
            _writer = new LogFileWriter("tmp.zip");
            _dataSource.MarkDirty();
        }

        public void LogStop(SessionStopped e)
         {
            if (_writer == null) return;
            if (Annotater == null)
            {
                ThreadPool.QueueUserWorkItem(x => ((LogFileWriter)x).Save(), _writer);
            }
            else
            {
                if (Annotater.QualifiesForStorage(this))
                {
                    var t = new Task((x) => {
                                                         ((LogFileWriter)x).Save();
                                                         Annotater.Store(this, ((LogFileWriter)x));
                                                     }, _writer);
                    t.Start();
                    t.Wait();
                }
                else
                {
                    _writer.Clear();
                }
            }

            _writer = null;
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
        public void Update(int time)
        {
            if (_writer != null)
            {
                var dt = time - lastTime;

                if (dt > 0)
                {
                    _writer.Update(time);
                    _TimeLine.Add(time);
                }

                lastTime = time;
            }
        }

        public void Close()
        {
            LogStop(null);
        }

        public void SetAnnotater(IFileAnnotater annotater)
        {
            Annotater = annotater;
        }

    }
}
