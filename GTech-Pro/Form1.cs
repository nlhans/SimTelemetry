using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Game.Rfactor;
using Triton;
using Triton.Joysticks;

namespace GTech_Pro
{
    public partial class Form1 : Form
    {
        private Joystick Joy;
        public static int TheButton = 6;
        private int StatusMenu = 0;
        Timer t = new Timer();

        void LiveTelemetry_FormClosing(object sender, FormClosingEventArgs e)
        {
            TritonBase.TriggerExit();
        }


        void Joy_Release(string joystick, int button)
        {
            if (button == TheButton)
            {
                StatusMenu++;

            }
        }
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            try
            {
                //rFactor r = new rFactor(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }

            this.FormClosing += LiveTelemetry_FormClosing;

            string[] lines = File.ReadAllLines("config.txt");
            string controller = "";
            bool controlleruseindex = false;
            int controllerindex = 0;
            foreach (string line in lines)
            {
                string[] p = line.Trim().Split("=".ToCharArray());
                if (line.StartsWith("button"))
                {
                    TheButton = Convert.ToInt32(p[1]);
                }
                if (line.StartsWith("index"))
                {
                    controlleruseindex = true;
                    controllerindex = Convert.ToInt32(p[1]);
                }
                if (line.StartsWith("controller"))
                {
                    controller = p[1];
                }
            }

            /*
            List<JoystickDevice> devices = JoystickDevice.GetDevices();
            StringBuilder devicedata = new StringBuilder();
            if (devices.Count == 0)
            {
                MessageBox.Show("Cycling through panels disabled, no suitable joystick found?");
                devicedata.AppendLine("No devices found!");
            }
            else
            {
                if (controlleruseindex)
             * 
                {
                    Joy = new Joystick(devices[controllerindex]);
                    Joy.Release += Joy_Release;
                }
                else
                {
                    int i = 0;
                    foreach (JoystickDevice jd in devices)
                    {
                        devicedata.AppendLine("device #" + i + ": " + jd.Name);
                        if (jd.Name.Contains(controller.Trim()))
                        {
                            Joy = new Joystick(jd);
                            Joy.Release += Joy_Release;


                        }
                        i++;
                    }
                }
            }
            */
            // READY
            t.Interval = 1;
            t.Tick += new EventHandler(t_Tick);
            t.Start();

            t2.Interval = 20;
            t2.Tick += new EventHandler(t2_Tick);
            t2.Start();
        }

        void t2_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private Timer t2 = new Timer();
        private bool triggered = false;
        private bool recording = false;
        private double Acc_Start;
        private Dictionary<int, double> Acc_Times = new Dictionary<int, double>();
        private Dictionary<int, double> BestAcc_Times = new Dictionary<int, double>();
        private Dictionary<int, double>Diff_Times = new Dictionary<int, double>();
        private double acc_time;
        
        void t_Tick(object sender, EventArgs e)
        {
            double spd = 3.6 * Math.Max(rFactor.Player.Speed, Math.Abs(rFactor.Player.SpeedSlipping));
            double time = rFactor.Session.Time;
            spd = Math.Abs(spd);
            if (spd < 0.1)
            {
                triggered = true;
                recording = false;
            }
            else if (triggered)
            {
                triggered = false;
                recording = true;
                Acc_Start = time;
                Acc_Times = new Dictionary<int, double>();
                Diff_Times = new Dictionary<int, double>();
                Diff_Times.Add(0, 0);
                Acc_Times.Add(0,0);
            }
            else if (recording)
            {
                 acc_time = time - Acc_Start;
                 int ispd = Convert.ToInt32(Math.Floor(spd));
                 if (BestAcc_Times.ContainsKey(ispd) == false) BestAcc_Times.Add(ispd,acc_time);
                if (Acc_Times.ContainsKey(ispd) == false)
                {
                    Acc_Times.Add(ispd, acc_time);
                    Diff_Times.Add(ispd, acc_time - BestAcc_Times[ispd]);
                    if (BestAcc_Times[ispd] > Acc_Times[ispd])
                        BestAcc_Times[ispd] = Acc_Times[ispd];
                }

                if (spd > 400 || spd < 0.1  ||
                    rFactor.Player.Pedals_Brake > 0.2)
                    // done!
                {
                    recording = false;

                }
            }
        }

        private double GetTime(int speed)
        {
            if (!Acc_Times.ContainsKey(speed))
                return acc_time;
            else return Acc_Times[speed];
        }

        private double GetBestTime(int speed)
        {
            if (!BestAcc_Times.ContainsKey(speed))
                return acc_time;
            else return BestAcc_Times[speed];
        }

        private double GetDiffTime(int speed)
        {
            if (!Diff_Times.ContainsKey(speed))
                return 0;
            else return Diff_Times[speed];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);

            System.Drawing.Font bf = new Font("Arial", 18f);
            System.Drawing.Font f = new Font("Arial", 10f);
            System.Drawing.Font sf = new Font("Arial", 8f);
            try
            {
                switch (StatusMenu)
                {
                    default:
                        StatusMenu = 0;
                        this.Invalidate();
                        break;

                    case 0:

                        if (triggered == true)
                        {
                            g.FillRectangle(Brushes.DarkGreen, e.ClipRectangle);
                            g.DrawString("Triggered", bf, Brushes.White, 10f, 10f);
                        }
                        else if (recording)
                        {

                            g.FillRectangle(Brushes.Orange, e.ClipRectangle);
                            g.DrawString("Recording", bf, Brushes.White, 10f, 10f);

                        }
                        else
                        {


                            g.FillRectangle(Brushes.DarkRed, e.ClipRectangle);
                            g.DrawString("Not recording", bf, Brushes.White, 10f, 10f);
                        }

                        break;

                    case 1:
                        g.DrawString("Best run: " + Math.Round(GetBestTime(100), 1) + "s", f,
                                     Brushes.White, 10f, 10f);
                        g.DrawString("Last run: " + Math.Round(GetBestTime(100), 1) + "s", f,
                                     Brushes.White, 10f, 28f);
                        for (int spd = 0; spd < 400; spd += 20)
                        {
                            float extra = 0;
                            if (spd >= 200) extra = 140f;
                            g.DrawString((20+spd).ToString(), sf, Brushes.Yellow, extra+120f, 10f + Convert.ToSingle((spd%200)*0.8));
                            g.DrawString(Math.Round(GetTime(spd+20),2).ToString(), sf, Brushes.White, extra + 150f,
                                         10f + Convert.ToSingle((spd % 200) * 0.8));
                            g.DrawString(Math.Round(GetBestTime(spd + 20), 2).ToString(), sf, Brushes.White, extra + 200f,
                                         10f + Convert.ToSingle((spd % 200) * 0.8));
                            g.DrawString(Math.Round(GetDiffTime(spd + 20), 2).ToString(), sf, Brushes.White, extra + 230f,
                                         10f + Convert.ToSingle((spd % 200) * 0.8));

                        }
                        break;
                }
            }catch(Exception ex)
            {
                g.DrawString(ex.Message, sf, Brushes.White, 20, 20);
                g.DrawString(ex.StackTrace.Replace("in","\n"), sf, Brushes.White, 20, 50);

            }

        }
    }
}