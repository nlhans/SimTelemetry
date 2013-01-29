using System;
using System.Runtime.InteropServices;

namespace SimTelemetry.Domain.Memory
{
    internal class MemoryReaderApi
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();
    }
}