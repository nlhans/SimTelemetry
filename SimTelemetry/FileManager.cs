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
using System.Windows.Forms;
using Triton.Controls;
using Triton.Database;
using Timer = System.Windows.Forms.Timer;

namespace SimTelemetry
{
    public partial class FileManager : Form
    {
        #region Laptimes

        private VisualListDetails _vldLaptimes;
        private Timer _tList = new Timer { Interval = 1000 };
        public string Datafile { set; get; }
        public bool DatafileNew { get; set; }
        #endregion
        public FileManager(string CurrentFile)
        {
            InitializeComponent();

            if (CurrentFile == "")
                this.lbCurrent.Text = "Current file: none";
            else
                this.lbCurrent.Text = "Current file: " + CurrentFile;


            _vldLaptimes = new VisualListDetails(true);
            _vldLaptimes.Columns.Add("id", "#", 30);
            _vldLaptimes.Columns.Add("track", "Circuit", 180);
            _vldLaptimes.Columns.Add("car", "Car", 180);
            _vldLaptimes.Columns.Add("lap", "Lap", 40);
            _vldLaptimes.Columns.Add("s1", "S1",80);
            _vldLaptimes.Columns.Add("s2", "S2",80);
            _vldLaptimes.Columns.Add("s3", "S3",80);
            _vldLaptimes.Columns.Add("laptime", "Laptime", 100);
            _vldLaptimes.Columns.Add("date", "Driven at", 130);
            _vldLaptimes.Columns.Add("file", "File", 20);
            _vldLaptimes.ItemActivate += new EventHandler(_vldLaptimes_ItemActivate);
            split.Panel2.Controls.Add(_vldLaptimes);

            _tList.Tick += new EventHandler(_tList_Tick);
            _tList.Start();
        }

        void _vldLaptimes_ItemActivate(object sender, EventArgs e)
        {
            if (_vldLaptimes.SelectedItems.Count > 0)
            {
                Datafile = _vldLaptimes.SelectedItems[0].SubItems[9].Text;
                DatafileNew = true;
                this.Close();
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if(_vldLaptimes.SelectedItems.Count == 0)
            {
                btCancel_Click(sender, e);
                return;
            }
            else{
            Datafile = _vldLaptimes.SelectedItems[0].SubItems[9].Text;
            DatafileNew = true;
            this.Close();
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DatafileNew = false;
            this.Close();
        }

        private string FormatTime(double time)
        {
            if (time <= 0)
                return "-";
            if(time < 60)
            {
                double s = Math.Floor(time);
                double ms = time -s;

                return String.Format("00:{0:00}.{1:000}", s, ms*1000);
            }
            else
            {
                double min = Math.Floor(time/60);
                double s = Math.Floor(time - min*60);
                double ms = time%1.0;

                return String.Format("{2:00}:{0:00}.{1:000}", s, ms*1000,min);
            }
            return time.ToString();
        }

        void _tList_Tick(object sender, EventArgs e)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new EventHandler(_tList_Tick), new object[2] {sender, e});
                return;
            }

            // Update the list
            OleDbConnection con = DatabaseOleDbConnectionPool.GetOleDbConnection();
            using (OleDbCommand sLaps = new OleDbCommand("SELECT id, Simulator, Circuit, Series, Car, Laptime, S1, S2, S3, Driven, LapNo, FilePath FROM laptimes WHERE InvalidLapData = 0", con))
            using (OleDbDataReader rLaps = sLaps.ExecuteReader())
            {
                while(rLaps.HasRows && rLaps.Read())
                {
                    int id = rLaps.GetInt32(0);
                    bool exists = false;
                    foreach (ListViewItem lvi in _vldLaptimes.Items)
                        if (lvi.SubItems[0].Text == id.ToString())
                            exists = true;
                    if (!exists)
                    {

                        _vldLaptimes.Items.Add(new ListViewItem(new string[10] {id.ToString(), rLaps.GetString(2),
                                                                rLaps.GetString(4), 
                                                                rLaps.GetInt32(10).ToString(),
                                                                FormatTime(rLaps.GetDouble(6)),
                                                                FormatTime(rLaps.GetDouble(7)),
                                                                FormatTime(rLaps.GetDouble(8)),
                                                                FormatTime(rLaps.GetDouble(5)),
                                                                rLaps.GetDateTime(9).ToShortDateString() + " " + rLaps.GetDateTime(9).ToShortTimeString(),
                                                                rLaps.GetString(11)
                    }
                    ))
                        ;
                    }

                }
            }

            DatabaseOleDbConnectionPool.Freeup();
        }
    }
}