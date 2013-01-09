using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using SimTelemetry.Core;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class Logger
    {
        private static List<GlobalEventLog> log;
        private static DateTime da;
        [Test]
        public void LogTest()
        {
            var sampleSize = 200;
            var sampleSets = 100;

            var r = new EngineRPM { Value = 100.0 };

            log = new List<GlobalEventLog>();

            GlobalEvents.HookLogger(logEvent);

            Console.WriteLine("Events");
            long total = 0;
            for (int k = 0; k < 10; k++)
            {
                var w = new Stopwatch();
                w.Start();
                for (int i = 0; i < sampleSets; i++)
                {
                    for (int j = 0; j < sampleSize; j++)
                    {
                        GlobalEvents.Fire(r, true);
                    }
                }
                w.Stop();
                total += w.ElapsedMilliseconds;
            }
            Console.WriteLine(total);

            Console.WriteLine("Original");
            total = 0;
            log = new List<GlobalEventLog>();
            for (int k = 0; k < 10; k++)
            {
                var w = new Stopwatch();
                w.Start();
                for (int i = 0; i < sampleSets; i++)
                {
                    for (int j = 0; j < sampleSize; j++)
                    {
                        log.Add(new GlobalEventLog(r, da));
                    }
                }
                w.Stop();
                total += w.ElapsedMilliseconds;
            }
            Console.WriteLine(total);
        }

        public void logEvent(ILoggableEvent e)
        {
            log.Add(new GlobalEventLog(e, da));
            //Debug.WriteLine("Logging " + e.GetHashCode());
        }
    }
}
