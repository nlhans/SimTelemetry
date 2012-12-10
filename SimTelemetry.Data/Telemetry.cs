/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading;
using SimTelemetry.Data.Logger;
using SimTelemetry.Data.Net;
using SimTelemetry.Data.Stats;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Peripherals;
using Triton;
using Triton.Database;

namespace SimTelemetry.Data
{
    public sealed class Telemetry : ITelemetry
    {
        /// <summary>
        /// Class containing whether a simulator is running and has a session active.
        /// </summary>
        internal class Telemetry_SimState
        {
            public bool Active;
            public bool Session;
            public bool Driving;
            public int Laps;
            public Telemetry_SimState()
            {
                Laps = -1;
                Driving = false;
                Active = false;
                Session = false;
            }
        }
        /// <summary>
        /// Single-ton Telemetry for general access everywhere.
        /// </summary>
        public static readonly Telemetry m = new Telemetry();

        /// <summary>
        /// Track parser containing data about the current track.
        /// </summary>
        /// <remarks>Data is not directly available after Session_Start is fired.</remarks>
        public ITrackParser Track { get; set; }

        /// <summary>
        /// Data sent to hardware devices. Currently only used for exclusive peripherals.
        /// </summary>
        public IDevices Peripherals;

        /// <summary>
        /// Collection of simulators available in the plug-in directory. (As of debug it's the bin/ directory).
        /// </summary>
        public Simulators Sims { get; set; }

        /// <summary>
        /// Gets the running simulator. Returns null if not available.
        /// </summary>
        public ISimulator Sim { get {  return Sims.GetRunning(); } }

        /// <summary>
        /// General calculations based on game data.
        /// </summary>
        public Computations Computations = new Computations();

        /// <summary>
        /// Returns true if a simulator is active. 
        /// </summary>
        public bool Active_Sim
        {
            get
            {
                if (Sims == null) // in case Sims is not initialized yet.
                    return false;
                else
                    return Sims.Available;
            }
        }

        /// <summary>
        /// Returns if any simulator has an active session.
        /// </summary>
        public bool Active_Session 
        { 
            get
            {
                if (Sims == null)
                    return false;
                else
                    return Active_Sim && Sim.Session.Active;
            }
        }

        /// <summary>
        /// Contains the state of each simulator and which simulator has an active session.
        /// This dictionary is used for comparison and firing events sim_start, sim_stop, session_start, session_stop.
        /// </summary>
        private Dictionary<string, Telemetry_SimState> Simulator_StateCollection = new Dictionary<string, Telemetry_SimState>();
        
        /// <summary>
        /// Thread for polling the status of all simulators and firing event whenever once has changed.
        /// </summary>
        private Thread Simulator_StatePollerThread;
        
        /// <summary>
        /// Class collecting driving stats, storing them into the log database and providing methods for
        /// searching and analyzing data.
        /// </summary>
        public TelemetryStats Stats { get; set; }

        public TelemetryNetwork Net { get; set; }

        #region Events
        /// <summary>
        /// Event fired whenever a simulator process starts. An object is passed containing the simulator instance.
        /// </summary>
        public event Signal Sim_Start;

        /// <summary>
        /// Event fired whenever a simulator process stops. An object is passed containing the simulator instance.
        /// </summary>
        public event Signal Sim_Stop;

        /// <summary>
        /// Event fired whenever a session starts. Note that the track is not yet available when this event fires;
        /// this is accessable after Track_Loaded is fired.
        /// </summary>
        public event Signal Session_Start;

        /// <summary>
        /// Event fired whenever a session stops.
        /// </summary>
        public event Signal Session_Stop;

        /// <summary>
        /// Event fired after Session_Start and the SimTelemetry framework has loaded the track data.
        /// </summary>
        public event Signal Track_Loaded;

        /// <summary>
        /// Event fired whenever a driver enters the cockpit view. Some simulators may not support this.
        /// </summary>
        public event Signal Driving_Start;

        /// <summary>
        /// Event fired whenever a driver leaves the cockpit view. Some simulators may not support this.
        /// </summary>
        public event Signal Driving_Stop;

