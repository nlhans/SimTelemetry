using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Domain.Events
{
    public class LogFileWriteAction
    {
        public LogFileWriteAction(LogFileWriter file, string @group, LogFileType filetype, byte[] data,int writeNumber)
        {
            File = file;
            Group = group;
            FileType = filetype;
            Data = data;
            WriteNumber = writeNumber;
        }

        public LogFileWriter File { get; private set; }
        public string Group { get; private set; }
        public byte[] Data { get; private set; }
        public LogFileType FileType { get; private set; }
        public int WriteNumber { get; private set; }
    }
}