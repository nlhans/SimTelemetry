using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Domain.Events
{
    public class LogFileWriteAction
    {
        public LogFileWriteAction(LogFileWriter file, string @group, LogFileType filetype, byte[] data)
        {
            File = file;
            Group = group;
            FileType = filetype;
            Data = data;
        }

        public LogFileWriter File { get; private set; }
        public string Group { get; private set; }
        public byte[] Data { get; private set; }
        public LogFileType FileType { get; private set; }
    }
}