        /// <summary>
        /// Event fired whenever the player starts a new lap.
        /// </summary>
        public event LapEvent Lap;
        #endregion

        /// <summary>
        /// This creates a new OleDB connection to the SimTelemetry access database. This function is called
        /// from the Triton Database pool allowing simultanous access from various threads to the database.
        /// </summary>
        /// <returns>New database connection</returns>
        /// <permission cref="Triton.Database">Only used by Triton.Database namespace.</permission>
        private IDbConnection GetConnection()
        {
            OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data source=Laptimes.accdb");
            return con;
        }

        /// <summary>
        /// Initialize telemetry. The main program will have to call Run() seperately.
        /// </summary>
        public Telemetry()
        {
            // Initialize the database with a maximum of 4 connections.
            DatabaseOleDbConnectionPool.MaxConnections = 4;
            DatabaseOleDbConnectionPool.Boot(GetConnection);

        }

        /// <summary>
        /// This method actually boots up the data frame-work. This method is called via the ThreadPool from Run().
        /// </summary>
        /// <param name="no">Do not pass any argument</param>
        public void Bootup(object no)
        {
            // Initialize simulator collection, data logger and stats collector.
            Net = new TelemetryNetwork();
            Sims = new Simulators();
            new TelemetryLogger(this);
            Stats = new TelemetryStats();
            new Splits();


            Simulator_StatePollerThread = new Thread(Simulator_StatePoller);
            Simulator_StatePollerThread.Start();

            // When a session start load the track.
            Session_Start += Telemetry_Session_Start;

        }

        /// <summary>
        /// Whenever a session starts this function will trigger the track parser to load the new track file.
        /// The event Track_Loaded is fired with a 500ms delay to allow the track parser to complete.
        /// The session_start function is trigger from the Simulator_StatePoller, which runs seperately anyway.
        /// </summary>
        /// <param name="sender"></param>
        private void Telemetry_Session_Start(object sender)
        {
            // Start track parser.
            // Wait 500ms because the track-parser may need some time to complete parsing the track.
            if (Net == null || !Net.IsClient)
            {
                Track = new Track.Track(Telemetry.m.Sim, Sim.Session.GameData_TrackFile);
                Thread.Sleep(500);
            }

            if (Track_Loaded != null)
                Track_Loaded(null);
        }

