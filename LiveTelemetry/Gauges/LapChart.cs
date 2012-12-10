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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Objects;

namespace LiveTelemetry
{
    public partial class LapChart : UserControl
    {
        private Dictionary<string, int> DisplayTimers = new Dictionary<string, int>();
        private Dictionary<string, int> LastPitLap = new Dictionary<string, int>();
        public LapChart()
        {
            InitializeComponent();

            this.Size = new Size(460, 500);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

        }

        private int sortDriver(IDriverGeneral d1, IDriverGeneral d2)
        {
            if (d1 == null || d2 == null) return 0;
            if (d1.Position == d2.Position) return 0;
            if (d1.Position > 120 || d1.Position == 0) return 1;
            if (d2.Position > 120 || d2.Position == 0) return -1;
            if (d1.Position > d2.Position) return 1;
            else return -1;
        }

        private string PrintLapTime(float time, bool sector)
        {
            if(time < 60)
            {
                if (sector) return time.ToString("00.000");
                else
                {
                    return "0:" + time.ToString("00.000");
                }
            }
            else
            {
                int minutes = Convert.ToInt32(Math.Floor(Convert.ToDouble(time/60f)));

                return minutes + ":" + (time%60f).ToString("00.000");
            }
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.Size = new Size(460, this.Size.Height);
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (!Telemetry.m.Active_Session) return;

            // Draw header
            System.Drawing.Font f = new Font("Arial", 8, FontStyle.Regular);

            try
            {
                switch (Telemetry.m.Sim.Session.Type.Type)
                {
                    default:
                        g.DrawString("P", f, Brushes.DarkGray, 10f, 10f);
                        g.DrawString("Name", f, Brushes.DarkGray, 38f, 10f);
                        g.DrawString("Lap", f, Brushes.DarkGray, 140f, 10f);
                        g.DrawString("Interval", f, Brushes.DarkGray, 195f, 10f);
                        g.DrawString("S1", f, Brushes.DarkGray, 245f, 10f);
                        g.DrawString("S2", f, Brushes.DarkGray, 295f, 10f);
                        g.DrawString("S3", f, Brushes.DarkGray, 345f, 10f);
                        g.DrawString("Laps", f, Brushes.DarkGray, 395f, 10f);

                        break;
                    case SessionType.RACE:
                        g.DrawString("P", f, Brushes.DarkGray, 10f, 10f);
                        g.DrawString("Name", f, Brushes.DarkGray, 38f, 10f);
                        g.DrawString("Gap", f, Brushes.DarkGray, 140f, 10f);
                        g.DrawString("Interval", f, Brushes.DarkGray, 165f, 10f);
                        g.DrawString("Lap", f, Brushes.DarkGray, 222f, 10f);
                        g.DrawString("S1", f, Brushes.DarkGray, 245f, 10f);
                        g.DrawString("S2", f, Brushes.DarkGray, 295f, 10f);
                        g.DrawString("S3", f, Brushes.DarkGray, 345f, 10f);
                        g.DrawString("Pit", f, Brushes.DarkGray, 395f, 10f);

                        break;
                }
                int ind = 1;
                float LineHeight = 15f;

                List<IDriverGeneral> drivers = Telemetry.m.Sim.Drivers.AllDrivers;
                drivers.Sort(sortDriver);

                double lapTimeBest = 10000;
                int driverIndex = 0;

                float BestSector1 = 1000f;
                float BestSector2 = 1000f;
                float BestSector3 = 1000f;

                float MyBestSector1 = 1000f;
                float MyBestSector2 = 1000f;
                float MyBestSector3 = 1000f;

                int BestSector1Position = 0;
                int BestSector2Position = 0;
                int BestSector3Position = 0;

                float FastestLap = 1000f;
                string FastestDriver = "";
                int DriverCount = 0;
                // get all time best sector times
                List<ILap> mylaps = Telemetry.m.Sim.Drivers.Player.GetLapTimes();
                foreach (ILap l in mylaps)
                {
                    if (l.Sector1 >0.1 && l.Sector2 >0.1 && l.Sector3 >0.1)
                    {
                        MyBestSector1 = Math.Min(MyBestSector1, l.Sector1);
                        MyBestSector2 = Math.Min(MyBestSector2, l.Sector2);
                        MyBestSector3 = Math.Min(MyBestSector3, l.Sector3);
                    }

                }

                foreach (IDriverGeneral driver in drivers)
                {
                    
                    if (driver.Position != 0 && driver.Position <= 120 && Math.Abs(driver.CoordinateX) >= 0.1)
                    {
                        driverIndex++;
                        //if (driver.Position >= 120 || driver.Position == 0) continue;
                        //if (driver.Name.Trim() == "") continue;

                        // fastest lap
                        DriverCount++;
                        ILap fastlap = driver.GetBestLap();


                        if (fastlap != null && fastlap.LapTime < FastestLap && fastlap.LapTime > 0)
                        {
                            FastestLap = fastlap.LapTime;
                            FastestDriver = driver.Name;
                        }
                        if (driver.LapTime_Best_Sector1 > 0)
                        {
                            if (BestSector1 > driver.LapTime_Best_Sector1)
                            {
                                BestSector1 = driver.LapTime_Best_Sector1;
                                BestSector1Position = driver.Position;
                            }
                        }
                        if (driver.LapTime_Best_Sector2 > 0)
                        {
                            if (BestSector2 > driver.LapTime_Best_Sector2)
                            {
                                BestSector2 = driver.LapTime_Best_Sector2;
                                BestSector2Position = driver.Position;
                            }
                        }
                        //BestSector2 = Math.Min(BestSector2, driver.LapTime_Best_Sector2);
                        if (driver.LapTime_Best_Sector3 > 0)
                        {
                            if (BestSector3 > driver.LapTime_Best_Sector3)
                            {
                                BestSector3 = driver.LapTime_Best_Sector3;
                                BestSector3Position = driver.Position;
                            }
                            //BestSector3 = Math.Min(BestSector3, driver.LapTime_Best_Sector3);
                        }
                    }

                }
                driverIndex = 0;

                if (DriverCount < 16)
                    LineHeight = 20f;
                else
                    LineHeight = Math.Max(10, Convert.ToSingle((this.Size.Height - 20)/(4.0 + DriverCount)));
                

                // Go through all drivers
                IDriverGeneral Lastdriver = drivers[0];
                double Previous_SplitLeader = 0;
                foreach (IDriverGeneral driver in drivers)
                {
                    driverIndex++;
                    Lastdriver = driver;
                    if (ind * LineHeight > this.Size.Height - 20 - 3 * 20)
                        break;

                    if (Math.Abs(driver.CoordinateX) < 0.1) continue;
                    //if (driver.Position >= 120 || driver.Position == 0 || driver.__LapsData.ToInt32() == 0) continue;
                    //if (driver.Name.Trim() == "")
                    //    continue;
                    Brush OntrackBrush = ((!driver.Pits && driver.Speed > 5) ? Brushes.White : Brushes.Red);
                    g.DrawString(driver.Position.ToString(), f, Brushes.White, 10f, 10f + ind*LineHeight);
                    string[] name = driver.Name.ToUpper().Split(" ".ToCharArray());
                    if (name.Length == 1)
                        g.DrawString(name[0], f, OntrackBrush, 38f, 10f + ind*LineHeight);
                    else if (name.Length > 1)
                        g.DrawString(name[0].Substring(0, 1) + ". " + name[name.Length - 1], f, OntrackBrush, 38f,
                                     10f + ind*LineHeight);

                    if (!LastPitLap.ContainsKey(driver.Name))
                        LastPitLap.Add(driver.Name, -1);
                    if(driver.Pits)
                    {
                        LastPitLap[driver.Name] = driver.Laps;
                    }

                    if (Telemetry.m.Sim.Session.Type.Type == SessionType.RACE)
                    {
                        if (driver.LapTime_Last < lapTimeBest && driver.LapTime_Last != -1f)
                            lapTimeBest = driver.LapTime_Last;
                        if (driver.LapTime_Last != -1f)
                            g.DrawString(PrintLapTime(driver.LapTime_Last, false), f, Brushes.White, 140f,
                                         10f + ind*LineHeight);

                        if (driver.Position == 1)
                            g.DrawString("LAP " + drivers[0].Laps, f, Brushes.Yellow, 190f, 10f + ind * LineHeight);
                        else
                        {
                            double split_leader = driver.GetSplitTime(drivers[0]);
                            if (split_leader >= 10000)
                            {
                                int laps = Convert.ToInt32(split_leader/10000);
                                if (laps < 100)
                                    g.DrawString("+" + laps + "L", f, Brushes.Yellow, 190f, 10f + ind*LineHeight);
                                else
                                    g.DrawString("???", f, Brushes.Yellow, 190f, 10f + ind*LineHeight);
                            }
                            else
                                g.DrawString(Math.Round(split_leader, 1).ToString(), f, Brushes.White, 190f, 10f + ind*LineHeight);

                            double split_next = split_leader - Previous_SplitLeader;
                            Previous_SplitLeader = split_leader;
                            if (split_next < 1000 && split_next > 0)
                                g.DrawString(Math.Round(split_next, 1).ToString(), f,Brushes.White, 220f, 10f + ind*LineHeight);
                            else if(split_next > 0)
                            {
                                int laps = Convert.ToInt32(split_next/10000);
                                g.DrawString("+" + laps + "L", f, Brushes.Yellow, 220f, 10f + ind*LineHeight);
                            }

                        }


                        if (driver.Pits && driver.Sector_1_Best > 0)
                        {
                            Brush Sector1Brush = Brushes.Yellow;
                            Brush Sector2Brush = Brushes.Yellow;
                            Brush Sector3Brush = Brushes.Yellow;
                            if (driver.Sector_1_Best <= BestSector1) Sector1Brush = Brushes.Magenta;
                            if (driver.Sector_2_Best <= BestSector2) Sector2Brush = Brushes.Magenta;
                            if (driver.Sector_3_Best <= BestSector3) Sector3Brush = Brushes.Magenta;
                            g.DrawString(PrintLapTime(driver.Sector_1_Best, true), f, Sector1Brush, 245f,
                                         10f + ind*LineHeight);
                            g.DrawString(PrintLapTime(driver.Sector_2_Best, true), f, Sector2Brush, 295f,
                                         10f + ind*LineHeight);
                            g.DrawString(PrintLapTime(driver.Sector_3_Best, true), f, Sector3Brush, 345f,
                                         10f + ind*LineHeight);
                        }
                        else if (driver.Sector_1_Last > 0)
                        {
                            bool DisplayAllSectors = false;

                            if (!DisplayTimers.ContainsKey(driver.Name))
                                DisplayTimers.Add(driver.Name, -1);
                            if (driver.TrackPosition == TrackPosition.SECTOR1)
                                DisplayTimers[driver.Name]--;
                            if (driver.TrackPosition == TrackPosition.SECTOR2)
                                DisplayTimers[driver.Name] = 10;
                            if (DisplayTimers[driver.Name] > 0 && driver.TrackPosition == TrackPosition.SECTOR1)
                                DisplayAllSectors = true;
                            //if (data[driverIndex].CurrentSector >= 2 || data[driverIndex].CurrentSector == 0)
                            if (driver.TrackPosition == TrackPosition.SECTOR2 ||
                                driver.TrackPosition == TrackPosition.SECTOR3 || DisplayAllSectors)
                            {
                                Brush Sector1Brush = Brushes.Yellow;
                                if (driver.TrackPosition == TrackPosition.SECTOR2) Sector1Brush = Brushes.White;
                                if (driver.Sector_1_Last == driver.LapTime_Best_Sector1) Sector1Brush = Brushes.Green;
                                if (driver.Sector_1_Last <= BestSector1) Sector1Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(driver.Sector_1_Last, true), f, Sector1Brush, 245f,
                                             10f + ind*LineHeight);
                            }
                            if (driver.TrackPosition == TrackPosition.SECTOR3 || DisplayAllSectors)
                                //if (data[driverIndex].CurrentSector == 3 || data[driverIndex].CurrentSector == 0)
                            {

                                Brush Sector2Brush = Brushes.Yellow;
                                if (driver.TrackPosition == TrackPosition.SECTOR3) Sector2Brush = Brushes.White;
                                if (driver.Sector_2_Last == driver.LapTime_Best_Sector2) Sector2Brush = Brushes.Green;
                                if (driver.Sector_2_Last <= BestSector2) Sector2Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(driver.Sector_2_Last, true), f, Sector2Brush, 295f,
                                             10f + ind*LineHeight);
                            }
                            if (DisplayAllSectors && driver.Sector_3_Last > 0)
                                //if (data[driverIndex].CurrentSector == 0)
                            {
                                Brush Sector3Brush = Brushes.Yellow;
                                if (driver.TrackPosition == TrackPosition.SECTOR1) Sector3Brush = Brushes.White;
                                if (driver.Sector_3_Last == driver.LapTime_Best_Sector3) Sector3Brush = Brushes.Green;
                                if (driver.Sector_3_Last <= BestSector3) Sector3Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(driver.Sector_3_Last, true), f, Sector3Brush, 345f,
                                             10f + ind*LineHeight);
                            }
                            if (!driver.Pits && driver.Speed < 5)
                            {
                                if (driver.TrackPosition == TrackPosition.SECTOR1)
                                    g.DrawString("[STOP]", f, Brushes.Red, 245f, 10f + ind * LineHeight);
                                if (driver.TrackPosition == TrackPosition.SECTOR2)
                                    g.DrawString("[STOP]", f, Brushes.Red, 295f, 10f + ind * LineHeight);
                                if (driver.TrackPosition == TrackPosition.SECTOR3)
                                    g.DrawString("[STOP]", f, Brushes.Red, 345f, 10f + ind * LineHeight);

                            }
                        }
                        else if (driver.Laps > 0 || (driver.Laps == 0 && driver.Pits == false))
                            g.DrawString("OUT", f, Brushes.Red, 265f, 10f + ind*LineHeight);

                        g.DrawString(driver.PitStopRuns.ToString(), f, Brushes.White, 395f, 10f + ind*LineHeight);
                    }
                    else
                    {

/*if (driver.Retired && driver.Speed < 5)
{
    g.DrawString("[RETIRED]", f, Brushes.Red, 265f, 10f + ind * LineHeight);

}
else */
                        if (driver.Pits && driver.Sector_1_Best > 0)
                        {

                            if (driver.LapTime_Best < lapTimeBest && driver.LapTime_Best != -1f)
                                lapTimeBest = driver.LapTime_Best;
                            if (driver.LapTime_Best != -1f)
                                g.DrawString(PrintLapTime(driver.LapTime_Best, false), f, Brushes.Yellow, 140f,
                                             10f + ind * LineHeight);
                            if (driver.LapTime_Best != lapTimeBest && driver.LapTime_Best != -1f)
                            {
                                double diff = driver.LapTime_Best - lapTimeBest;
                                g.DrawString(diff.ToString("0.000"), f, Brushes.Yellow, 195f, 10f + ind * LineHeight);
                            }

                            Brush Sector1Brush = Brushes.Yellow;
                            Brush Sector2Brush = Brushes.Yellow;
                            Brush Sector3Brush = Brushes.Yellow;
                            if (driver.Sector_1_Best <= BestSector1) Sector1Brush = Brushes.Magenta;
                            if (driver.Sector_2_Best <= BestSector2) Sector2Brush = Brushes.Magenta;
                            if (driver.Sector_3_Best <= BestSector3) Sector3Brush = Brushes.Magenta;
                            g.DrawString(PrintLapTime(driver.Sector_1_Best, true), f, Sector1Brush, 245f,
                                         10f + ind*LineHeight);
                            g.DrawString(PrintLapTime(driver.Sector_2_Best, true), f, Sector2Brush, 295f,
                                         10f + ind*LineHeight);
                            g.DrawString(PrintLapTime(driver.Sector_3_Best, true), f, Sector3Brush, 345f,
                                         10f + ind*LineHeight);
                        }
                        else 
                        {
                            bool DisplayAllSectors = false;
                            if (!DisplayTimers.ContainsKey(driver.Name))
                                DisplayTimers.Add(driver.Name, -1);
                            if (driver.TrackPosition == TrackPosition.SECTOR1)
                                DisplayTimers[driver.Name]--;
                            if (driver.TrackPosition == TrackPosition.SECTOR2)
                                DisplayTimers[driver.Name] = 10;
                            if (DisplayTimers[driver.Name] > 0 && driver.TrackPosition == TrackPosition.SECTOR1)
                                DisplayAllSectors = true;
                            //if (data[driverIndex].CurrentSector >= 2 || data[driverIndex].CurrentSector == 0)

                            if ((LastPitLap[driver.Name] == driver.Laps || driver.Sector_1_Last < 0) && !driver.Pits)
                                    g.DrawString("OUT", f, Brushes.Red, 245f, 10f + ind*LineHeight);
                                else
                                    if (driver.TrackPosition == TrackPosition.SECTOR2 ||
                                        driver.TrackPosition == TrackPosition.SECTOR3 || DisplayAllSectors)
                                {
                                    Brush Sector1Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPosition.SECTOR2) Sector1Brush = Brushes.White;
                                    if (driver.Sector_1_Last == driver.LapTime_Best_Sector1)
                                        Sector1Brush = Brushes.Green;
                                    if (driver.Sector_1_Last <= BestSector1) Sector1Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(driver.Sector_1_Last, true), f, Sector1Brush, 245f,
                                                 10f + ind*LineHeight);
                                }
                            
