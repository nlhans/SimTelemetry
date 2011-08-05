using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Game.Rfactor;
using Triton.Joysticks;

namespace LiveTelemetry
{
    public partial class Gauge_A1GP : UserControl
    {
        private List<double> FuelUsage = new List<double>();
        private List<double> EngineUsage = new List<double>();
        private double Engine_LastLap = 1;
        private double Engine_Max;
        private double Fuel_LastLap = 0;
        private double Fuel_LastLapNo;
        private double Engine_Shortterm = 0;

        private Joystick joy;
        private Timer t = new Timer();
        private int Counter = 0;

        private DateTime lastrender = DateTime.Now;

        public Gauge_A1GP(Joystick j)
        {
            joy = j;
            j.Release += new JoystickButtonEvent(j_Release);

            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            t = new Timer();
            t.Interval = 500;
            t.Tick += t_Tick;

            t.Enabled = true;
            t.Start();
        }

        void j_Release(string joystick, int button)
        {
            if(Counter == 10 && button == LiveTelemetry.TheButton)
            {
                Counter = 0;
                LiveTelemetry.StatusMenu = 0;
            }
        }
        void t_Tick(object sender, EventArgs e)
        {
            if (joy.Holding(LiveTelemetry.TheButton))
            {
                Counter++;
                if (Counter >= 4 && Counter != 10)
                {
                    LiveTelemetry.StatusMenu = 0;
                    Engine_Max = rFactor.Player.Engine_Lifetime_Live;
                    Counter = 10;
                }
            }
            else
                Counter = 0;
        }


        public void Update()
        {
            if(Fuel_LastLapNo != rFactor.Drivers.Player.Laps)
            {
                double engine_state = rFactor.Player.Engine_Lifetime_Live / rFactor.Player.Engine_Lifetime_Typical;

                Fuel_LastLapNo = rFactor.Drivers.Player.Laps;
                double usedF = rFactor.Drivers.Player.Fuel - Fuel_LastLap;
                double usedE = engine_state - Engine_LastLap;
                Engine_LastLap = engine_state;
                Fuel_LastLap = rFactor.Drivers.Player.Fuel;

                if (usedF < 0)
                    FuelUsage.Add(0 - usedF);
                if(usedE > 0 && usedE < 0.05)
                    EngineUsage.Add(usedE);


            }

        }

