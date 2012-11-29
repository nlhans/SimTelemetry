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
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimTelemetry.Data.Logger;
using SimTelemetry.Objects;
using Triton.Database;
using Triton.Maths;
using Timer = System.Timers.Timer;

namespace SimTelemetry.Data
{
    public class Splits
    {
        private static Filter _Split = new Filter(5);
        public static double Split { get { return _Split.Average; }}
        private bool PreviousRecords = false;
        private TelemetryLogReader _mReferenceLapReader;
        private Dictionary<double, double> MetersToTime_Table = new Dictionary<double, double>();
        private Timer UpdateSplitTime;
        private Timer _mUpdateBestLap;
        private double Time_Start = 0;

        public Splits()
        {
            Telemetry.m.Driving_Start += m_Driving_Start;
            Telemetry.m.Driving_Stop += m_Driving_Stop;
            Telemetry.m.Lap += m_Lap;
            Telemetry.m.Track_Loaded += m_Track_Loaded;

            UpdateSplitTime = new Timer {Interval = 50};
            UpdateSplitTime.Elapsed += (s, e) =>
                                           {
                                               if (Telemetry.m.Sim == null || Telemetry.m.Sim.Drivers == null || Telemetry.m.Sim.Drivers.Player == null)
                                                   return;

                                               double MyMeters = Telemetry.m.Sim.Drivers.Player.MetersDriven;
                                               double MyDt = Telemetry.m.Sim.Session.Time - Time_Start;
                                               double OldTime = GetTimeFromTable(MyMeters);
                                               if (OldTime > 0)
                                               {
                                                   _Split.Add(MyDt - OldTime/1000);
                                               }


                                           };

            // Trigger-once timer
            _mUpdateBestLap = new Timer {Interval = 100};
            _mUpdateBestLap.AutoReset = false;
            _mUpdateBestLap.Elapsed += (s, e) =>
                                           {
                                               bool RemoveLapTelemetryPath = false;
                                               List<int> LapsToInvalidate = new List<int>();
                                               if (Telemetry.m.Sim == null) return;
                                               if (Telemetry.m.Track == null) return;
                                               if (Telemetry.m.Track.Name == null) return;
                                               if (Telemetry.m.Net.IsClient) return;

                                               OleDbConnection con =
                                                   DatabaseOleDbConnectionPool.GetOleDbConnection();
                                               using (
                                                   OleDbCommand times =
                                                       new OleDbCommand("SELECT Filepath, `ID` FROM Laptimes WHERE " +
                                                                        "Car = '" +
                                                                        Telemetry.m.Sim.Drivers.Player.CarModel.
                                                                            Replace("'", "\\'") + "' AND " +
                                                                        "Series = '" +
                                                                        Telemetry.m.Sim.Drivers.Player.CarClass.
                                                                            Replace("'", "\\'") + "' AND " +
                                                                        "Circuit = '" +
                                                                        Telemetry.m.Track.Name.Replace("'", "\\'") +
                                                                        "' AND " +
                                                                        "Simulator = '" + Telemetry.m.Sim.Name + "'" +
                                                                        "AND Laptime > 1 " +
                                                                        "AND `InvalidLapData` = 0 " +
                                                                        "ORDER BY Laptime ASC",
                                                                        con))
                                               using (OleDbDataReader times_Reader = times.ExecuteReader())
                                               {
                                                   if (times_Reader.HasRows)
                                                   {
                                                       while (PreviousRecords == false && times_Reader.Read())
                                                       {
                                                           int lapID = 0;
                                                           try
                                                           {
                                                               lapID = times_Reader.GetInt32(1);
                                                               _mReferenceLapReader =
                                                                   new TelemetryLogReader(times_Reader.GetString(0));
                                                               _mReferenceLapReader.ReadPolling();
                                                               PreviousRecords = true;
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               LapsToInvalidate.Add(lapID);
                                                               RemoveLapTelemetryPath = true;
                                                               _mReferenceLapReader = null;
                                                           }

                                                       }
                                                   }
                                               }
                                               if (RemoveLapTelemetryPath)
                                               {
                                                   while (LapsToInvalidate.Count > 0)
                                                   {
                                                       int lapID = LapsToInvalidate[0];
                                                       OleDbCommand upd =
                                                           new OleDbCommand(
                                                               "UPDATE Laptimes SET InvalidLapData = 1 WHERE `ID` = " + lapID, con);
                                                       upd.ExecuteNonQuery();
                                                       LapsToInvalidate.RemoveAt(0);
                                                   }
                                               }
                                               DatabaseOleDbConnectionPool.Freeup();
                                               Load_ExtractMetersTable();
                                           };
        }

        private void m_Lap(ISimulator sim, ILap lap)
        {
            if (Telemetry.m.Sim != null)
            {
                Time_Start = Telemetry.m.Sim.Session.Time; // TODO: add local timing if not supported.
                // Get the last lap from the game.
                PreviousRecords = false;
                _mUpdateBestLap.Start();
            }
        }

        private double GetTimeFromTable(double meters)
        {
            double previous_m = -1;
            lock (MetersToTime_Table)
            {
                foreach (double m in MetersToTime_Table.Keys)
                {
                    if (previous_m < meters && meters <= m)
                    {
                        return MetersToTime_Table[m];
                    }
                    else
                    {
                        previous_m = m;
                    }
                }
            }
            return -1;
        }

        private void Load_ExtractMetersTable()
        {
            if (PreviousRecords)
            {
                lock (MetersToTime_Table)
                {
                    MetersToTime_Table.Clear();
                    lock (_mReferenceLapReader)
                    {
                        foreach (KeyValuePair<double, TelemetrySample> kvp in _mReferenceLapReader.Samples)
                        {
                            double meter = _mReferenceLapReader.GetDouble(kvp.Value, "Driver.MetersDriven");
                            double time = kvp.Key;

                            if (MetersToTime_Table.ContainsKey(meter) == false)
                                MetersToTime_Table.Add(meter, time);

                        }
                    }
                }
            }
        }

        private void m_Track_Loaded(object sender)
        {
            PreviousRecords = false;
            // Get the best lap from the telemetry archive.

            _mUpdateBestLap.Start();
        }

        private void m_Driving_Start(object sender)
        {
            UpdateSplitTime.Start();
        }

        private void m_Driving_Stop(object sender)
        {
            UpdateSplitTime.Stop();
        }
    }

}
