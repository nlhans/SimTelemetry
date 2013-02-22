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

        private List<string> temporaryFiles = new List<string>();
        private int pendingFileWrites = 0;

        private ZipStorer _archive;

        public LogFileWriter(string file)
        {
            FileName = file;
            _archive = ZipStorer.Create(file, "SimTelemetry log");

            if(Directory.Exists("./tmp/"))
                Directory.Delete("./tmp/", true);

            Directory.CreateDirectory("./tmp/");

            GlobalEvents.Hook<LogFileWriteAction>(StartWriteTemporaryFile, false);
        }

        private void StartWriteTemporaryFile(LogFileWriteAction lfwa)
        {
            if (lfwa.File != this) return;
            pendingFileWrites++;
            ThreadPool.QueueUserWorkItem(WriteTemporaryFile, lfwa);

            
        }
        private void WriteTemporaryFile(object oLfwa)
        {
            var lfwa = (LogFileWriteAction) oLfwa;
            pendingFileWrites--;

            var file = "";
            var fm = FileMode.Append;
            switch (lfwa.FileType)
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

            if (lfwa.Group != "")
            {
                path = "./tmp/" + lfwa.Group + "/" + file;
                if (!Directory.Exists("./tmp/" + lfwa.Group + "/"))
                    Directory.CreateDirectory("./tmp/" + lfwa.Group + "/");
            }
            else
                path = "./tmp/" + file;

            // Open the file stream.
            FileStream fs = File.Open(path, fm);
            fs.Write(lfwa.Data, 0, lfwa.Data.Length);
            fs.Close();

            if (temporaryFiles.Contains(path) == false)
                temporaryFiles.Add(path);
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

            int timeout = 30000;
            while (pendingFileWrites > 0 && timeout > 0)
            {
                Thread.Sleep(25);
                timeout -= 25;
            }

            foreach (var file in temporaryFiles.ToList())
            {
                _archive.AddFile(ZipStorer.Compression.Deflate, file,
                                 file.Substring(6), ""); // 6 is removing ./tmp/
                File.Delete(file);
            }

            // Close zip file.
            _archive.Close();

            Directory.Delete("./tmp/", true);
        }
    }
}
