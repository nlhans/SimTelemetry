using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        public string TemporaryDirectory { get; private set; }
        public string FileName { get; protected set; }

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected ConcurrentBag<LogGroup> _groups = new ConcurrentBag<LogGroup>();

        #region Private fields
        private ZipStorer _archive;

        private Dictionary<string, FileStream> fileStreams = new Dictionary<string, FileStream>();
        private ConcurrentBag<LogFileWriteAction> _pendingWrites = new ConcurrentBag<LogFileWriteAction>();
        private bool _pendingWriteActive = false;
        private bool _pendingWriteBusy = false;
        private Thread _pendingWriteWorker;
        #endregion

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
                    var seekToBegin = false;
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
                            fm = FileMode.Truncate;
                            seekToBegin = true;
                            break;

                        case LogFileType.Laps:
                            file = "Laps.xml";
                            break;
                    }

                    string path = "";

                    if (action.Group != "")
                    {
                        path = "./"+TemporaryDirectory+"/" + action.Group + "/" + file;
                        if (!Directory.Exists("./" + TemporaryDirectory + "/" + action.Group + "/"))
                            Directory.CreateDirectory("./" + TemporaryDirectory + "/" + action.Group + "/");
                    }
                    else
                        path = "./" + TemporaryDirectory + "/" + file;

                    // Open the file stream.
                    if (!fileStreams.ContainsKey(path))
                    {
                        if (!File.Exists(path) && fm == FileMode.Truncate)
                        {
                            File.Delete(path);
                            fm = FileMode.Create;
                        }
                        fileStreams.Add(path, File.Open(path, fm));
                    }

                    if (action.FileType == LogFileType.Structure)
                    {
                        Debug.WriteLine("writing to structure.xml @ "+path);
                    }
                    FileStream fs = fileStreams[path];

                    if (seekToBegin)
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

        public LogFileWriter(string file, string tmp_dir)
        {
            TemporaryDirectory = tmp_dir;
            FileName = file;

            if (TemporaryDirectory != null && TemporaryDirectory != string.Empty)
            {
                if (Directory.Exists("./" + TemporaryDirectory + "/"))
                    Directory.Delete("./" + TemporaryDirectory + "/", true);

                Directory.CreateDirectory("./" + TemporaryDirectory + "/");
                
                
            }
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

            _archive = ZipStorer.Create(FileName, "SimTelemetry log");
            foreach (var file in Directory.GetFiles("./" + TemporaryDirectory + "/", "*", SearchOption.AllDirectories))
            {
                var tmpIndex = file.LastIndexOf(TemporaryDirectory+"/", 0, 1);
                if (tmpIndex == -1) tmpIndex = file.IndexOf("" + TemporaryDirectory + "/");
                var filenameInZip = file.Substring(tmpIndex+TemporaryDirectory.Length+1);

                _archive.AddFile(ZipStorer.Compression.Deflate, file, filenameInZip, "");
                File.Delete(file);
            }

            // Close zip file.
            _archive.Close();

            Directory.Delete("./" + TemporaryDirectory + "/", true);
        }

        public void Clear()
        {
            // We don't want this log file.
            if(_archive != null)
            _archive.Close();

            File.Delete(FileName);
            Directory.Delete("./" + TemporaryDirectory + "/", true);
        }
    }
}
