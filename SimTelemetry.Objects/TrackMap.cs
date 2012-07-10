
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

namespace SimTelemetry.Controls
{
    public partial class TrackMap : UserControl
    {
        public string AIW_File = "";

        protected double pos_x_max = 1000000000.0;
        protected double pos_x_min = -1000000000.0;
        protected double pos_y_max = 1000000000.0;
        protected double pos_y_min = -1000000000.0;
        protected WayPoint[] waypoints = new WayPoint[25001];
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;
            
            if (BackgroundImage == null)
            {
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            }
            else
            {
                g.DrawImage(BackgroundImage, 0, 0);
            } 
            base.OnPaint(e);
        }

        public void ReadAIW(string file)
        {
            AIW_File = file;

            if (file == "") return;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            waypoints = new WayPoint[25001];

            pos_x_max = 1000000000.0;
            pos_x_min = -1000000000.0;
            pos_y_max = 1000000000.0;
            pos_y_min = -1000000000.0;
            if (new FileInfo(AIW_File).Exists)
            {

                StreamReader streamReader = File.OpenText(AIW_File);
                int waypoints_count = 0;
                while (streamReader != null && streamReader.Peek() != -1)
                {
                    string line = streamReader.ReadLine().ToLower();
                    if (line.Contains("number_waypoints="))
                    {
                        waypoints_count = Convert.ToInt32(line.Replace("number_waypoints=", "").Trim());
                        break;
                    }
                }
                if (waypoints_count > 0)
                {
                    string s_lap_length = "0";
                    string s_sector_1_length = "0";
                    string s_sector_2_length = "0";
                    while (streamReader.Peek() != -1)
                    {
                        string infoline = streamReader.ReadLine().Split(new char[1] { '/' })[0].ToLower().Trim();
                        if (infoline.Contains("lap_length"))
                            s_lap_length = infoline.Replace("lap_length=", "").Trim();
                        if (infoline.Contains("sector_1_length"))
                            s_sector_1_length = infoline.Replace("sector_1_length=", "").Trim();
                        if (infoline.Contains("sector_2_length"))
                        {
                            s_sector_2_length = infoline.Replace("sector_2_length=", "").Trim();
                            break;
                        }
                    }
                    int waypoints_offset = 1;
                    waypoints_min_1 = checked((short)((int)waypoints_count - 1));
                    for (int waypoint_i = (short)waypoints_offset;
                         (int)waypoint_i <= (int)waypoints_min_1;
                         ++waypoint_i)
                    {
                        while (streamReader.Peek() != -1)
                        {
                            string waypoint_line =
                                streamReader.ReadLine().Split(new char[1] { '/' })[0].ToLower().Trim();
                            if (waypoint_line.Contains("wp_pos"))
                                waypoints[(int)waypoint_i].Pos = waypoint_line;
                            if (waypoint_line.Contains("wp_score"))
                                waypoints[(int)waypoint_i].Score = waypoint_line;
                            if (waypoint_line.Contains("wp_branchid"))
                                waypoints[(int)waypoint_i].BranchID = waypoint_line;
                            if (waypoint_line.Contains("wp_ptrs"))
                            {
                                waypoints[(int)waypoint_i].PTRS = waypoint_line;
                                break;
                            }
                        }
                        if (waypoints[waypoint_i].Pos.Contains("wp_pos="))
                        {
                            string[] wp_split = waypoints[(int)waypoint_i].Pos.Split(new char[1] { ',' });
                            if (wp_split.Length == 3)
                            {
                                string[] wp_bf = wp_split[0].Split(new char[1] { '(' });
                                string[] wp_af = wp_split[2].Split(new char[1] { ')' });
                                waypoints[waypoint_i].PosX = Convert.ToDouble(wp_bf[1]);
                                waypoints[waypoint_i].PosZ = Convert.ToDouble(wp_af[0]);
                                if (waypoints[waypoint_i].PosX < pos_x_max)
                                    pos_x_max = waypoints[waypoint_i].PosX;
                                if (waypoints[waypoint_i].PosX > pos_x_min)
                                    pos_x_min = waypoints[waypoint_i].PosX;
                                if (waypoints[waypoint_i].PosZ < pos_y_max)
                                    pos_y_max = waypoints[waypoint_i].PosZ;
                                if (waypoints[waypoint_i].PosZ > pos_y_min)
                                    pos_y_min = waypoints[waypoint_i].PosZ;
                            }
                        }
                        if (waypoints[waypoint_i].BranchID.Contains("wp_branchid="))
                            waypoints[waypoint_i].BranchIDstr =
                                ((waypoints[waypoint_i].BranchID.Contains("wp_branchid=(0)")) ? "1" : "0");

                        if (waypoints[waypoint_i].Score.Contains("wp_score="))
                        {
                            string[] score_parts = waypoints[(int)waypoint_i].Score.Split(new char[1] { ',' });
                            string[] score_bf = score_parts[0].Split(new char[1] { '(' });
                            string[] score_af = score_parts[1].Split(new char[1] { ')' });
                            waypoints[waypoint_i].Sector = score_bf[1];
                            waypoints[waypoint_i].Distance = Convert.ToDouble(score_af[0]);
                        }
                        if (waypoints[waypoint_i].PTRS.Contains("wp_ptrs="))
                        {
                            string[] ptrs_parts =
                                waypoints[(int)waypoint_i].PTRS.Split(new char[1] { ',' })[3].Split(new char[1] { ')' });
                            waypoints[waypoint_i].PTRS4 = ptrs_parts[0];
                        }
                    }
                    streamReader.Close();
                    streamReader.Dispose();
                    double distance_sector0 = 100000.0;
                    double distance_sector1 = 100000.0;
                    double distance_sector2 = 100000.0;
                    int waypoint_sector0;
                    int waypoint_sector1;
                    int waypoint_sector2;
                    for (int k = waypoints_offset; k <= waypoints_min_1; ++k)
                    {
                        if (waypoints[k].BranchIDstr.Contains("0"))
                        {
                            if (waypoints[k].Sector.Contains("0"))
                            {
                                distance_sector0 = waypoints[k].Distance;
                                waypoint_sector0 = k;
                            }
                            if (waypoints[k].Sector.Contains("1"))
                            {
                                distance_sector1 = waypoints[k].Distance;
                                waypoint_sector1 = k;
                            }
                            if (waypoints[k].Sector.Contains("2"))
                            {
                                distance_sector2 = waypoints[k].Distance;
                                waypoint_sector2 = k;
                            }
                        }
                    }
                    pos_x_span = pos_x_min - pos_x_max;
                    pos_y_span = pos_y_min - pos_y_max;

                    pos_x_span = Math.Max(pos_x_span, pos_y_span);
                    pos_y_span = Math.Min(pos_x_span, pos_y_span);
                    destination_width = pos_x_span / (this.Width - border_size * 2);
                    destination_height = pos_y_span / (this.Height - 80 - border_size * 2);
                    x_scale = pos_x_max >= 0.0
                                  ? Convert.ToInt32(0.0 - pos_x_max / (double)destination_width)
                                  : Convert.ToInt32(Math.Abs(pos_x_max) / (double)destination_width);
                    y_scale = pos_y_min >= 0.0
                                  ? Convert.ToInt32(pos_y_min / (double)destination_height)
                                  : Convert.ToInt32(Math.Abs(pos_y_min) / (double)destination_height);
                    width =
                        Convert.ToInt32(Math.Round((border_size * 2.0) + pos_x_span / ((double)destination_width)));
                    height =
                        Convert.ToInt32(Math.Round((border_size * 2.0) + pos_y_span / ((double)destination_height))) + 80;

                    BackgroundImage = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(BackgroundImage);
                    g.FillRectangle(Brushes.Black, 0, 0, width, height);
                    SolidBrush brush_sector1 = new SolidBrush(Color.FromArgb(105, 105, 105));
                    SolidBrush brush_sector2 = new SolidBrush(Color.FromArgb(47, 79, 79));
                    SolidBrush brush_sector3 = new SolidBrush(Color.FromArgb(85, 107, 47));

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    int ind = 2;
                    int trackmap_trace_width = 9;
                    while (ind <= waypoints_min_1)
                    {
                        double a1 = x_scale + (double)border_size + waypoints[ind].PosX / (double)destination_width;
                        double a2 = 80 + y_scale + (double)border_size - waypoints[ind].PosZ / (double)destination_height;

                        //if (!waypoints[ind].BranchIDstr.Contains("0") && (waypoints[ind].PTRS4.Contains("0") || waypoints[ind].PTRS4.Contains("2")))
                        if (true)
                        {
                            string sector = waypoints[ind].Sector;
                            if (sector.Contains("0"))
                                g.FillEllipse(brush_sector1, (int)Math.Round(a1), (int)Math.Round(a2),
                                              trackmap_trace_width, trackmap_trace_width);
                            else if (sector.Contains("1"))
                                g.FillEllipse(brush_sector2, (int)Math.Round(a1), (int)Math.Round(a2),
                                              trackmap_trace_width, trackmap_trace_width);
                            else if (sector.Contains("2"))
                                g.FillEllipse(brush_sector3, (int)Math.Round(a1), (int)Math.Round(a2),
                                              trackmap_trace_width, trackmap_trace_width);

                        }
                        ++ind;

                    }

                    // draw track name
                    try
                    {
                        string gdb = AIW_File.Replace(".AIW", ".gdb");
                        gdb = gdb.Replace(".aiw", ".gdb");

                        // alright open it
                        string[] data = File.ReadLines(gdb).ToArray();

                        string track_location = "";
                        string track_length = "";
                        string track_type = "";
                        string track_name = "";

                        foreach (string data_line in data)
                        {
                            string[] spl = data_line.Split("=".ToCharArray());

                            if (data_line.Contains("TrackName"))
                                track_name = spl[1].Trim();


                            if (data_line.Contains("Location"))
                                track_location = spl[1].Trim();


                            if (data_line.Contains("Length"))
                                track_length = spl[1].Trim();


                            if (data_line.Contains("TrackType"))
                                track_type = spl[1].Trim();

                        }

                        System.Drawing.Font tf24 = new Font("calibri", 24f);
                        System.Drawing.Font tf16 = new Font("calibri", 16f);
                        System.Drawing.Font tf12 = new Font("calibri", 12f);
                        System.Drawing.Font tf18 = new Font("calibri", 18f);

                        g.DrawString(track_name, tf24, Brushes.White, 10f, 10f);
                        g.DrawString(track_location, tf18, Brushes.White, 10f, 40f);
                        g.DrawString(track_length + " , " + track_type, tf12, Brushes.White, 10f, 65f);

                    }
                    catch (Exception ex)
                    {


                    }
                    brush_sector3.Dispose();
                    brush_sector1.Dispose();
                    brush_sector2.Dispose();
                    BackgroundImage.Save("current.png");
                    this.Invalidate();
                }
            }
            else
            {
                //5MessageBox.Show("Could not find " + AIW_File);
            }
        }

        protected int width, height, waypoints_min_1, border_size = 15;
        protected double x_scale, y_scale, destination_width, destination_height, pos_x_span, pos_y_span;

        protected Bitmap BackgroundImage;

        /*
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            Graphics g2 = e.Graphics;
            g2.FillRectangle(Brushes.Black, 0, 0, 1, 1);

            try
            {
            }
            catch (Exception ex)
            {

            }
        }*/

        public TrackMap()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}