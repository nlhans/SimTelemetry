using System;
using SimTelemetry.Domain.Events;

namespace SimTelemetry.Domain.Logger
{
    public class LogGroupStream
    {
        public int Size { get; private set; }
        public LogFileType Type{ get; private set; }

        public LogGroup Group { get; private set; }

        public int Offset { get; private set; }
        public int Index { get; private set; }


        public bool Closed { get; private set; }

        private int writeCycle = 0;
        protected byte[] Buffer;

        public LogGroupStream(LogGroup group, LogFileType type, int size)
        {
            Group = group;
            Type = type;
            Size = size;

            AllocateBuffer();
        }

        private void AllocateBuffer()
        {
            Index = 0;
            Buffer = new byte[Size];
            Closed = false;
        }

        private void FreeBuffer()
        {
            Offset += Index;

            Array.Resize(ref Buffer, Index);
            GlobalEvents.Fire(new LogFileWriteAction(Group.FileWriter, Group.Name, this.Type, Buffer, writeCycle),true);

            writeCycle++;

            Buffer = null;
            Closed = true;

        }

        public void Write(byte[] data)
        {
            bool IsDirty = false;

            if (Buffer == null)
                return;
            if (Index + data.Length > Buffer.Length)
            {
                Array.Resize(ref Buffer, Index + Math.Max(8 * 1024, data.Length));
                IsDirty = true;
            }

            Array.Copy(data, 0, Buffer, Index, data.Length);
            Index += data.Length;

            if (IsDirty)
            {
                FreeBuffer();
                AllocateBuffer();
                IsDirty = false;
            }
        }

        public void Close()
        {
            FreeBuffer();
        }

    }
}