        /// <summary>
        /// Thread polling the status of all simulators.
        /// Will fire events Sim_Start/Stop, Session_Start/Stop.
        /// </summary>
        /// <remarks>For proper shutdown of this thread, call TritonBase.TriggerExit();</remarks>
        private void Simulator_StatePoller()
        {
            // Run whenever the TritonBase (Framework) is active.
            while (TritonBase.Active)
            {
                foreach (ISimulator sim in this.Sims.Sims)
                {
                    if (Telemetry.m.Net.IsClient
                        && sim != Telemetry.m.Sims.Network)
                        continue;

                    if (Simulator_StateCollection.ContainsKey(sim.ProcessName) == false)
                        Simulator_StateCollection.Add(sim.ProcessName, new Telemetry_SimState());

                    Telemetry_SimState state = Simulator_StateCollection[sim.ProcessName];

                    if (sim.Attached != state.Active) // Simulator events.
                    {
                        state.Active = sim.Attached;
                        if (sim.Attached)
                        {
                            Report_SimStart(sim);
                        }
                        else
                        {
                            // Also fire session if it was still active (unexpected close)
                            if (state.Session)
                                Report_SessionStop(sim);

                            Report_SimStop(sim);
                        }
                    }
                    else if (state.Active && sim.Session.Active != state.Session)// Session events.
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
                    if (sim.Session != null && sim.Drivers != null && sim.Session.Active && sim.Drivers.Player != null)
                    {
                        if (state.Active && sim.Drivers.Player.Driving != state.Driving)
                        {
                            state.Driving = sim.Drivers.Player.Driving;
                            if (state.Driving)
                            {
                                Report_DrivingStart(sim);
                            }
                            else
                            {
                                Report_DrivingStop(sim);
                            }
                        }
                        if(sim.Drivers.Player.Laps != state.Laps)
                        {
                            state.Laps = sim.Drivers.Player.Laps;
                            Report_Laps(sim, sim.Drivers.Player.GetLapTime(sim.Drivers.Player.Laps));
                        }
                    }

                    // Restore state.
                    Simulator_StateCollection[sim.ProcessName] = state;
                }

                // 50Hz
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Fire Driving Start event.
        /// </summary>
        /// <param name="sim">Simulator with drive session started</param>
        private void Report_DrivingStart(ISimulator sim)
        {
            Debug.WriteLine("DrivingStart fired");
            if (Driving_Start != null)
                Driving_Start(sim);
        }

        /// <summary>
        /// Fire Driving Stop event.
        /// </summary>
        /// <param name="sim">Simulator with drive session stopped</param>
        private void Report_DrivingStop(ISimulator sim)
        {
            Debug.WriteLine("DrivingStop fired");
            if (Driving_Stop != null)
                Driving_Stop(sim);
        }

        /// <summary>
        /// Fire Sim Start event.
        /// </summary>
        /// <param name="me">Simulator started</param>
        internal void Report_SimStart(ISimulator me)
        {
            Debug.WriteLine("SimStart fired");
            if (Sim_Start != null)
                Sim_Start(me);
        }

        /// <summary>
        /// Fire Sim Stop event.
        /// </summary>
        /// <param name="me">Simulator stopped</param>
        internal void Report_SimStop(ISimulator me)
        {
            Debug.WriteLine("SimStop fired");
            if (Sim_Stop != null)
                Sim_Stop(me);
        }

        /// <summary>
        /// Fire Session Start event.
        /// </summary>
        /// <param name="me">Simulator of which session started</param>
        internal void Report_SessionStart(ISimulator me)
        {
            Debug.WriteLine("SessionStart fired");
            if (Session_Start != null)
                Session_Start(me);
        }

        /// <summary>
        /// Fire Session Stop event.
        /// </summary>
        /// <param name="me">Simulator of which session stopped</param>
        internal void Report_SessionStop(ISimulator me)
        {
            Debug.WriteLine("SessionStop fired");
            if (Session_Stop != null)
                Session_Stop(me);
        }

        /// <summary>
        /// Fire Lap event.
        /// </summary>
        /// <param name="me">Simulator of which session stopped</param>
        internal void Report_Laps(ISimulator me, ILap lap)
        {
            Debug.WriteLine("Laps fired");
            if (Lap != null)
                Lap(me , lap);
        }

        /// <summary>
        /// Load new track. Specify location of gamedirectory and file.
        /// </summary>
        /// <param name="gamedir">Absolute path to gamedirectory.</param>
        /// <param name="track">Relative path from gamedirectory to track file.</param>
        public void Track_Load(ISimulator sim, string track)
        {
            if (Net == null || !Net.IsClient)
            {
                Track = new Track.Track(sim, track);
            }
        }

        /// <summary>
        /// Unloads track.
        /// </summary>
        public void Track_Unload()
        {
            Track = null;
        }

        /// <summary>
        /// Runs the telemetry network. The boot-up code is called via ThreadPool
        /// </summary>
        public void Run()
        {
            Peripherals = new Devices();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Bootup));
        }

        //TODO: move to track class
        /// <summary>
        /// Loads route from network.
        /// </summary>
        /// <param name="routeCollection"></param>
        public void NetworkTrack_LoadRoute(RouteCollection routeCollection)
        {
            if (Net != null && Net.IsClient)
            {
                Track = new Track.Track(routeCollection);
            }
        }
        
        //TODO: move to track class
        /// <summary>
        /// Loads track info from network.
        /// </summary>
        /// <param name="routeInfo"></param>
        public void NetworkTrack_LoadInfo(NetworkTrackInformation routeInfo)
        {
            if(Net != null && Track != null)
            {
                ((Track.Track)Track).NetworkSetInfo(routeInfo);

                if (Track_Loaded != null)
                    Track_Loaded(null);
            }
        }
    }
}