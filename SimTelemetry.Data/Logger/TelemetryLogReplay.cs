using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Logger
{
    public class TelemetryLogReplay : TelemetryLogReader
    {
        private Timer _mReplayTimer;

        private double FramedTime = 0;
        private DateTime Time;

        public double GetDouble(string key)
        {
            try
            {
                return (double) Get(key);
            }catch(Exception ex)
            {
                return 0;
            }
        }
        public int GetInt32(string key)
        {
            return (int)Get(key);
        }

        private object Get(string key)
        {
            return Get(FramedTime, key);
        }

        public TelemetryLogReplay(string file) : base(file)
        {
            _mReplayTimer = new Timer { Interval = 10, Enabled = true };
            _mReplayTimer.Elapsed += new ElapsedEventHandler(t_Elapsed);
        }

        public void Start()
        {
            Time = DateTime.Now;
            _mReplayTimer.Start();
        }

        public void Stop()
        {
            _mReplayTimer.Stop();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Match frame.
            double CurrentTime = DateTime.Now.Subtract(Time).TotalMilliseconds;

            double least_dt = 1000;
            double max_t = 0;
            double t = 0;
            lock (this.Samples)
            {
                foreach (KeyValuePair<double, TelemetrySample> kvp in this.Samples)
                {
                    double dt = Math.Abs(kvp.Key - CurrentTime);
                    if (dt < least_dt)
                    {
                        least_dt = dt;
                        t = kvp.Key;
                    }
                    max_t = Math.Max(kvp.Key, max_t);
                }
            }
            if (max_t < CurrentTime)
            {

                Time = DateTime.Now;
            }

            FramedTime = t;
        }
    }
}
