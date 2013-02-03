using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Domain.Plugins;

namespace SimTelemetry.Tests.Telemetry
{
    [TestFixture]
    public class TelemetryTests
    {
        [Test]
        public void Basic()
        {
            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                testPlugin.Initialize();
                Process rf = Process.GetProcessesByName("rfactor")[0];
                testPlugin.SimulatorStart(rf);
                var telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);

                int i = 0;
                while (i++ < 100)
                {
                    telemetryObject.Update();
                    Debug.WriteLine(telemetryObject.Session.Time);

                    Thread.Sleep(10);
                }

            }
        }


        int calls = 0;
        private Domain.Aggregates.Telemetry telemetryObject;
        public void Continous()
        {
            using (var pluginHost = new Plugins())
            {
                pluginHost.PluginDirectory = TestConstants.SimulatorsBinFolder;

                pluginHost.Load();
                var testPlugin = pluginHost.Simulators[0];

                testPlugin.Initialize();
                Process rf = Process.GetProcessesByName("rfactor")[0];
                testPlugin.SimulatorStart(rf);
                telemetryObject = new Domain.Aggregates.Telemetry(testPlugin.TelemetryProvider, rf);

                MultimediaTimer t = new MultimediaTimer(1000);
                t.Tick += (o, s) =>
                                 {
                                     Console.WriteLine(calls);
                                     calls = 0;
                                 };
                t.Start();

                Stopwatch w = new Stopwatch();
                MultimediaTimer mt = new MultimediaTimer(20);
                mt.Tick += (a, b) =>
                               {
                                   w.Stop();
                                   long  time = w.ElapsedMilliseconds;
                                   w.Restart();
                                   Debug.WriteLine(time);
                                   telemetryObject.Update();
                                   calls++;
                               };
                mt.Start();

                Console.ReadLine();
                mt.Stop();
            }
        }

    }

    public
    class MultimediaTimer
    {
        private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
        private TimerEventHandler handler;

        public event EventHandler Tick;

        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventHandler handler, IntPtr user, int eventType);

        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);

        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);

        private int interval = 1;
        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                if (value <= 0) return;

                interval = value;
            }
        }

        private int timerID = 0;

        public MultimediaTimer(int i)
        {
            Interval = i;
        }

        public void Start()
        {
            if (timerID != 0) return;

            timeBeginPeriod(1);
            handler = new TimerEventHandler(TimerHandler);
            timerID = timeSetEvent(interval, 1, handler, IntPtr.Zero, EVENT_TYPE);
        }

        private void TimerHandler(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (Tick != null)
                Tick(this, new EventArgs());
        }

        public void Stop()
        {
            if (timerID == 0) return;

            int err = timeKillEvent(timerID);
            timeEndPeriod(1);
            timerID = 0;
        }

        public bool Enabled
        {
            get
            {
                return timerID != 0;
            }
        }
    }
}
