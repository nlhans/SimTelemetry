using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public Track Track { get; protected set; }
        public Session Session { get; protected set; }

        public IEnumerable<int> TimeLine { get { return _timeLine; } }
        private readonly IList<int> _timeLine = new List<int>();

        public Dictionary<string, List<Lap>> Laps { get { return _laps; } }
        private readonly Dictionary<string, List<Lap>> _laps = new Dictionary<string, List<Lap>>();

        private Dictionary<IDataNode, Lap> activeLaps = new Dictionary<IDataNode, Lap>(); 

        public string TemporaryFile { get; private set; }
        public string TemporaryDirectory { get; private set; }

        private LogFileWriter _writer;
        private IDataProvider _dataSource;
        private Task _closeWriter;
        private int _lastTime = -1;

        protected static readonly string[] TimepathFields = new[] { "Index", "IsAI", "IsPlayer", "Laps", "Meter", "Speed" };

        public TelemetryLogger(string simulator, TelemetryLoggerConfiguration config)
        {
            TemporaryFile = "tmp.zip";
            Simulator = simulator;
            Configuration = config;

            GlobalEvents.Hook<SessionStarted>(LogStart, true);
            GlobalEvents.Hook<SessionStopped>(LogStop, true);

            GlobalEvents.Hook<DriversAdded>(UpdateStructure, true);
            GlobalEvents.Hook<DriversRemoved>(UpdateStructure, true);

            GlobalEvents.Hook<TelemetryLapComplete>(RecordLap, true);
        }

        private void RecordLap(TelemetryLapComplete obj)
        {
            if (_laps.ContainsKey(obj.Driver.Name))
                _laps[obj.Driver.Name].Add(obj.Lap);
            else
                _laps.Add(obj.Driver.Name, new List<Lap>(new[] {obj.Lap}));
        }

        public void SetDatasource(IDataProvider source)
        {
            _dataSource = source;
        }

        public void SetAnnotater(IFileAnnotater annotater)
        {
            Annotater = annotater;
        }

        public void SetTemporaryLocations(string file, string dir)
        {
            TemporaryFile = file;
            TemporaryDirectory = dir;
        }

        public void DriverRemovedHandler(DriversRemoved removed)
        {
            
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
            _writer = new LogFileWriter(TemporaryFile, TemporaryDirectory);
            _dataSource.MarkDirty();
        }

        public void LogStop(SessionStopped e)
         {
            if (_writer == null) return;
            if (Annotater == null)
            {
                _closeWriter = new Task((x) => ((LogFileWriter)x).Save(), _writer);
                _closeWriter.Start();
            }
            else
            {
                if (Annotater.QualifiesForStorage(this))
                {
                    _closeWriter = new Task((x) =>
                                               {
                                                   ((LogFileWriter) x).Save();
                                                   Annotater.Store(this, ((LogFileWriter) x));
                                               }, _writer);
                    _closeWriter.Start();
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
                var dt = time - _lastTime;

                if (dt > 0)
                {
                    _writer.Update(time);
                    _timeLine.Add(time);

                    // Compute which drivers have completed a lap
                    foreach (var group in _writer.Groups)
                    {
                        var laps = group.DataSource.ReadAs<int>("Laps");

                        // Create lap if the current driver does not have one
                        if (activeLaps.ContainsKey(group.DataSource) == false)
                        {
                            var createNewLap = new Lap(group.DataSource.ReadAs<int>("DriverIndex"), false, laps,
                                -1, -1, -1, -1, false, false);
                            activeLaps.Add(group.DataSource, createNewLap);
                        }
                        else
                        {
                            if (activeLaps[group.DataSource].LapNumber != laps)
                            {
                                // We've driven a new lap.
                                GlobalEvents.Fire(new TelemetryLapComplete(null, group.DataSource as TelemetryDriver, activeLaps[group.DataSource] ), true);
                            }
                        }
                        // Update lap with new information.

                    }
                }

                _lastTime = time;
            }
        }

        public void Close()
        {
            GlobalEvents.Unhook<SessionStarted>(LogStart);
            GlobalEvents.Unhook<SessionStopped>(LogStop);

            GlobalEvents.Unhook<DriversAdded>(UpdateStructure);
            GlobalEvents.Unhook<DriversRemoved>(UpdateStructure);

            GlobalEvents.Unhook<TelemetryLapComplete>(RecordLap);

            LogStop(null);
            if (_closeWriter != null)
            {
                _closeWriter.Wait();
                _closeWriter = null;
            }
        }
    }
}