                            if (driver.TrackPosition == TrackPosition.SECTOR3 || DisplayAllSectors)
                                //if (data[driverIndex].CurrentSector == 3 || data[driverIndex].CurrentSector == 0)
                            {

                                Brush Sector2Brush = Brushes.Yellow;
                                if (driver.TrackPosition == TrackPosition.SECTOR3) Sector2Brush = Brushes.White;
                                if (driver.Sector_2_Last == driver.LapTime_Best_Sector2) Sector2Brush = Brushes.Green;
                                if (driver.Sector_2_Last <= BestSector2) Sector2Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(driver.Sector_2_Last, true), f, Sector2Brush, 295f,
                                             10f + ind*LineHeight);
                            }
                            if (DisplayAllSectors && driver.Sector_3_Last > 0)
                                //if (data[driverIndex].CurrentSector == 0)
                            {
                                Brush Sector3Brush = Brushes.Yellow;
                                if (driver.TrackPosition == TrackPosition.SECTOR1) Sector3Brush = Brushes.White;
                                if (driver.Sector_3_Last == driver.LapTime_Best_Sector3) Sector3Brush = Brushes.Green;
                                if (driver.Sector_3_Last <= BestSector3) Sector3Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(driver.Sector_3_Last, true), f, Sector3Brush, 345f,
                                             10f + ind*LineHeight);



