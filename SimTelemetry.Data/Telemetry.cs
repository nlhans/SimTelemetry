using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading;
using SimTelemetry.Data.Logger;
using SimTelemetry.Data.Track;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Peripherals;
using Triton;
using Triton.Database;

namespace SimTelemetry.Data
{
    struct Telemetry_SimState
    {
        public bool Active;
        public bool Session;
    }

    public sealed class Telemetry : ITelemetry
    {
        private static Telemetry _m = new Telemetry();
        public static Telemetry m { get { return _m; } }

        public ITrackParser Track { get; set; }

        public IDevices Peripherals;

        public Simulators Sims;

        public ISimulator Sim { get { return Sims.GetRunning(); } }
        public bool Active_Sim { get { if (Sims == null) return false;
            else
            return Sims.Available; } }
        public bool Active_Session { get {  if (Sims == null) return false;
            else
            return Active_Sim && Sim.Session.Active; } }

        private Dictionary<string, Telemetry_SimState> State = new Dictionary<string, Telemetry_SimState>();
        private bool AppActive = true;
        private Thread StatePooler;
        private TelemetryLogger Logger;
        #region Events

        public event Signal Sim_Start;
        public event Signal Sim_Stop;

        public event Signal Session_Start;
        public event Signal Session_Stop;

        public event Signal Track_Load;

        #endregion
        private IDbConnection GetConnection()
        {
            OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data source=Laptimes.accdb");
            return con;
        }

        public Telemetry()
        {
            // We always need database connection.

            DatabaseOleDbConnectionPool.MaxConnections = 4;
            DatabaseOleDbConnectionPool.Boot(GetConnection);

        }

        void TritonBase_PreExit()
        {
            AppActive = false;
        }

        public void Bootup(object no)
        {
            Sims = new Simulators();
            Logger = new TelemetryLogger(this);

            StatePooler = new Thread(SimStatePooler);
            StatePooler.Start();

            TritonBase.PreExit += new AnonymousSignal(TritonBase_PreExit);

            this.Session_Start += new Signal(Telemetry_Session_Start);

        }

        void Telemetry_Session_Start(object sender)
        {
            Track = new TrackParser(Sim.Session.GameDirectory, Sim.Session.GameData_TrackFile);
            Thread.Sleep(500);

            if (Track_Load != null)
                Track_Load(null);
        }

        private void SimStatePooler()
        {
            while (AppActive)
            {
                // Check all sims.
                foreach(ISimulator sim in this.Sims.Sims)
                {
                    if(State.ContainsKey(sim.ProcessName) == false)
                        State.Add(sim.ProcessName, new Telemetry_SimState{Active=false, Session=false});

                    Telemetry_SimState state = State[sim.ProcessName];

                    // Simulator events.
                    if(sim.Memory.Attached != state.Active)
                    {
                        state.Active = sim.Memory.Attached;
                        if (sim.Memory.Attached)
                        {
                            Report_SimStart(sim);
                        }
                        else
                        {
                            Report_SimStop(sim);

                            // Also fire session if it was still active (unexpected close)
                            if(state.Session)
                                Report_SessionStop(sim);
                        }
                    }

                    // Session events.
                    if (state.Active && sim.Session.Active != state.Session)
                    {
                        state.Session = sim.Session.Active;
                        if (state.Session)
                        {
                            Report_SessionStart(sim);
                        }
                        else
                        {
                            Report_SessionStop(sim);
                        }
                    }

                    // restore state.
                    State[sim.ProcessName] = state;
                }

                // 100Hz
                Thread.Sleep(10);
            }
        }

        // All code destioned for plugins.
        internal void Report_SimStart(ISimulator me)
        {
            Debug.WriteLine("SimStart fired");
            if (Sim_Start != null)
                Sim_Start(me);
        }

        internal void Report_SimStop(ISimulator me)
        {
            Debug.WriteLine("SimStop fired");
            if (Sim_Stop != null)
                Sim_Stop(me);
        }

        internal void Report_SessionStart(ISimulator me)
        {
            Debug.WriteLine("SessionStart fired");
            if (Session_Start != null)
                Session_Start(me);
        }

        internal void Report_SessionStop(ISimulator me)
        {
            Debug.WriteLine("SessionStop fired");
            if (Session_Stop != null)
                Session_Stop(me);
        }

        public void LoadTrack(string gamedir, string track)
        {
            Track = new TrackParser(gamedir, track);
        }

        public void UnloadTrack()
        {
            Track = null;
        }

        public void Run()
        {
            Peripherals = new Devices();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Bootup));
        }
    }
}