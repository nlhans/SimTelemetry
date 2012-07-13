using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Triton.Controls;
using Triton.Database;
using Timer = System.Windows.Forms.Timer;

namespace SimTelemetry
{
    public partial class FileManager : Form
    {
        private Timer _tStatus = new Timer { Interval = 10 };

        #region Laptimes

        private VisualListDetails _vldLaptimes;
        private Timer _tList = new Timer { Interval = 20 };
        #endregion
        #region Clean up
        private Timer _tCleanup = new Timer { Interval = 120000 };
        private bool _bCleanup = false;

        private int _iDirs = 0;
        private int _iDirsDone = 0;

        #endregion

        private IDbConnection GetConnection()
        {
            OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data source=Laptimes.accdb");
            return con;
        }

        public FileManager()
        {
            InitializeComponent();

            DatabaseOleDbConnectionPool.MaxConnections = 4;
            DatabaseOleDbConnectionPool.Boot(GetConnection);

            _vldLaptimes = new VisualListDetails(true);
            _vldLaptimes.Columns.Add("id", "#", 30);
            _vldLaptimes.Columns.Add("track", "Circuit", 180);
            _vldLaptimes.Columns.Add("car", "Car", 180);
            _vldLaptimes.Columns.Add("lap", "Lap", 40);
            _vldLaptimes.Columns.Add("s1", "S1",80);
            _vldLaptimes.Columns.Add("s2", "S2",80);
            _vldLaptimes.Columns.Add("s3", "S3",80);
            _vldLaptimes.Columns.Add("laptime", "Laptime",100);
            _vldLaptimes.Columns.Add("date", "Driven at",130);
            Controls.Add(_vldLaptimes);

            _tCleanup.Tick += new EventHandler(_tCleanup_Tick);
            _tStatus.Tick += new EventHandler(_tStatus_Tick);
            _tList.Tick += new EventHandler(_tList_Tick);
            _tStatus.Start();
            _tCleanup.Start();
            _tList.Start();
            ThreadPool.QueueUserWorkItem(new WaitCallback((n) => _tCleanup_Tick(null, new EventArgs())));
        }

        private string FormatTime(double time)
        {
            if(time < 60)
            {
                double ms = time%1.0;
                double s = time - ms;

                return String.Format("{0:00}.{1:000}", s, ms);
            }
            else
            {
                double min = Math.Floor(time/60);
                double s = Math.Floor(time - min*60);
                double ms = time%1.0;

                return String.Format("{2:00}:{0:00}.{1:000}", s, ms,min);
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
            using(OleDbCommand sLaps = new OleDbCommand("SELECT id, Simulator, Circuit, Series, Car, Laptime, S1, S2, S3, Driven, LapNo, FilePath FROM laptimes",con))
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

                        _vldLaptimes.Items.Add(new ListViewItem(new string[9] {id.ToString(), rLaps.GetString(2),
                                                                rLaps.GetString(4), 
                                                                rLaps.GetInt32(10).ToString(),
                                                                FormatTime(rLaps.GetDouble(6)),
                                                                FormatTime(rLaps.GetDouble(7)),
                                                                FormatTime(rLaps.GetDouble(8)),
                                                                FormatTime(rLaps.GetDouble(5)),
                                                                rLaps.GetDateTime(9).ToShortDateString() + " " + rLaps.GetDateTime(9).ToShortTimeString()
                    }
                    ))
                        ;
                    }

                }
            }

            DatabaseOleDbConnectionPool.Freeup();
        }

        void _tStatus_Tick(object sender, EventArgs e)
        {
            if (_bCleanup)
            {
                if (_iDirs > 0 && _iDirs > _iDirsDone)
                    statusProgress.Value = _iDirsDone * 1000 / _iDirs;
                else
                    statusProgress.Value = 0;
                txtStatus.Text = "Cleaning up logs..";
                statusProgress.Size = new Size(300, 10);
            }
            else
            {
                statusProgress.Value = 0;
                statusProgress.Size = new Size(1,10);
                txtStatus.Text = "Idle";
            }
        }

        void _tCleanup_Tick(object sender, EventArgs e)
        {
            _bCleanup = true;

            Task t = new Task(() =>
            {
                System.Threading.Thread.Sleep(500);
                string[] sims = Directory.GetDirectories("Logs/");
                _iDirs = 0;
                _iDirsDone = 0;
                foreach (string sim in sims)
                {
                    _iDirs += Directory.GetDirectories(sim + "/").Length;
                }
                // remove empty directories
                foreach (string sim in sims)
                {
                    foreach (string directory in Directory.GetDirectories(sim + "/"))
                    {
                        string[] files = Directory.GetFiles(directory + "/");
                        if (files.Length == 0)
                            Directory.Delete(directory + "/");

                        if (files.Length <= 2)
                        {
                            bool mark_deletion = true;
                            foreach (string file in files)
                            {
                                if (file.EndsWith(".gz"))
                                {
                                    mark_deletion = false;
                                    break;
                                }

                                DateTime created =
                                    new FileInfo(file).CreationTime;
                                if (file.EndsWith(".dat") &&
                                    DateTime.Now.Subtract(created).TotalHours > 2)
                                {
                                    mark_deletion = true;
                                }


                            }
                            if (mark_deletion)
                            {
                                foreach (string file in files)
                                    File.Delete(file);

                                Directory.Delete(directory);
                            }
                        }

                        _iDirsDone++;
                        System.Threading.Thread.Sleep(25);
                    }
                    //TODO: Compress all files

                }
                _bCleanup = false;
            });
            t.Start();
        }
    }
}