                                if (driver.LapTime_Last < lapTimeBest && driver.LapTime_Last != -1f)
                                    lapTimeBest = driver.LapTime_Best;
                                if (driver.LapTime_Last != -1f)
                                    g.DrawString(PrintLapTime(driver.LapTime_Last, false), f, Brushes.White, 140f,
                                                 10f + ind * LineHeight);
                                if (driver.LapTime_Last != lapTimeBest && driver.LapTime_Last != -1f)
                                {
                                    double diff = driver.LapTime_Last - lapTimeBest;
                                    g.DrawString(diff.ToString("0.000"), f, Brushes.White, 195f, 10f + ind * LineHeight);
                                }
                            }
                            else
                            {


                                if (driver.LapTime_Best < lapTimeBest && driver.LapTime_Best != -1f)
                                    lapTimeBest = driver.LapTime_Best;
                                if (driver.LapTime_Best != -1f)
                                    g.DrawString(PrintLapTime(driver.LapTime_Best, false), f, Brushes.Yellow, 140f,
                                                 10f + ind * LineHeight);
                                if (driver.LapTime_Best != lapTimeBest && driver.LapTime_Best != -1f)
                                {
                                    double diff = driver.LapTime_Best - lapTimeBest;
                                    g.DrawString(diff.ToString("0.000"), f, Brushes.Yellow, 195f, 10f + ind * LineHeight);
                                }
                            }
                        if (!driver.Pits && driver.Speed < 5)
                        {
                            if (driver.TrackPosition == TrackPosition.SECTOR1)
                                g.DrawString("[STOP]", f, Brushes.Red, 245f, 10f + ind * LineHeight);
                            if (driver.TrackPosition == TrackPosition.SECTOR2)
                                g.DrawString("[STOP]", f, Brushes.Red, 295f, 10f + ind * LineHeight);
                            if (driver.TrackPosition == TrackPosition.SECTOR3)
                                g.DrawString("[STOP]", f, Brushes.Red, 345f, 10f + ind * LineHeight);

                        }
                        }

                        g.DrawString(driver.Laps.ToString(), f, Brushes.White, 395f, 10f + ind*LineHeight);
                    }

                    ind++;

                }

                // Place fastest lap & combined lap
                if (BestSector3Position != 0)
                {
                    g.DrawString("Fastest lap: ", f, Brushes.DarkGray, 10f, 10f + ind*LineHeight);
                    g.DrawString(FastestDriver, f, Brushes.Yellow, 195f, 10f + ind*LineHeight);
                    g.DrawString(PrintLapTime(FastestLap, false), f, Brushes.Magenta, 140f, 10f + ind*LineHeight);



                    g.DrawString("P" + BestSector1Position, f, Brushes.Yellow, 275f, 10f + ind*LineHeight);
                    g.DrawString("P" + BestSector2Position, f, Brushes.Yellow, 325f, 10f + ind*LineHeight);
                    g.DrawString("P" + BestSector3Position, f, Brushes.Yellow, 375f, 10f + ind*LineHeight);

                }
                if (BestSector3 != 1000)
                {
                    ind++;
                    g.DrawString("Combined lap: ", f, Brushes.DarkGray, 10f, 10f + ind * LineHeight);
                    g.DrawString(PrintLapTime(BestSector1 + BestSector2 + BestSector3, false), f, Brushes.Magenta, 140f,
                                 10f + ind * LineHeight);

                    g.DrawString(PrintLapTime(BestSector1, true), f, Brushes.Magenta, 245f, 10f + ind * LineHeight);
                    g.DrawString(PrintLapTime(BestSector2, true), f, Brushes.Magenta, 295f, 10f + ind * LineHeight);
                    g.DrawString(PrintLapTime(BestSector3, true), f, Brushes.Magenta, 345f, 10f + ind * LineHeight);
                    ind++;
                    g.DrawString("Personal Combined lap: ", f, Brushes.DarkGray, 10f, 10f + ind * LineHeight);
                    g.DrawString(PrintLapTime(MyBestSector1 + MyBestSector2 + MyBestSector3, false), f, Brushes.Magenta, 140f,
                                 10f + ind * LineHeight);

                    g.DrawString(PrintLapTime(MyBestSector1, true), f, Brushes.Magenta, 245f, 10f + ind * LineHeight);
                    g.DrawString(PrintLapTime(MyBestSector2, true), f, Brushes.Magenta, 295f, 10f + ind * LineHeight);
                    g.DrawString(PrintLapTime(MyBestSector3, true), f, Brushes.Magenta, 345f, 10f + ind * LineHeight);
                }
            }
            catch (Exception ex)
            {
                /*
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                g.DrawString("ERROR", f, Brushes.Red, 10f, 10f);
                g.DrawString(ex.Message, f, Brushes.Yellow, 10f, 50f);
                string[] lines = ex.StackTrace.Split("\n".ToCharArray());

                int i = 1;
                foreach (string line in lines)
                {
                    i++;
                    g.DrawString(line, f, Brushes.White, 10f, 10f + i * 30f);
                }*/
            }
            base.OnPaint(e);
        }

        private SessionType LastSession = SessionType.TEST_DAY;

    }

}