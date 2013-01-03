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
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Thread = System.Threading.Thread;
using Triton.Database;
using Timer = System.Timers.Timer;

namespace SimTelemetry.Data.Stats
{
    public class TelemetryStats
    {
        public double GlobalOdometer
        {
            get { return GlobalDistance + TodaysDistance; }
        }

        protected double TodaysDistance { get; set; }
        protected double GlobalDistance
        {
            get
            {
                if (Car == null || Series == null)
                    return 0;

                if (_GlobalDistance < 0)
                {
                    _GlobalDistance = 0;
                    Task t = new Task(() =>
                                          {
                                              try
                                              {
                                                  OleDbConnection con =
                                                      DatabaseOleDbConnectionPool.GetOleDbConnection();
                                                  using (
                                                      OleDbCommand ReadOdo = new OleDbCommand(
                                                "SELECT SUM(Distance) AS d FROM laptimes " +
                                                "WHERE Car = '" + Car + "' AND Series = '" + Series + "'", con))
                                                  using (OleDbDataReader ReadOdoData = ReadOdo.ExecuteReader())
                                                  {
                                                      if (ReadOdoData.HasRows)
                                                      {
                                                          ReadOdoData.Read();
                                                          TodaysDistance = 0;
                                                          _GlobalDistance = ReadOdoData.GetDouble(0);
                                                      }
                                                  }
                                              }catch(Exception )
                                              {
                                              }
                                              DatabaseOleDbConnectionPool.Freeup();
                                          });
                    t.Start();
                }
                return _GlobalDistance;
            }
        }
        // Cache for global distance driven.
        private double _GlobalDistance = -1;

        // This will log general statistics.
        // Driven distance.
        private double _dStats_Distance = 0;
        // Fuel burnt
        private double _dStats_Fuel = 0;
        // Gear changes
        private int _dStats_Gears = 0;
        // Engine revolutions
        private double _dStats_Engines = 0;
        // Time spent
        private double _dStats_Time = 0;

        public double Stats_Distance { get { return _dStats_Distance; } }
        public double Stats_Fuel { get { return _dStats_Fuel; } }
        public int Stats_Gears { get { return _dStats_Gears; } }
        public double Stats_EngineRevs { get { return _dStats_Engines; } }
        public double Stats_Time { get { return _dStats_Time; } }

        public string Stats_AnnotationQuery
        {
            get { return _Stats_AnnotationQuery; }
        }

        public bool Stats_AnnotationReset { get; set; }
        private string _Stats_AnnotationQuery;
        private string Car;
        private string Series;

        private double Last_T;
        private DateTime Last_DT;
        private int Last_Gear;
        private double Last_Fuel;

        private Timer _mStatsCounter;

        public TelemetryStats()
        {
            _mStatsCounter = new Timer { AutoReset = true, Enabled = true, Interval = 25 };
            _mStatsCounter.Elapsed += Count;

            Telemetry.m.Track_Loaded += new Triton.Signal(m_Track_Loaded);

            Stats_AnnotationReset = true;
            _Stats_AnnotationQuery = "0,0,0,0,0,NOW(),-1";
            _GlobalDistance = -1;
        }

        void m_Track_Loaded(object sender)
        {
            _GlobalDistance = -1;
        }
        public void Reset()
        {
            if (Telemetry.m.Sim.Drivers.Player == null) return;

            Car = Telemetry.m.Sim.Drivers.Player.CarModel;
            Series = Telemetry.m.Sim.Drivers.Player.CarClass;

            if (Stats_AnnotationReset)
            {
                _Stats_AnnotationQuery = Math.Round(Stats_Distance, 3) + "," + Math.Round(Stats_EngineRevs, 1) + "," +
                                         Math.Round(Stats_Fuel, 4) + "," + Stats_Gears + "," + Math.Round(Stats_Time, 3)+
                                         ",NOW(),"+Math.Round(Telemetry.m.Sim.Session.TimeClock,3);
                Stats_AnnotationReset = false;
            }
            _dStats_Engines = 0;
            _dStats_Time = 0;
            _dStats_Gears = 0;
            _dStats_Distance = 0;
            _dStats_Fuel = 0;

            if (Telemetry.m.Sim.Modules.Time_Available)
                Last_T = Telemetry.m.Sim.Session.Time;
            else
                Last_DT = DateTime.Now;

            _mStatsCounter.Start();

        }

        private void Count(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Telemetry.m.Active_Session && !Telemetry.m.Net.IsClient)
                {
                    if (Telemetry.m.Sim.Player == null)
                        return;

                    double dt_ms = 0;
                    if (Telemetry.m.Sim.Modules.Time_Available)
                    {
                        dt_ms = Telemetry.m.Sim.Session.Time - Last_T;
                        Last_T = Telemetry.m.Sim.Session.Time;
                    }
                    else
                    {
                        TimeSpan dt = DateTime.Now.Subtract(Last_DT);
                        dt_ms = dt.TotalMilliseconds/1000.0;
                        Last_DT = DateTime.Now;
                    }
                    if (dt_ms > 0 && !Telemetry.m.Sim.Drivers.Player.Control_AI_Aid &&
                        Telemetry.m.Sim.Drivers.Player.Driving) // time is ticking and player is driving
                    {

                        if (_dStats_Fuel < 0) _dStats_Fuel = 0;
                        if (_dStats_Time < 0) _dStats_Time = 0;
                        if (_dStats_Gears < 0) _dStats_Gears = 0;
                        if (_dStats_Engines < 0) _dStats_Engines = 0;
                        if (_dStats_Distance < 0) _dStats_Distance = 0;

                        if (Telemetry.m.Sim.Player.Gear != Last_Gear)
                            _dStats_Gears++;

                        double dFuel = (Last_Fuel - Telemetry.m.Sim.Player.Fuel);
                        if (Telemetry.m.Sim.Player.Fuel < Last_Fuel && dFuel < 0.5) // do not count refueling
                            _dStats_Fuel += (Last_Fuel - Telemetry.m.Sim.Player.Fuel);

                        _dStats_Engines += dt_ms*Telemetry.m.Sim.Player.Engine_RPM/2.0/Math.PI; // rad/s to rps
                        _dStats_Distance += dt_ms*Math.Abs(Telemetry.m.Sim.Player.Speed)/1000.0; // km's
                        TodaysDistance += dt_ms*Math.Abs(Telemetry.m.Sim.Player.Speed)/1000.0;
                            // quick approximation of odometer

                        _dStats_Time += dt_ms;

                        Last_Gear = Telemetry.m.Sim.Player.Gear;
                        Last_Fuel = Telemetry.m.Sim.Player.Fuel;
                    }

                }
            }catch(Exception ex)
            {
                
            }
        }
    }
}