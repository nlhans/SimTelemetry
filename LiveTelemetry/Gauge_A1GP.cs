using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Objects;
using Triton.Joysticks;
using Triton.Maths;

namespace LiveTelemetry
{
    public partial class Gauge_A1GP : UserControl
    {
        private List<double> FuelUsage = new List<double>();
        private List<double> EngineUsage = new List<double>();
        private Filter PowerUsage = new Filter(5);

        private double Engine_LastLap = 1;
        private double Engine_Max;
        private double Fuel_LastLap = 0;
        private double Fuel_LastLapNo;
        private double Engine_Shortterm = 0;

        private Joystick joy; 
        private Timer t = new Timer();
        private int Counter = 0; // Timer used to reset engine wear.
        private double fps = 10;
        private DateTime lastrender = DateTime.Now; // FPS counter.

        // Color scheme settings:
        private Color DimColor = Color.FromArgb(70, 70, 70);
        private SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        
        public Gauge_A1GP(Joystick j)
        {
            if (j != null)
            {
                joy = j;
                j.Release += new JoystickButtonEvent(j_Release);
            }
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            t = new Timer{Interval=500, Enabled=true};
            t.Tick += t_Tick;
            t.Start();
        }

        #region Joystick Engine Wear control
        private void j_Release(Joystick joystick, int button)
        {
            if(Counter == 10 && button == LiveTelemetry.TheButton)
            {
                Counter = 0;
                LiveTelemetry.StatusMenu = 0;
            }
        }
        private void t_Tick(object sender, EventArgs e)
        {
            if (joy != null && joy.GetButton(LiveTelemetry.TheButton))
            {
                Counter++;
                if (Counter >= 4 && Counter != 10) // Timer for resetting engine wear.
                {
                    LiveTelemetry.StatusMenu = 0;
                    Engine_Max = Telemetry.m.Sim.Player.Engine_Lifetime_Live;
                    Counter = 10;
                }
            }
            else
                Counter = 0;
        }
        #endregion
        #region Fuel/engine wear usage timer.
        public void Update()
        {
            if (!Telemetry.m.Active_Session) return;

            // TOOD: Move back to Data libraries so they can be used without the UI.
            if(Fuel_LastLapNo != Telemetry.m.Sim.Drivers.Player.Laps)
            {
                double engine_state = Telemetry.m.Sim.Player.Engine_Lifetime_Live / Telemetry.m.Sim.Player.Engine_Lifetime_Typical;

                Fuel_LastLapNo = Telemetry.m.Sim.Drivers.Player.Laps;
                double usedF = Telemetry.m.Sim.Drivers.Player.Fuel - Fuel_LastLap;
                double usedE = Engine_LastLap-engine_state;
                Engine_LastLap = engine_state;
                Fuel_LastLap = Telemetry.m.Sim.Drivers.Player.Fuel;

                if (usedF < 0)
                    FuelUsage.Add(0 - usedF);
                if(usedE > 0 && usedE < 0.15)
                    EngineUsage.Add(usedE);


            }

        }
        #endregion
        #region UI drawing code
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                int border_bounds = 60;
                int width = this.Width;
                int height = this.Height;
                width -= border_bounds;
                height -= border_bounds;
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, 0,0,this.Width, this.Height);
                if (!Telemetry.m.Active_Session) return;




                // ---------------------------------       Speed      ---------------------------------
                double SpeedTop;
                if (Telemetry.m.Sim.Modules.Aero_Drag_Cw == false || Telemetry.m.Sim.Modules.Engine_Power == false)
                    SpeedTop = 400; // Considered as a typical topspeed for most driving simulators..
                else
                    SpeedTop = 400; // TODO: Add interface to simulators to calculate this.
                
                if (double.IsNaN(SpeedTop) || double.IsInfinity(SpeedTop))
                    SpeedTop = 400;

                double SpeedMax = SpeedTop;
                double SpeedMin = 0;
                double SpeedStep = 30;

                if (SpeedMax < 300) SpeedStep = 25;
                if (SpeedMax < 200) SpeedStep = 20;

