using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimTelemetry.Domain.Exceptions;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Domain.Logger
{
    public class LogFileReader
    {
        public string FileName { get; protected set; }
        public IEnumerable<LogGroup> Groups { get { return _groups; } }

        private ZipStorer zipFile;
        private List<string> zipFiles = new List<string>();
        private List<LogGroup> _groups = new List<LogGroup>();

        public LogFileReader(string file)
        {
            FileName = file;
            try
            {
                zipFile = ZipStorer.Open(file, FileAccess.Read);
                zipFiles = zipFile.ReadCentralDir().Select(x => x.FilenameInZip).ToList();
                _groups = zipFiles.Select(Path.GetDirectoryName).Distinct().Select(x => new LogGroup(this, x)).ToList();
            }
            catch(Exception ex)
            {
                throw new LogFileException("Error while reading log file '" + file +"'", ex);
            }
        }

        internal byte[] ReadArchiveFile(string file)
        {
            var targetFile = zipFile.ReadCentralDir().Where(x => x.FilenameInZip == file);
            
            if (targetFile.Count() == 0)
                return new byte[0];

            using (var bufferStream = new MemoryStream())
            {
                zipFile.ExtractFile(targetFile.FirstOrDefault(), bufferStream);

                return bufferStream.ToArray();
            }
        }

        public LogSampleProvider GetProvider(string[] groups, int start, int end)
        {
            return new LogSampleProvider(this, groups, start, end);
        }

        public LogGroup GetGroup(string group)
        {
            if (_groups.Any(x => x.Name == group))
                return _groups.Where(x => x.Name == group).FirstOrDefault();
            return null;
        }
    }
}