        private Color DimColor = Color.FromArgb(70, 70, 70);
        private SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                int border_bounds = 60;
                int width = e.ClipRectangle.Width;
                int height = e.ClipRectangle.Height;
                width -= border_bounds;
                height -= border_bounds;
                base.OnPaint(e);
                double SpeedTop = 3.6*Computations.GetPracticalTopSpeed();
                SpeedTop = 3.6*Computations.GetTheoraticalTopSpeed();
                double SpeedMax = SpeedTop;
                double SpeedStep = 30;
                if (SpeedMax < 300) SpeedStep = 25;
                if (SpeedMax < 200) SpeedStep = 20;
                if (SpeedMax%SpeedStep > 0)
                    SpeedMax += SpeedStep - (SpeedMax%SpeedStep);
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (rFactor.Session.Cars == 0) return;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2, border_bounds/2, height, height, 90, 225);
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2 + 8, border_bounds/2 + 8, height - 16, height - 16,
                          90, 225);
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2 + 25, border_bounds/2 + 25, height - 50, height - 50,
                          90, 225);

                //g.SmoothingMode = SmoothingMode.HighSpeed;
                double RPM_Max = 1000*Math.Ceiling(Rads_RPM(rFactor.Player.Engine_RPM_Max_Live)/1000);
                double RPM_Min = 0;
                double RPM_Step = 2000;
                // Ranges.
                if (RPM_Max <= 8000) RPM_Step = 1000;
                else if (RPM_Max <= 12000) RPM_Step = 2000;
                else if (RPM_Max <= 20000)
                {
                    RPM_Step = 2000;
                    RPM_Min = RPM_Max - 6*2000;

                }
                else if (RPM_Max <= 30000)
                {
                    RPM_Step = 2000;
                    RPM_Min = RPM_Max - 7*2000;

                }
                double rpm_idle = Rads_RPM(rFactor.Player.Engine_RPM_Idle_Max);
                if (RPM_Min > rpm_idle)
                {
                    RPM_Min = rpm_idle;

                    if (RPM_Min%RPM_Step != 0)
                        RPM_Min -= (RPM_Min%RPM_Step);

                }

                if (RPM_Max > 14000)
                    RPM_Min = 6000;
                if (RPM_Max > 20000)
                    RPM_Min = 8000;


                if (RPM_Min%RPM_Step != 0)
                    RPM_Min += RPM_Step - (RPM_Min%RPM_Step);

                if (RPM_Max%RPM_Step != 0)
                    RPM_Max += RPM_Step - (RPM_Max%RPM_Step);

                System.Drawing.Font f = new Font("Arial", 10f, FontStyle.Bold);
                
                double fAngle_RPM_RedLine = (Rads_RPM(rFactor.Player.Engine_RPM_Max_Live) - RPM_Step/2 - RPM_Min)/
                                            (RPM_Max - RPM_Min)*225;
                if (double.IsInfinity(fAngle_RPM_RedLine) || double.IsNaN(fAngle_RPM_RedLine)) fAngle_RPM_RedLine = 200;
                int Angle_RPM_RedLine = Convert.ToInt32(Math.Round(fAngle_RPM_RedLine));


                double fAngle_RPM_WarningLine = (Rads_RPM(rFactor.Player.Engine_Lifetime_RPM_Base) - RPM_Step/2 -
                                                 RPM_Min) / (RPM_Max - RPM_Min) * 225;
                if (double.IsInfinity(fAngle_RPM_WarningLine) || double.IsNaN(fAngle_RPM_WarningLine)) fAngle_RPM_WarningLine = 180;
                int Angle_RPM_WarningLine = Convert.ToInt32(Math.Round(fAngle_RPM_WarningLine));
                if (Angle_RPM_WarningLine > Angle_RPM_RedLine)
                {
                    g.DrawArc(new Pen(Color.DarkRed, 10f), border_bounds/2 + 58, border_bounds/2 + 57, height - 116,
                              height - 116,
                              90 + Angle_RPM_RedLine, 225 - Angle_RPM_RedLine);
                    g.DrawArc(new Pen(Color.Red, 10f), border_bounds/2 + 58, border_bounds/2 + 57, height - 116,
                              height - 116, 90 + Angle_RPM_WarningLine, 225 - Angle_RPM_WarningLine);
                }
                else
                {

                    g.DrawArc(new Pen(Color.DarkRed, 10f), border_bounds/2 + 58, border_bounds/2 + 57, height - 116,
                              height - 115, 90 + Angle_RPM_WarningLine, 225 - Angle_RPM_WarningLine);
                    g.DrawArc(new Pen(Color.Red, 10f), border_bounds/2 + 58, border_bounds/2 + 57, height - 116,
                              height - 116,
                              90 + Angle_RPM_RedLine, 225 - Angle_RPM_RedLine);
                }
                for (double angle = 90; angle <= 90 + 225; angle += 225.0/((RPM_Max - RPM_Min)/RPM_Step))
                {
                    double sin_a = Math.Sin(angle/180.0*Math.PI);
                    double cos_a = Math.Cos(angle/180.0*Math.PI);

                    double y_a = e.ClipRectangle.Height/2 + sin_a*70;
                    double x_a = e.ClipRectangle.Height/2 + cos_a*70;
                    double y_b = e.ClipRectangle.Height/2 + sin_a*80;
                    double x_b = e.ClipRectangle.Height/2 + cos_a*80;

                    double y_c = e.ClipRectangle.Height/2 + sin_a*95 - 8;
                    double x_c = e.ClipRectangle.Height/2 + cos_a*95 - 8;

                    g.DrawLine(new Pen(Color.White, 2f), Convert.ToSingle(x_a), Convert.ToSingle(y_a),
                               Convert.ToSingle(x_b), Convert.ToSingle(y_b));
                    double RPM = RPM_Min + (RPM_Max - RPM_Min)*(angle - 90)/225;
                    g.DrawString((RPM/1000.0).ToString("0"), f, Brushes.White, Convert.ToSingle(x_c),
                                 Convert.ToSingle(y_c));

                    for (int i = 1; i < 5; i++)
                    {
                        double angle2 = angle + 225.0/((RPM_Max - RPM_Min)/RPM_Step)*i/5;
                        if (angle2 > 90 + 225) break;
                        double sin2_a = Math.Sin(angle2/180.0*Math.PI);
                        double cos2_a = Math.Cos(angle2/180.0*Math.PI);

                        double y2_a = e.ClipRectangle.Height/2 + sin2_a*72;
                        double x2_a = e.ClipRectangle.Height/2 + cos2_a*72;
                        double y2_b = e.ClipRectangle.Height/2 + sin2_a*78;
                        double x2_b = e.ClipRectangle.Height/2 + cos2_a*78;

                        if (angle2 > 90 + fAngle_RPM_RedLine)
                            g.DrawLine(new Pen(Color.Red, 1f), Convert.ToSingle(x2_a), Convert.ToSingle(y2_a),
                                       Convert.ToSingle(x2_b), Convert.ToSingle(y2_b));
                        else
                            g.DrawLine(new Pen(Color.White, 1f), Convert.ToSingle(x2_a), Convert.ToSingle(y2_a),
                                       Convert.ToSingle(x2_b), Convert.ToSingle(y2_b));

                    }
                }

                for (double angle = 90; angle <= 90 + 225.1; angle += 225.0/(SpeedMax/SpeedStep))
                {

                    double sin_a = Math.Sin(angle/180.0*Math.PI);
                    double cos_a = Math.Cos(angle/180.0*Math.PI);

                    double y_a = e.ClipRectangle.Height/2 + sin_a*133 - 3;
                    double x_a = e.ClipRectangle.Height/2 + cos_a*133 - 3;

                    double y_c = e.ClipRectangle.Height/2 + sin_a*147 - 10;
                    double x_c = e.ClipRectangle.Height/2 + cos_a*147 - 15;

                    g.FillEllipse(Brushes.DarkRed, Convert.ToSingle(x_a), Convert.ToSingle(y_a), 6, 6);
                    g.DrawEllipse(new Pen(Color.White, 1f), Convert.ToSingle(x_a), Convert.ToSingle(y_a), 6, 6);

                    double Spd = SpeedMax*(angle - 90)/225;
                    g.DrawString(Spd.ToString("000"), f, Brushes.White, Convert.ToSingle(x_c),
                                 Convert.ToSingle(y_c));

                }

                // draw rpm 
                double RPMLive = Rads_RPM(rFactor.Player.Engine_RPM);
                double fAngle_RPM = 90 + (RPMLive - RPM_Min)/(RPM_Max - RPM_Min)*225;
                if (fAngle_RPM < 90) fAngle_RPM = 90;
                if (fAngle_RPM > 90 + 225) fAngle_RPM = 90 + 225;
                double rpm_gauge_sin = Math.Sin(fAngle_RPM/180.0*Math.PI);
                double rpm_gauge_cos = Math.Cos(fAngle_RPM/180.0*Math.PI);

                double rpm_gauge_ya = e.ClipRectangle.Height/2 + rpm_gauge_sin*-15;
                double rpm_gauge_xa = e.ClipRectangle.Height/2 + rpm_gauge_cos*-15;
                double rpm_gauge_yb = e.ClipRectangle.Height/2 + rpm_gauge_sin*80;
                double rpm_gauge_xb = e.ClipRectangle.Height/2 + rpm_gauge_cos*80;

                g.DrawLine(new Pen(Color.DarkRed, 4f), Convert.ToSingle(rpm_gauge_xa), Convert.ToSingle(rpm_gauge_ya),
                           Convert.ToSingle(rpm_gauge_xb), Convert.ToSingle(rpm_gauge_yb));

                // fill up speed bar
                double SpeedLive = rFactor.Player.Speed*3.6;
                double fAngle_Speed = 90 + SpeedLive/SpeedMax*225;
                for (double angle = 90; angle <= fAngle_Speed; angle += 3.5)
                {
                    double Spd = SpeedMax*(angle - 90)/225;
                    if (Spd > SpeedLive || Math.Abs(Spd - SpeedLive) < 0.5) break;
                    double le = 3;
                    if (angle + 3 > fAngle_Speed) le = fAngle_Speed - angle;
                    Color clr = Color.FromArgb(Convert.ToInt32(100 + Spd*155/SpeedMax),
                                               Convert.ToInt32(255 - Spd*255/SpeedMax), 0);
                    g.DrawArc(new Pen(clr, 4f), Convert.ToSingle(border_bounds/2 + 4),
                              Convert.ToSingle(border_bounds/2 + 4), Convert.ToSingle(height - 8),
                              Convert.ToSingle(height - 8), Convert.ToSingle(angle), Convert.ToSingle(le));
                }
                

                // draw speed

                if (fAngle_Speed < 90) fAngle_Speed = 90;
                if (fAngle_Speed > 90 + 225) fAngle_Speed = 90 + 225;

                double Speed_gauge_sin = Math.Sin(fAngle_Speed/180.0*Math.PI);
                double Speed_gauge_cos = Math.Cos(fAngle_Speed/180.0*Math.PI);

                double Speed_gauge_ya = e.ClipRectangle.Height/2 + Speed_gauge_sin*-20;
                double Speed_gauge_xa = e.ClipRectangle.Height/2 + Speed_gauge_cos*-20;
                double Speed_gauge_yb = e.ClipRectangle.Height/2 + Speed_gauge_sin*133;
                double Speed_gauge_xb = e.ClipRectangle.Height/2 + Speed_gauge_cos*133;

                g.DrawLine(new Pen(Color.DarkGreen, 6f), Convert.ToSingle(Speed_gauge_xa),
                           Convert.ToSingle(Speed_gauge_ya), Convert.ToSingle(Speed_gauge_xb),
                           Convert.ToSingle(Speed_gauge_yb));
                g.FillEllipse(Brushes.DarkGreen, e.ClipRectangle.Height/2 - 10, e.ClipRectangle.Height/2 - 10, 20, 20);
                

                //draw water &  oil
                // 60C to 140C
                
                //g.DrawArc(new Pen(Color.White, 2f), height + 40, 30,80, 80, 180, 180);
                /*double Max_oil = rFactor.Player.Engine_Lifetime_Oil_Base;
                double fAngle_Oil_redline = (Max_oil - 60)/(140 - 60)*180;
                int Angle_Oil_redline = Convert.ToInt32(Math.Round(fAngle_Oil_redline));
                g.DrawArc(new Pen(Color.Red, 6f), height + 38, 32, 80,
                          80,
                          180 + Angle_Oil_redline, 180 - Angle_Oil_redline);

                fAngle_Oil_redline += 180;
                for (double angle = 180; angle <= 180 + 180; angle += 180/((140 - 60)/20))
                {

                    double sin_a = Math.Sin(angle/180.0*Math.PI);
                    double cos_a = Math.Cos(angle/180.0*Math.PI);

                    double x_c = height + 40 + 40 + cos_a*55 - 12;
                    double y_c = 30 + 40 + sin_a*55 - 8;


                    double x_a = height + 40 + 40 + cos_a*40;
                    double y_a = 30 + 40 + sin_a*40;
                    double x_b = height + 40 + 40 + cos_a*35;
                    double y_b = 30 + 40 + sin_a*35;

                    if (angle > fAngle_Oil_redline)
                        g.DrawLine(new Pen(Color.Red, 3f), Convert.ToSingle(x_a), Convert.ToSingle(y_a),
                                   Convert.ToSingle(x_b), Convert.ToSingle(y_b));
                    else
                        g.DrawLine(new Pen(Color.White, 3f), Convert.ToSingle(x_a), Convert.ToSingle(y_a),
                                   Convert.ToSingle(x_b), Convert.ToSingle(y_b));

                    double degr = 60 + (angle - 180)/180*(140 - 60);
                    g.DrawString(degr.ToString("000"), f, Brushes.White, Convert.ToSingle(x_c), Convert.ToSingle(y_c));

                    for (int i = 1; i < 5; i++)
                    {
                        double angle2 = angle + 180.0/((140 - 60)/20)*i/5.0;
                        if (angle2 > 180 + 180) break;
                        double sin2_a = Math.Sin(angle2/180.0*Math.PI);
                        double cos2_a = Math.Cos(angle2/180.0*Math.PI);


                        double x2_a = height + 40 + 40 + cos2_a*40;
                        double y2_a = 30 + 40 + sin2_a*40;
                        double x2_b = height + 40 + 40 + cos2_a*35;
                        double y2_b = 30 + 40 + sin2_a*35;
                        if (angle2 > fAngle_Oil_redline)
                            g.DrawLine(new Pen(Color.Red, 1f), Convert.ToSingle(x2_a), Convert.ToSingle(y2_a),
                                       Convert.ToSingle(x2_b), Convert.ToSingle(y2_b));
                        else
                            g.DrawLine(new Pen(Color.White, 1f), Convert.ToSingle(x2_a), Convert.ToSingle(y2_a),
                                       Convert.ToSingle(x2_b), Convert.ToSingle(y2_b));

                    }
                }


                g.DrawString("Oil", f, DimBrush, height + 50, 60);
                double OilTempLive = rFactor.Player.Engine_Temperature_Oil;
                double fAngle_OilTemp = 180 + (OilTempLive - 60)/(140 - 60)*180;
                if (fAngle_OilTemp < 165) fAngle_OilTemp = 165;
                if (fAngle_OilTemp > 180 + 180) fAngle_OilTemp = 180 + 180;
                double OilTemp_gauge_sin = Math.Sin(fAngle_OilTemp/180.0*Math.PI);
                double OilTemp_gauge_cos = Math.Cos(fAngle_OilTemp/180.0*Math.PI);

                double OilTemp_gauge_ya = 30 + 40 + OilTemp_gauge_sin*15;
                double OilTemp_gauge_xa = height + 40 + 40 + OilTemp_gauge_cos*15;
                double OilTemp_gauge_yb = 30 + 40 + OilTemp_gauge_sin*40;
                double OilTemp_gauge_xb = height + 40 + 40 + OilTemp_gauge_cos*40;

                g.DrawLine(new Pen(Color.DarkSlateGray, 2f), Convert.ToSingle(OilTemp_gauge_xa),
                           Convert.ToSingle(OilTemp_gauge_ya), Convert.ToSingle(OilTemp_gauge_xb),
                           Convert.ToSingle(OilTemp_gauge_yb));

                g.DrawString("Water", f, DimBrush, height + 75, 60);
                double WaterTempLive = rFactor.Player.Engine_Temperature_Water;
                double fAngle_WaterTemp = 180 + (WaterTempLive - 60)/(140 - 60)*180;
                if (fAngle_WaterTemp < 165) fAngle_WaterTemp = 165;
                if (fAngle_WaterTemp > 180 + 180) fAngle_WaterTemp = 180 + 180;
                double WaterTemp_gauge_sin = Math.Sin(fAngle_WaterTemp/180.0*Math.PI);
                double WaterTemp_gauge_cos = Math.Cos(fAngle_WaterTemp/180.0*Math.PI);

                double WaterTemp_gauge_ya = 30 + 40 + WaterTemp_gauge_sin*15;
                double WaterTemp_gauge_xa = height + 40 + 40 + WaterTemp_gauge_cos*15;
                double WaterTemp_gauge_yb = 30 + 40 + WaterTemp_gauge_sin*40;
                double WaterTemp_gauge_xb = height + 40 + 40 + WaterTemp_gauge_cos*40;

                g.DrawLine(new Pen(Color.DarkBlue, 2f), Convert.ToSingle(WaterTemp_gauge_xa),
                           Convert.ToSingle(WaterTemp_gauge_ya), Convert.ToSingle(WaterTemp_gauge_xb),
                           Convert.ToSingle(WaterTemp_gauge_yb));*/

                //g.DrawString("Top speed: " + (SpeedTop).ToString("000") + "km/h", f, Brushes.White, height + 40, 100);

                System.Drawing.Font gear_f = new Font("Arial", 30f);
                System.Drawing.Font speed_f = new Font("Arial", 18f);

                if (rFactor.Player.Gear == -1 || rFactor.Player.Gear == 0xFF)
                    g.DrawString("R", gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                 border_bounds/2 + height/2 + 40);
                else if (rFactor.Player.Gear > 0)
                    g.DrawString(rFactor.Player.Gear.ToString(), gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                 border_bounds/2 + height/2 + 40);
                else if (rFactor.Player.Gear == 0)
                    g.DrawString("N", gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                 border_bounds/2 + height/2 + 40);

                g.DrawString(Math.Abs(rFactor.Player.Speed*3.6).ToString("000") + "km/h", speed_f, Brushes.White,
                             border_bounds/2 + height/2 + 10, border_bounds/2 + height/2 + 80);

                g.DrawString("PEDALS", f, DimBrush, height - 50, 119);
                /*for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc > rFactor.Drivers.Player.Brake*120)
                        g.FillRectangle(DimBrush, height + 10 + perc, 120, 2, 13);
                    else
                        g.FillRectangle(Brushes.DarkRed, height + 10 + perc, 120, 2, 13);

                }

                for (int perc = 0; perc < rFactor.Drivers.Player.Throttle*120; perc += 3)
                {
                    g.FillRectangle(Brushes.DarkGreen, height + 10 + perc, 120, 2, 13);

                }*/

                g.FillRectangle(DimBrush, height + 10, 120, 120, 13);
                g.FillRectangle(Brushes.DarkRed, height + 10, 120, Convert.ToSingle(rFactor.Drivers.Player.Brake * 120), 13);
                g.FillRectangle(Brushes.DarkGreen, height + 10, 120, Convert.ToSingle(rFactor.Drivers.Player.Throttle* 120), 13);
                if (rFactor.Drivers.Player.Brake > rFactor.Drivers.Player.Throttle)
                {
                    g.DrawString(rFactor.Drivers.Player.Brake.ToString("000%"), f, Brushes.DarkRed,
                                 e.ClipRectangle.Width - 45, 119);
                }
                else
                {
                    g.DrawString(rFactor.Drivers.Player.Throttle.ToString("000%"), f, Brushes.DarkGreen,
                                 e.ClipRectangle.Width - 45, 119);
                }

                double fuel_state = rFactor.Drivers.Player.Fuel/rFactor.Drivers.Player.Fuel_Max;
                g.DrawString("FUEL", f, DimBrush, height - 50, 139);
                /*for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc > fuel_state)
                        g.FillRectangle(DimBrush, height + 10 + perc, 140, 2, 13);
                    else if (fuel_state < 120*0.1)
                        g.FillRectangle(Brushes.Red, height + 10 + perc, 140, 2, 13);
                    else
                        g.FillRectangle(Brushes.DarkOrange, height + 10 + perc, 140, 2, 13);

                }*/

                g.FillRectangle(DimBrush, height + 10 , 140, 120, 13);
                if (fuel_state > 0.1)
                {
                    g.FillRectangle(Brushes.Red, height + 10, 140, 12, 13);
                    g.FillRectangle(Brushes.DarkOrange, height + 22, 140, Convert.ToSingle(120 * (fuel_state - 0.1)), 13);
                }
                else
                {
                    g.FillRectangle(Brushes.Red, height + 10, 180, Convert.ToSingle(fuel_state * 120), 13);
                }
                if (fuel_state < 0.1)
                    g.DrawString(rFactor.Drivers.Player.Fuel.ToString("000L"), f, Brushes.Red,
                                 e.ClipRectangle.Width - 45, 139);
                else
                    g.DrawString(rFactor.Drivers.Player.Fuel.ToString("000L"), f, Brushes.DarkOrange,
                                 e.ClipRectangle.Width - 45, 139);

                if (FuelUsage.Count > 5)
                {
                    double avg = 0;
                    for (int i = FuelUsage.Count - 5; i < FuelUsage.Count; i++) avg += FuelUsage[i];
                    avg /= FuelUsage.Count - (FuelUsage.Count - 5);
                    if (avg > 0)
                    {
                        double laps = rFactor.Player.Fuel/avg;
                        g.DrawString(laps.ToString("(000)"), f, Brushes.DarkOrange,
                                     e.ClipRectangle.Width - 45, 159);
                        g.DrawString(avg.ToString("0.00L") + " per lap", f, Brushes.DarkOrange,
                                     height + 10, 159);
                    }
                }
                else

                    g.DrawString("(???)", f, DimBrush,
                                 e.ClipRectangle.Width - 45, 159);

                double engine_live = rFactor.Player.Engine_Lifetime_Live;
                double engine_perc = engine_live/Engine_Max;
                if (double.IsInfinity(engine_perc) || double.IsNaN(engine_perc)) engine_perc = 1;
                g.DrawString("ENGINE", f, DimBrush, height - 50, 179);
                /*for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc > engine_perc*120)
                        g.FillRectangle(DimBrush, height + 10 + perc, 180, 2, 13);
                    else if (engine_perc < 0.1)
                        g.FillRectangle(Brushes.Red, height + 10 + perc, 180, 2, 13);
                    else
                        g.FillRectangle(Brushes.DarkOrange, height + 10 + perc, 180, 2, 13);

                }*/


                g.FillRectangle(DimBrush, height + 10, 180, 120, 13);

                if (engine_perc > 0.1)
                {
                    g.FillRectangle(Brushes.Red, height + 10, 180, 12, 13);
                    g.FillRectangle(Brushes.DarkOrange, height + 22, 180, Convert.ToSingle(120 * (engine_perc - 0.1)), 13);
                }
                else
                {
                    g.FillRectangle(Brushes.Red, height + 10, 180, Convert.ToSingle(engine_perc*120), 13);
                }


                if (engine_perc < 0.1)
                        g.DrawString(engine_perc.ToString("000%"), f, Brushes.Red,
                                     e.ClipRectangle.Width - 45, 179);
                    else
                        g.DrawString(engine_perc.ToString("000%"), f,
                                     Brushes.DarkOrange,
                                     e.ClipRectangle.Width - 45,
                                     179)
                            ;

                if (Counter == 10)
                    g.DrawString("Refreshed!", f, Brushes.Yellow, e.ClipRectangle.Width - 75, 199);
                else
                {
                if (EngineUsage.Count > 5)
                {
                    double avg = 0;
                    for (int i = EngineUsage.Count-5; i < EngineUsage.Count; i++) avg += EngineUsage[i];
                    avg /= EngineUsage.Count - (EngineUsage.Count-5);
                    if (avg > 0)
                    {
                        double engine_laps = Engine_LastLap / avg;
                        g.DrawString(engine_laps.ToString("(000)"), f, Brushes.DarkOrange,
                                     e.ClipRectangle.Width - 45, 199);
                        g.DrawString(avg.ToString("0.00%") + " per lap", f, Brushes.DarkOrange,
                                      height +10, 199);
                    }
                }
                else

                    g.DrawString("(???)", f, DimBrush,
                                 e.ClipRectangle.Width - 45, 199);
                }

                double power = Computations.Get_Engine_CurrentHP();
                double power_max = Computations.Get_Engine_MaxHP();

                float power_factor = Convert.ToSingle(power/power_max);
                if (power_factor > 1f) power_factor = 1f;

                g.DrawString("POWER", f, DimBrush, height - 50, 219);
               /* for (int perc = -20; perc < 100; perc += 3)
                {
                    if (perc > power_factor * 100)
                        g.FillRectangle(DimBrush, height + 30 + perc, 220, 2, 13);
                    else
                        if (perc < 0)
                            g.FillRectangle(Brushes.YellowGreen, height +30 + perc, 220, 2, 13);
                        else
                        g.FillRectangle(Brushes.Yellow, height + 30 + perc, 220, 2, 13);

                }*/
                g.FillRectangle(Brushes.Yellow, height + 10, 220, power_factor * 120f, 13);
                g.DrawString((power).ToString("0000"), f, Brushes.Yellow, e.ClipRectangle.Width - 45, 220);
                
                Font sf = new Font("Arial", 8f);

                g.DrawString("Engine", sf, Brushes.White, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, DimBrush, this.Width -35, this.Height - 45);
                g.DrawString("Laptimes", sf, DimBrush, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf, DimBrush, this.Width - 30, this.Height - 15);

                TimeSpan dt = DateTime.Now.Subtract(lastrender);
                fps = fps*0.98 + 0.02*(1000/dt.TotalMilliseconds);
                g.DrawString(fps.ToString("00fps"), sf, Brushes.White, 0, 0);
                lastrender = DateTime.Now;
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 8f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace.Replace("in","\r\n"), f, Brushes.White, 10, 30);


            }
        }

        private double fps = 10;
        public static double Rads_RPM(double inp)
        {
            return inp / 2.0 / Math.PI * 60.0;
        }
    }
}