                if (SpeedMax%SpeedStep > 0)
                    SpeedMax += SpeedStep - (SpeedMax%SpeedStep);

                double player_spd = Math.Max(Telemetry.m.Sim.Player.Speed, Telemetry.m.Sim.Drivers.Player.Speed);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2, border_bounds/2, height, height, 90, 225);
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2 + 8, border_bounds/2 + 8, height - 16, height - 16,
                          90, 225);
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2 + 25, border_bounds/2 + 25, height - 50, height - 50,
                          90, 225);

                // ---------------------------------        RPM       ---------------------------------
                double RPM_Max = 1000*Math.Ceiling(Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Max_Live)/1000);
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
                double rpm_idle = Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Idle_Max);
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

                // ---------------------------------     RPM Gauge    ---------------------------------
                System.Drawing.Font f = new Font("Arial", 10f, FontStyle.Bold);
                
                double fAngle_RPM_RedLine = (Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM_Max_Live) - RPM_Step/2 - RPM_Min)/
                                            (RPM_Max - RPM_Min)*225;
                if (double.IsInfinity(fAngle_RPM_RedLine) || double.IsNaN(fAngle_RPM_RedLine)) fAngle_RPM_RedLine = 200;
                int Angle_RPM_RedLine = Convert.ToInt32(Math.Round(fAngle_RPM_RedLine));


                double fAngle_RPM_WarningLine = (Rads_RPM(Telemetry.m.Sim.Player.Engine_Lifetime_RPM_Base) - RPM_Step/2 -
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
                if (SpeedMax > 500) SpeedMax = 500;
                if (SpeedMax < 120) SpeedMax = 120;
                for (double angle = 90; angle <= 90 + 225.1; angle += 225.0/((SpeedMax-SpeedMin)/SpeedStep))
                {

                    double sin_a = Math.Sin(angle/180.0*Math.PI);
                    double cos_a = Math.Cos(angle/180.0*Math.PI);

                    double y_a = e.ClipRectangle.Height/2 + sin_a*133 - 3;
                    double x_a = e.ClipRectangle.Height/2 + cos_a*133 - 3;

                    double y_c = e.ClipRectangle.Height/2 + sin_a*147 - 10;
                    double x_c = e.ClipRectangle.Height/2 + cos_a*147 - 15;

                    g.FillEllipse(Brushes.DarkRed, Convert.ToSingle(x_a), Convert.ToSingle(y_a), 6, 6);
                    g.DrawEllipse(new Pen(Color.White, 1f), Convert.ToSingle(x_a), Convert.ToSingle(y_a), 6, 6);

                    double Spd = SpeedMin+(SpeedMax-SpeedMin)*(angle - 90)/225;
                    g.DrawString(Spd.ToString("000"), f, Brushes.White, Convert.ToSingle(x_c),
                                 Convert.ToSingle(y_c));

                }

                // ---------------------------------      RPM Needle    ---------------------------------
                double RPMLive = Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM);
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

                // ---------------------------------      Speed bar     ---------------------------------
                double SpeedLive = Telemetry.m.Sim.Player.Speed*3.6;
                if (SpeedLive > SpeedTop) SpeedLive = SpeedTop + 2;
                double fAngle_Speed = 90 + (SpeedLive-SpeedMin)/(SpeedMax-SpeedMin)*225;
                /* Bars filling up the speed bar proved too CPU intensive
                for (double angle = 90; angle <= fAngle_Speed; angle += 3.5)
                {
                    double Spd = SpeedMin+(SpeedMax-SpeedMin)*(angle - 90)/225;
                    if (Spd > SpeedLive || Math.Abs(Spd - SpeedLive) < 0.5) break;
                    double le = 3;
                    if (angle + 3 > fAngle_Speed) le = fAngle_Speed - angle;
                    Color clr = Color.FromArgb(Convert.ToInt32(100 + Spd*155/SpeedMax),
                                               Convert.ToInt32(255 - Spd*255/SpeedMax), 0);
                    /g.DrawArc(new Pen(clr, 4f), Convert.ToSingle(border_bounds/2 + 4),
                              Convert.ToSingle(border_bounds/2 + 4), Convert.ToSingle(height - 8),
                              Convert.ToSingle(height - 8), Convert.ToSingle(angle), Convert.ToSingle(le));
                }*/


                // ---------------------------------     Speed Needle    ---------------------------------
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


                // ---------------------------------     Water/Oil     ---------------------------------
                //g.DrawArc(new Pen(Color.White, 2f), height + 40, 30,80, 80, 180, 180);
                /*double Max_oil = Telemetry.m.Sim.Player.Engine_Lifetime_Oil_Base;
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
                double OilTempLive = Telemetry.m.Sim.Player.Engine_Temperature_Oil;
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
                double WaterTempLive = Telemetry.m.Sim.Player.Engine_Temperature_Water;
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


                // ---------------------------------    Gear/Speed    ---------------------------------
                System.Drawing.Font gear_f = new Font("Arial", 30f);
                System.Drawing.Font speed_f = new Font("Arial", 18f);

                if (Telemetry.m.Sim.Player.Gear == -1 || Telemetry.m.Sim.Player.Gear == 0xFF)
                    g.DrawString("R", gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                 border_bounds/2 + height/2 + 40);
                else if (Telemetry.m.Sim.Player.Gear > 0)
                    g.DrawString(Telemetry.m.Sim.Player.Gear.ToString(), gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                 border_bounds/2 + height/2 + 40);
                else if (Telemetry.m.Sim.Player.Gear == 0)
                    g.DrawString("N", gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                 border_bounds/2 + height/2 + 40);

                if (Telemetry.m.Sim.Drivers.Player.PitLimiter)
                    g.DrawString(Math.Abs(Telemetry.m.Sim.Player.Speed * 3.6).ToString("000") + "km/h", speed_f, Brushes.DarkOrange,
                                 border_bounds / 2 + height / 2 + 10, border_bounds / 2 + height / 2 + 80);
                else
                g.DrawString(Math.Abs(Telemetry.m.Sim.Player.Speed*3.6).ToString("000") + "km/h", speed_f, Brushes.White,
                             border_bounds/2 + height/2 + 10, border_bounds/2 + height/2 + 80);

                // ---------------------------------    Labels   ---------------------------------

                // Throttle/brake
                g.DrawString("PEDALS", f, DimBrush, height - 50, 119);
                g.FillRectangle(DimBrush, height + 10, 120, 120, 13);
                g.FillRectangle(Brushes.DarkRed, height + 10, 120, Convert.ToSingle(Telemetry.m.Sim.Drivers.Player.Brake * 120), 13);
                g.FillRectangle(Brushes.DarkGreen, height + 10, 120, Convert.ToSingle(Telemetry.m.Sim.Drivers.Player.Throttle* 120), 13);
                if (Telemetry.m.Sim.Drivers.Player.Brake > Telemetry.m.Sim.Drivers.Player.Throttle)
                {
                    g.DrawString(Telemetry.m.Sim.Drivers.Player.Brake.ToString("000%"), f, Brushes.DarkRed,
                                 width+border_bounds - 45, 119);
                }
                else
                {
                    g.DrawString(Telemetry.m.Sim.Drivers.Player.Throttle.ToString("000%"), f, Brushes.DarkGreen,
                                 width + border_bounds - 45, 119);
                }
                
                // Fuel remaining / estimated laps.
                // TODO: Fix bug where red bar doesn't decrease.
                double fuel_state = Telemetry.m.Sim.Drivers.Player.Fuel/Telemetry.m.Sim.Drivers.Player.Fuel_Max;
                g.DrawString("FUEL", f, DimBrush, height - 50, 139);
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
                    g.DrawString(Telemetry.m.Sim.Drivers.Player.Fuel.ToString("00.0").Replace(".","L"), f, Brushes.Red,
                                 width + border_bounds - 45, 139);
                else
                    g.DrawString(Telemetry.m.Sim.Drivers.Player.Fuel.ToString("000.0").Replace(".", "L"), f, Brushes.DarkOrange,
                                 width + border_bounds - 45, 139);

                // Laps estimation.
                if (FuelUsage.Count > 5)
                {
                    double avg = 0;
                    for (int i = FuelUsage.Count - 5; i < FuelUsage.Count; i++) avg += FuelUsage[i];
                    avg /= FuelUsage.Count - (FuelUsage.Count - 5);
                    if (avg > 0)
                    {
                        double laps = Telemetry.m.Sim.Player.Fuel/avg;
                        g.DrawString(laps.ToString("(000)"), f, Brushes.DarkOrange, width+border_bounds - 45, 159);
                        g.DrawString(avg.ToString("0.00L") + " per lap", f, Brushes.DarkOrange, height + 10, 159);
                    }
                }
                else
                    g.DrawString("(???)", f, DimBrush, width + border_bounds - 45, 159);

                // Engine wear / laps estimation
                double engine_live = Telemetry.m.Sim.Player.Engine_Lifetime_Live;
                double engine_perc = engine_live/Engine_Max;
                if (double.IsInfinity(engine_perc) || double.IsNaN(engine_perc)) engine_perc = 1;
                g.DrawString("ENGINE", f, DimBrush, height - 50, 179);
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


                if (engine_perc < 0.4)
                    g.DrawString((100 * engine_perc).ToString("00.0").Replace(".", "%"), f, Brushes.Red, width + border_bounds - 45, 179);
                else
                    g.DrawString((100 * engine_perc).ToString("000.0").Replace(".", "%"), f, Brushes.DarkOrange, width + border_bounds - 45, 179);

                // Laps estimation.
                if (Counter == 10)
                    g.DrawString("Refreshed!", f, Brushes.Yellow, width + border_bounds - 75, 199);
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
                                     width + border_bounds - 45, 199);
                        g.DrawString(avg.ToString("0.00%") + " per lap", f, Brushes.DarkOrange,
                                      height +10, 199);
                    }
                }
                else

                    g.DrawString("(???)", f, DimBrush,
                                 width + border_bounds - 45, 199);
                }

                // Power bar.
                double power, power_max;

                if (Telemetry.m.Sim.Modules.Engine_Power)
                    power = Telemetry.m.Sim.Player.Engine_Torque * Rotations.Rads_RPM(Telemetry.m.Sim.Player.Engine_RPM )/ 5252;
                else power = 0;

                PowerUsage.Add(power);
                PowerUsage.MaxSize = 5;
                power = PowerUsage.Average;

                if (Telemetry.m.Sim.Modules.Engine_PowerCurve)
                    power_max = 700;
                else
                    power_max = 1000;// TODO: Add minimum/maximum power plug-in support.

                float power_factor = Convert.ToSingle(power/power_max);
                if (power_factor > 1f) power_factor = 1f;

                g.DrawString("POWER", f, DimBrush, height - 50, 219);
                g.FillRectangle(Brushes.Yellow, height + 10, 220, power_factor * 120f, 13);
                g.DrawString((power).ToString("0000"), f, Brushes.Yellow, width+border_bounds - 45, 220);
                
                Font sf = new Font("Arial", 8f);

                // ---------------------------------   Menu Labels   ---------------------------------
                g.DrawString("Engine", sf, Brushes.White, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, DimBrush, this.Width -35, this.Height - 45);
                g.DrawString("Laptimes", sf, DimBrush, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf, DimBrush, this.Width - 30, this.Height - 15);


                // ---------------------------------      FPS     ---------------------------------
                TimeSpan dt = DateTime.Now.Subtract(lastrender);
                fps = fps*0.7 + 0.3*(1000/dt.TotalMilliseconds);
                if (double.IsInfinity(fps) || double.IsNaN(fps))
                    fps = 0;
                g.DrawString(fps.ToString("00fps"), sf, Brushes.White, 0, 0);
                lastrender = DateTime.Now;
            }
            catch (Exception ex)
            {
                // ERROR
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 8f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace.Replace("in","\r\n"), f, Brushes.White, 10, 30);


            }
        }

        private static double Rads_RPM(double inp)
        {
            return inp / 2.0 / Math.PI * 60.0;
        }
        #endregion
    }
}