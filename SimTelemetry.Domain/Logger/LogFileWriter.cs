using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Domain.Logger
{
    public class LogFileWriter
    {
        public string FileName { get; protected set; }

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected ConcurrentBag<LogGroup> _groups = new ConcurrentBag<LogGroup>();

        private ZipStorer _archive;

        private Dictionary<string, FileStream> fileStreams = new Dictionary<string, FileStream>();
        private ConcurrentBag<LogFileWriteAction> _pendingWrites = new ConcurrentBag<LogFileWriteAction>();
        private bool _pendingWriteActive = false;
        private bool _pendingWriteBusy = false;
        private Thread _pendingWriteWorker;

        private void WriteWorker()
        {
            _pendingWriteBusy = true;

            while (_pendingWriteActive || _pendingWrites.Count>0)
            {
                var myActions = new List<LogFileWriteAction>();
                LogFileWriteAction myAction;
                while (_pendingWrites.TryTake(out myAction))
                    myActions.Add(myAction);

                foreach (var action in myActions.OrderBy(x => x.WriteNumber))
                {
                    var file = "";
                    var fm = FileMode.Append;
                    switch (action.FileType)
                    {
                        case LogFileType.Data:
                            file = "Data.bin";
                            break;

                        case LogFileType.Time:
                            file = "Time.bin";
                            break;

                        case LogFileType.Structure:
                            file = "Structure.xml";
                            fm = FileMode.Create;
                            break;

                        case LogFileType.Laps:
                            file = "Laps.xml";
                            break;
                    }

                    string path = "";

                    if (action.Group != "")
                    {
                        path = "./tmp/" + action.Group + "/" + file;
                        if (!Directory.Exists("./tmp/" + action.Group + "/"))
                            Directory.CreateDirectory("./tmp/" + action.Group + "/");
                    }
                    else
                        path = "./tmp/" + file;

                    // Open the file stream.
                    if (!fileStreams.ContainsKey(path))
                        fileStreams.Add(path, File.Open(path, fm));
                    
                    FileStream fs = fileStreams[path];
                    if (fm == FileMode.Create)
                        fs.Seek(0, SeekOrigin.Begin);

                    fs.Write(action.Data, 0, action.Data.Length);
                    fs.Flush();
                }

                Thread.Sleep(100);
            }

            foreach(var file in fileStreams.Keys.ToList())
            {
                fileStreams[file].Close();
                GC.SuppressFinalize(fileStreams[file]);
                fileStreams[file] = null;

            }
            fileStreams.Clear();
            fileStreams = null;

            Thread.Sleep(100);
            _pendingWriteBusy = false;
        }

        public LogFileWriter(string file)
        {
            FileName = file;
            _archive = ZipStorer.Create(file, "SimTelemetry log");

            if(Directory.Exists("./tmp/"))
                Directory.Delete("./tmp/", true);

            Directory.CreateDirectory("./tmp/");

            GlobalEvents.Hook<LogFileWriteAction>(WriteTemporaryFile, false);

            // Start my thread
            _pendingWriteWorker = new Thread(WriteWorker);
            _pendingWriteWorker.Priority = ThreadPriority.Lowest;
            _pendingWriteWorker.Start();

            _pendingWriteActive = true;
        }

        private void WriteTemporaryFile(LogFileWriteAction obj)
        {
            _pendingWrites.Add(obj);
        }

        public bool Subscribe(IDataNode dataSource)
        {
            LogGroup logGroup;

            if (_groups.Any(x => x.Name == dataSource.Name))
            {
                logGroup = _groups.Where(x => x.Name == dataSource.Name).FirstOrDefault();
                return logGroup.Resubscribe(dataSource);
            }

            logGroup = new LogGroup(this, dataSource.Name, dataSource);
            _groups.Add(logGroup);

            return true;
        }

        public bool Subscribe(IDataNode dataSource, string[] fieldLimit)
        {
            LogGroup logGroup;

            if (_groups.Any(x => x.Name == dataSource.Name))
            {
                logGroup = _groups.Where(x => x.Name == dataSource.Name).FirstOrDefault();
                return logGroup.Resubscribe(dataSource);
            }

            logGroup = new LogGroup(this, dataSource.Name, dataSource, fieldLimit);
            _groups.Add(logGroup);

            return true;
        }

        public bool Unsubscribe(IDataNode logGroup)
        {
            return Unsubscribe(logGroup.Name);
        }

        public bool Unsubscribe(string name)
        {
            if (!_groups.Any(x => x.Name == name))
                return false;

            _groups.Where(x => x.Name == name).FirstOrDefault().Close();

            return true;
        }

        public void Update(int time)
        {
            foreach (var group in _groups.ToList())
                group.Update(time);
        }

        public void Save()
        {
            // Unsubscribe all instances:
            _groups.ToList().ForEach(x => Unsubscribe(x.Name));

            // Try to close the thread.
            _pendingWriteActive = false;

            int timeout = 120000;
            while (_pendingWriteBusy && timeout > 0)
            {
                Thread.Sleep(25);
                timeout -= 25;
            }

            foreach(var file in Directory.GetFiles("./tmp/", "*", SearchOption.AllDirectories))
            {
                var tmpIndex = file.LastIndexOf("tmp/", 0, 1);
                if (tmpIndex == -1) tmpIndex = file.IndexOf("tmp/");
                var filenameInZip = file.Substring(tmpIndex+4);

                _archive.AddFile(ZipStorer.Compression.Deflate, file, filenameInZip, "");
            }

            // Close zip file.
            _archive.Close();

            Directory.Delete("./tmp/", true);
        }
    }
}
