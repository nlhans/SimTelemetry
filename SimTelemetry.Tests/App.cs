
using System;
using System.Diagnostics;

namespace SimTelemetry.Tests
{
    class App
    {

        static void Main(string[] args)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            MemoryTests l = new MemoryTests();
            l.TestRfactor();
            w.Stop();
            
#if DEBUG
            Debug.WriteLine("Time: " + w.ElapsedMilliseconds);
#else
            Console.WriteLine("Time : " +w.ElapsedMilliseconds );
            Console.ReadLine();
#endif

        }
    }
}
