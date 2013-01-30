using System;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryReader
    {
        protected const uint PROCESS_QUERY_INFORMATION = (0x0400);
        protected const uint PROCESS_QUERY_LIMITED_INFORMATION = (0x0100);
        protected const uint PROCESS_VM_READ = (0x0010);

        protected Process _Process;
        protected IntPtr m_hProcess = IntPtr.Zero;

        private bool _diagnostic = false;
        private Timer _diagnosticTimer = null;
        public bool Diagnostic
        {
            get { return _diagnostic; }
            set
            {
                _diagnostic = value;
                if (_diagnostic)
                {
                    _diagnosticTimer = new Timer(1000);
                    _diagnosticTimer.Elapsed += (a, b) =>
                                                    {
                                                        ReadCalls = _readCalls;
                                                        _readCalls = 0;
                                                    };
                    _diagnosticTimer.AutoReset = true;
                    _diagnosticTimer.Start();
                }
                else
                {
                    if (_diagnosticTimer != null)
                    {
                        _diagnosticTimer.Stop();
                        _diagnosticTimer = null;
                    }

                }
            }
        }

        private int _readCalls;
        public int ReadCalls { get; private set; }

        public Process Process { get { return _Process; } }

        public virtual bool Open(Process p)
        {
            m_hProcess = MemoryReaderApi.OpenProcess(PROCESS_VM_READ, 0, (uint)p.Id);

            var result = ((m_hProcess == IntPtr.Zero) ? false : true);
            if (result) _Process = p;

            return result;
        }

        public virtual bool Close()
        {
            if (m_hProcess == null || m_hProcess == IntPtr.Zero)
                return false;

            var iRetValue = MemoryReaderApi.CloseHandle(m_hProcess);
            return iRetValue != 0;
        }

        public virtual byte[] Read(IntPtr memoryAddress, uint bytesToRead)
        {
            if (Diagnostic)
                _readCalls++;
            IntPtr ptrBytesReaded;
            var buffer = new byte[bytesToRead];
            MemoryReaderApi.ReadProcessMemory(m_hProcess, memoryAddress, buffer, bytesToRead, out ptrBytesReaded);
            return buffer;
        }

        public virtual bool Read(IntPtr memoryAddress, byte[] buffer)
        {
            if (Diagnostic)
                _readCalls++;
            IntPtr ptrBytesReaded;

            MemoryReaderApi.ReadProcessMemory(m_hProcess, memoryAddress, buffer, (uint)buffer.Length, out ptrBytesReaded);
            return ((int)ptrBytesReaded == buffer.Length);
        }


        public virtual bool Read(int memoryAddress, byte[] buffer)
        {
            if (Diagnostic)
                _readCalls++;
            IntPtr ptrBytesReaded;

            MemoryReaderApi.ReadProcessMemory(m_hProcess, (IntPtr)memoryAddress, buffer, (uint)buffer.Length, out ptrBytesReaded);
            return ((int)ptrBytesReaded == buffer.Length);
        }

        #region Read<IntPtr>
        public T Read<T>(IntPtr address, uint size, Func<byte[], int, T> converter)
        {
            return converter(Read(address, size), 0);
        }

        public byte ReadByte(IntPtr address)
        {
            return Read(address, 1)[0];
        }

        public byte[] ReadBytes(IntPtr address, uint size)
        {
            return Read(address, size);
        }

        public string ReadString(IntPtr address, uint size)
        {
            int i = 0;
            byte[] bt = ReadBytes(address, size);
            for (i = 0; i < bt.Length; i++)
            {
                if (bt[i] == 0)
                    break;
            }

            return Encoding.ASCII.GetString(bt, 0, i);
        }

        public double ReadDouble(IntPtr address)
        {
            return BitConverter.ToDouble(Read(address, 8), 0);
        }

        public float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(Read(address, 4), 0);
        }

        public short ReadInt16(IntPtr address)
        {
            return BitConverter.ToInt16(Read(address, 2), 0);
        }

        public int ReadInt32(IntPtr address)
        {
            return BitConverter.ToInt32(Read(address, 4), 0);
        }

        public long ReadInt64(IntPtr address)
        {
            return BitConverter.ToInt64(Read(address, 8), 0);
        }

        public ushort ReadUInt16(IntPtr address)
        {
            return BitConverter.ToUInt16(Read(address, 2), 0);
        }

        public uint ReadUInt32(IntPtr address)
        {
            return BitConverter.ToUInt32(Read(address, 4), 0);
        }

        public ulong ReadUInt64(IntPtr address)
        {
            return BitConverter.ToUInt64(Read(address, 8), 0);
        }
        #endregion
        #region Read<int>
        public T Read<T>(int address, uint size, Func<byte[], int, T> converter)
        {
            return converter(Read((IntPtr)address, size), 0);
        }

        public byte ReadByte(int address)
        {
            return Read((IntPtr)address, 1)[0];
        }

        public byte[] ReadBytes(int address, uint size)
        {
            return Read((IntPtr)address, size);
        }

        public string ReadString(int address, uint size)
        {
            int i = 0;
            byte[] bt = ReadBytes(address, size);
            for (i = 0; i < bt.Length; i++)
            {
                if (bt[i] == 0)
                    break;
            }

            return Encoding.ASCII.GetString(bt, 0, i);
        }

        public double ReadDouble(int address)
        {
            return BitConverter.ToDouble(Read((IntPtr)address, 8), 0);
        }

        public float ReadFloat(int address)
        {
            return BitConverter.ToSingle(Read((IntPtr)address, 4), 0);
        }

        public short ReadInt16(int address)
        {
            return BitConverter.ToInt16(Read((IntPtr)address, 2), 0);
        }

        public int ReadInt32(int address)
        {
            return BitConverter.ToInt32(Read((IntPtr)address, 4), 0);
        }

        public long ReadInt64(int address)
        {
            return BitConverter.ToInt64(Read((IntPtr)address, 8), 0);
        }

        public ushort ReadUInt16(int address)
        {
            return BitConverter.ToUInt16(Read((IntPtr)address, 2), 0);
        }

        public uint ReadUInt32(int address)
        {
            return BitConverter.ToUInt32(Read((IntPtr)address, 4), 0);
        }

        public ulong ReadUInt64(int address)
        {
            return BitConverter.ToUInt64(Read((IntPtr)address, 8), 0);
        }

        #endregion
    }
}