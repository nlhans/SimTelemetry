using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Triton.Debugging;

namespace SimTelemetry.Tests
{
    class App
    {

        static void Main(string[] args)
        {
            BitConverterTests l = new BitConverterTests();
            l.TestRfactor();
            Console.WriteLine("ghello");
            Console.ReadLine();
        }
    }
}
