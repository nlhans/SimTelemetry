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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using Triton.Joysticks;
using Triton.Maths;
using Timer = System.Windows.Forms.Timer;

namespace LiveTelemetry
{
    public partial class Gauge_A1GP : UserControl
    {
        private List<double> FuelUsage = new List<double>();
        private List<double> EngineUsage = new List<double>();
        private Filter PowerUsage = new Filter(5);

        private bool RPM_AutoRange;

        // Properties of car
        private double RPM_Min;
        private double RPM_Max;
        private double Speed_Min;
        private double Speed_Max;
        private double Speed_Step;
        private double Power_Max;


        private double Engine_LastLap = 1;
        private double Engine_Max;
        private double Fuel_LastLap;
        private double Fuel_LastLapNo;

        private Joystick joy; 
        private Timer t = new Timer();
        private int Counter; // Timer used to reset engine wear.

        private Image _EmptyGauges;
        
        public Gauge_A1GP(Joystick j)
        {
            Counter = 0;
            if (j != null)
            {
                joy = j;
                j.Release += j_Release;
            }
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            t = new Timer{Interval=500, Enabled=true};
            t.Tick += t_Tick;
            t.Start();

            //Telemetry.m.Session_Start += PaintBackground;
            //Telemetry.m.Session_Stop += PaintBackground;
            //Telemetry.m.Driving_Start += PaintBackground;

            _EmptyGauges = new Bitmap(this.Size.Width, this.Size.Height);
        }

        #region Joystick Engine Wear control
        private void j_Release(Joystick joystick, int button)
        {
            if(Counter == 10 && button == LiveTelemetry.Joystick_Button)
            {
                Counter = 0;
                LiveTelemetry.StatusMenu = 0;
            }
        }
        private void t_Tick(object sender, EventArgs e)
        {
            if (joy != null && joy.GetButton(LiveTelemetry.Joystick_Button))
            {
                Counter++;
                if (Counter >= 4 && Counter != 10) // Timer for resetting engine wear.
                {
                    LiveTelemetry.StatusMenu = 0;
                    Engine_Max = TelemetryApplication.Telemetry.Player.EngineLifetime; //Telemetry.m.Sim.Player.Engine_Lifetime;
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
            if (!TelemetryApplication.TelemetryAvailable) return;
            if (TelemetryApplication.Telemetry.Player == null) return;
            if (TelemetryApplication.Car == null) return;
            try
            {
                // TOOD: Move back to Data libraries so they can be used without the UI.
                if (Fuel_LastLapNo != TelemetryApplication.Telemetry.Player.Laps)
                {
                    double engine_state = TelemetryApplication.Telemetry.Player.EngineLifetime/
                                          TelemetryApplication.Car.Engine.Lifetime.EngineRpm.Optimum;//avg

                    Fuel_LastLapNo = TelemetryApplication.Telemetry.Player.Laps;
                    double usedF = TelemetryApplication.Telemetry.Player.Fuel - Fuel_LastLap;
                    double usedE = Engine_LastLap - engine_state;
                    Engine_LastLap = engine_state;
                    Fuel_LastLap = TelemetryApplication.Telemetry.Player.Fuel;

                    if (usedF < 0)
                        FuelUsage.Add(0 - usedF);
                    if (usedE > 0 && usedE < 0.15)
                        EngineUsage.Add(usedE);


                }
            }catch(Exception ex)
            {
                //
            }
        }
        #endregion
        #region UI drawing code

        private void PaintBackground(object sender)
        {
            //System.Threading.Thread.Sleep(500);
            // Fonts
            Font font_arial_10 = new Font("Arial", 10f, FontStyle.Bold);

            Font gear_f = new Font("Arial", 30f);
            Font speed_f = new Font("Arial", 18f);
            Font dist_f = new Font("Arial", 12f);
            Font small_font = new Font("Arial", 8f);

            int border_bounds = 60;
            int width = this.Width - border_bounds;
            int height = this.Height - border_bounds;
            int clip_height = this.Height;
            int clip_width = this.Width;

            _EmptyGauges = new Bitmap(this.Size.Width, this.Size.Height);
            Graphics g = Graphics.FromImage(_EmptyGauges);
            lock (g)
            {
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
                if (!TelemetryApplication.TelemetryAvailable || !TelemetryApplication.CarAvailable) return;

                // ---------------------------------       Speed      ---------------------------------
                Speed_Min = 0;
                Speed_Max = 360; // TODO: Calculate top speed

                Speed_Step = 30;
                if (Speed_Max < 300) Speed_Step = 25;
                if (Speed_Max < 200) Speed_Step = 20;

                if (Speed_Max%Speed_Step > 0)
                    Speed_Max += Speed_Step - (Speed_Max%Speed_Step);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2, border_bounds/2, height, height, 90, 225);
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2 + 8, border_bounds/2 + 8, height - 16, height - 16, 90, 225);
                g.DrawArc(new Pen(Color.White, 2f), border_bounds/2 + 25, border_bounds/2 + 25, height - 50, height - 50, 90, 225);

                // ---------------------------------        RPM       ---------------------------------
                int retry = 10;
                RPM_AutoRange = false;
                do
                {
                    RPM_Max = 1000*Math.Ceiling(TelemetryApplication.Telemetry.Player.EngineRpmMax/1000);
                    Thread.Sleep(10);
                } while (RPM_Max < 100 && retry-- > 0);

                if(RPM_Max < 1000)
                {
                    RPM_AutoRange = true;
                    RPM_Max = Math.Max(1000 * Math.Ceiling(TelemetryApplication.Telemetry.Player.EngineRpm/ 1000), 6000);
                }

                RPM_Min = 0;

                double RPM_Step = 1000;

                // Ranges.
                if (RPM_Max >= 0 && RPM_Max <= 10000) RPM_Step = 1000;
                if (RPM_Max > 12000) RPM_Step = 2000;

                if (RPM_Max >= 12000 && RPM_Max < 20000)
                {
                    RPM_Step = 2000;
                    RPM_Min = RPM_Max - 6*2000;

                }
                if (RPM_Max >= 20000)
                {
                    RPM_Step = 2000;
                    RPM_Min = RPM_Max - 7*2000;

                }

                if (RPM_Min%RPM_Step != 0)
                    RPM_Min += RPM_Step - (RPM_Min%RPM_Step);

                if (RPM_Max%RPM_Step != 0)
                    RPM_Max += RPM_Step - (RPM_Max%RPM_Step);

                // ---------------------------------     RPM Gauge    ---------------------------------
                double fAngle_RPM_RedLine = (TelemetryApplication.Telemetry.Player.EngineRpmMax - RPM_Step / 2 - RPM_Min) /
                                            (RPM_Max - RPM_Min)*225;
                if (double.IsInfinity(fAngle_RPM_RedLine) || double.IsNaN(fAngle_RPM_RedLine)) fAngle_RPM_RedLine = 200;
                int Angle_RPM_RedLine = Convert.ToInt32(Math.Round(fAngle_RPM_RedLine));

                double fAngle_RPM_WarningLine = (TelemetryApplication.Car.Engine.Lifetime.EngineRpm.Optimum - RPM_Step / 2 -
                                                 RPM_Min)/(RPM_Max - RPM_Min)*225;
                if (double.IsInfinity(fAngle_RPM_WarningLine) || double.IsNaN(fAngle_RPM_WarningLine))
                    fAngle_RPM_WarningLine = 180;
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

                    double y_a = clip_height/2 + sin_a*70;
                    double x_a = clip_height/2 + cos_a*70;
                    double y_b = clip_height/2 + sin_a*80;
                    double x_b = clip_height/2 + cos_a*80;

                    double y_c = clip_height/2 + sin_a*95 - 8;
                    double x_c = clip_height/2 + cos_a*95 - 8;

                    g.DrawLine(new Pen(Color.White, 2f), Convert.ToSingle(x_a), Convert.ToSingle(y_a),
                               Convert.ToSingle(x_b), Convert.ToSingle(y_b));
                    double RPM = RPM_Min + (RPM_Max - RPM_Min)*(angle - 90)/225;
                    g.DrawString((RPM/1000.0).ToString("0"), font_arial_10, Brushes.White, Convert.ToSingle(x_c),
                                 Convert.ToSingle(y_c));

                    for (int i = 1; i < 5; i++)
                    {
                        double angle2 = angle + 225.0/((RPM_Max - RPM_Min)/RPM_Step)*i/5;
                        if (angle2 > 90 + 225) break;
                        double sin2_a = Math.Sin(angle2/180.0*Math.PI);
                        double cos2_a = Math.Cos(angle2/180.0*Math.PI);

                        double y2_a = clip_height/2 + sin2_a*72;
                        double x2_a = clip_height/2 + cos2_a*72;
                        double y2_b = clip_height/2 + sin2_a*78;
                        double x2_b = clip_height/2 + cos2_a*78;

                        if (angle2 > 90 + fAngle_RPM_RedLine)
                            g.DrawLine(new Pen(Color.Red, 1f), Convert.ToSingle(x2_a), Convert.ToSingle(y2_a),
                                       Convert.ToSingle(x2_b), Convert.ToSingle(y2_b));
                        else
                            g.DrawLine(new Pen(Color.White, 1f), Convert.ToSingle(x2_a), Convert.ToSingle(y2_a),
                                       Convert.ToSingle(x2_b), Convert.ToSingle(y2_b));

                    }
                    this.BackgroundImage = null;
                    this.BackgroundImage = _EmptyGauges;
                }

                for (double angle = 90; angle <= 90 + 225.1; angle += 225.0/((Speed_Max - Speed_Min)/Speed_Step))
                {

                    double sin_a = Math.Sin(angle/180.0*Math.PI);
                    double cos_a = Math.Cos(angle/180.0*Math.PI);

                    double y_a = clip_height/2 + sin_a*133 - 3;
                    double x_a = clip_height/2 + cos_a*133 - 3;

                    double y_c = clip_height/2 + sin_a*147 - 10;
                    double x_c = clip_height/2 + cos_a*147 - 15;

                    g.FillEllipse(Brushes.DarkRed, Convert.ToSingle(x_a), Convert.ToSingle(y_a), 6, 6);
                    g.DrawEllipse(new Pen(Color.White, 1f), Convert.ToSingle(x_a), Convert.ToSingle(y_a), 6, 6);

                    double Spd = Speed_Min + (Speed_Max - Speed_Min)*(angle - 90)/225;
                    g.DrawString(Spd.ToString("000"), font_arial_10, Brushes.White, Convert.ToSingle(x_c),
                                 Convert.ToSingle(y_c));

                }

                // Maximum power
                // TODO: Add maximum power
                Power_Max = 600; // Telemetry.m.Computations.MaximumPower(Telemetry.m.Sim, null);
                if (Power_Max <= 0 || Power_Max >= 1000)
                    Power_Max = 1000;

                // ---------------------------------    Labels   ---------------------------------
                SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

                // Throttle/brake
                g.DrawString("PEDALS", font_arial_10, DimBrush, height - 50, 119);
                g.FillRectangle(DimBrush, height + 10, 120, 120, 13);

                // Fuel remaining / estimated laps.
                g.DrawString("FUEL", font_arial_10, DimBrush, height - 50, 139);
                g.FillRectangle(DimBrush, height + 10, 140, 120, 13);

                // Engine wear / laps estimation
                g.DrawString("ENGINE", font_arial_10, DimBrush, height - 50, 179);
                g.FillRectangle(DimBrush, height + 10, 180, 120, 13);

                // Power bar.
                g.DrawString("POWER", font_arial_10, DimBrush, height - 50, 219);

                // ---------------------------------   Menu Labels   ---------------------------------
                g.DrawString("Engine", small_font, Brushes.White, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", small_font, DimBrush, this.Width - 35, this.Height - 45);
                g.DrawString("Laptimes", small_font, DimBrush, this.Width - 51, this.Height - 30);
                g.DrawString("Split", small_font, DimBrush, this.Width - 30, this.Height - 15);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            // Fonts
            Font font_arial_10 = new Font("Arial", 10f, FontStyle.Bold);

            Font gear_f = new Font("Arial", 30f);
            Font speed_f = new Font("Arial", 18f);
            Font dist_f = new Font("Arial", 12f);
            Font small_font = new Font("Arial", 8f);
            try
            {
                int border_bounds = 60;
                int width = this.Width;
                int height = this.Height;
                width -= border_bounds;
                height -= border_bounds;
                int clip_height = this.Height;
                int clip_width = this.Width;

                Graphics g = e.Graphics;

                
                //g.DrawImage(_EmptyGauges,0,0);
                if (!TelemetryApplication.TelemetryAvailable || !TelemetryApplication.CarAvailable) return;
                lock (g)
                {

                    // ---------------------------------      RPM Needle    ---------------------------------
                    double RPMLive = TelemetryApplication.Telemetry.Player.EngineRpm;
                    double fAngle_RPM = 90 + (RPMLive - RPM_Min)/(RPM_Max - RPM_Min)*225;
                    if (fAngle_RPM < 90) fAngle_RPM = 90;
                    if (fAngle_RPM > 90 + 225) fAngle_RPM = 90 + 225;
                    double rpm_gauge_sin = Math.Sin(fAngle_RPM/180.0*Math.PI);
                    double rpm_gauge_cos = Math.Cos(fAngle_RPM/180.0*Math.PI);

                    double rpm_gauge_ya = clip_height/2 + rpm_gauge_sin*-15;
                    double rpm_gauge_xa = clip_height/2 + rpm_gauge_cos*-15;
                    double rpm_gauge_yb = clip_height/2 + rpm_gauge_sin*80;
                    double rpm_gauge_xb = clip_height/2 + rpm_gauge_cos*80;

                    if (RPM_AutoRange)
                    {
                        if (RPMLive > RPM_Max)
                        {
                            RPM_Max += 1000;
                            PaintBackground(null);
                        }

                    }

                    g.DrawLine(new Pen(Color.DarkRed, 4f), Convert.ToSingle(rpm_gauge_xa),
                               Convert.ToSingle(rpm_gauge_ya),
                               Convert.ToSingle(rpm_gauge_xb), Convert.ToSingle(rpm_gauge_yb));

                    // ---------------------------------     Speed Needle    ---------------------------------
                    double SpeedLive = TelemetryApplication.Telemetry.Player.Speed* 3.6;

                    double fAngle_Speed = 90 + (SpeedLive - Speed_Min)/(Speed_Max - Speed_Min)*225;
                    if (fAngle_Speed < 90) fAngle_Speed = 90;
                    if (fAngle_Speed > 90 + 225) fAngle_Speed = 90 + 225;

                    double Speed_gauge_sin = Math.Sin(fAngle_Speed/180.0*Math.PI);
                    double Speed_gauge_cos = Math.Cos(fAngle_Speed/180.0*Math.PI);

                    double Speed_gauge_ya = clip_height/2 + Speed_gauge_sin*-20;
                    double Speed_gauge_xa = clip_height/2 + Speed_gauge_cos*-20;
                    double Speed_gauge_yb = clip_height/2 + Speed_gauge_sin*133;
                    double Speed_gauge_xb = clip_height/2 + Speed_gauge_cos*133;

                    g.DrawLine(new Pen(Color.DarkGreen, 6f), Convert.ToSingle(Speed_gauge_xa),
                               Convert.ToSingle(Speed_gauge_ya), Convert.ToSingle(Speed_gauge_xb),
                               Convert.ToSingle(Speed_gauge_yb));
                    g.FillEllipse(Brushes.DarkGreen, clip_height/2 - 10, clip_height/2 - 10, 20, 20);

                    /* Bars filling up the speed bar proved  CPU intensive */
                    /*for (double angle = 90; angle <= fAngle_Speed; angle += 3.5)
                    {
                        double Spd = Speed_Min+(Speed_Max-Speed_Min)*(angle - 90)/225;
                        if (Spd > SpeedLive || Math.Abs(Spd - SpeedLive) < 0.5) break;
                        double le = 3;
                        if (angle + 3 > fAngle_Speed) le = fAngle_Speed - angle;
                        Color clr = Color.FromArgb(Convert.ToInt32(100 + Spd*155/Speed_Max),
                                                   Convert.ToInt32(255 - Spd*255/Speed_Max), 0);
                        g.DrawArc(new Pen(clr, 4f), Convert.ToSingle(border_bounds/2 + 4),
                                  Convert.ToSingle(border_bounds/2 + 4), Convert.ToSingle(height - 8),
                                  Convert.ToSingle(height - 8), Convert.ToSingle(angle), Convert.ToSingle(le));
                    }*/


                    // ---------------------------------    Gear/Speed    ---------------------------------

                    if (TelemetryApplication.Telemetry.Player.Gear == -1 || TelemetryApplication.Telemetry.Player.Gear == 0xFF)
                        g.DrawString("R", gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                     border_bounds/2 + height/2 + 40);
                    else if (TelemetryApplication.Telemetry.Player.Gear > 0)
                        g.DrawString(TelemetryApplication.Telemetry.Player.Gear.ToString(), gear_f, Brushes.White,
                                     border_bounds/2 + height/2 + 5,
                                     border_bounds/2 + height/2 + 40);
                    else if (TelemetryApplication.Telemetry.Player.Gear == 0)
                        g.DrawString("N", gear_f, Brushes.White, border_bounds/2 + height/2 + 5,
                                     border_bounds/2 + height/2 + 40);

                    if (TelemetryApplication.Telemetry.Player.IsLimiter)
                        g.DrawString(Math.Abs(TelemetryApplication.Telemetry.Player.Speed* 3.6).ToString("000") + "km/h", speed_f,
                                     Brushes.DarkOrange,
                                     border_bounds/2 + height/2 + 10, border_bounds/2 + height/2 + 80);
                    else
                        g.DrawString(Math.Abs(TelemetryApplication.Telemetry.Player.Speed * 3.6).ToString("000") + "km/h", speed_f,
                                     Brushes.White,
                                     border_bounds/2 + height/2 + 10, border_bounds/2 + height/2 + 80);

                    // TODO: Add odo-meter
                    g.DrawString(Math.Round(0.0, 2).ToString("00000.00km"), dist_f,
                                 Brushes.LightGray, border_bounds/2 + height/2 + 10, border_bounds/2 + height/2 + 110);

                    // ---------------------------------    Labels   ---------------------------------

                    // Throttle
                    g.FillRectangle(Brushes.DarkRed, height + 10, 120,
                                    Convert.ToSingle(TelemetryApplication.Telemetry.Player.InputBrake* 120), 13);
                    g.FillRectangle(Brushes.DarkGreen, height + 10, 120,
                                    Convert.ToSingle(TelemetryApplication.Telemetry.Player.InputThrottle* 120), 13);
                    if (TelemetryApplication.Telemetry.Player.InputBrake > TelemetryApplication.Telemetry.Player.InputThrottle)
                    {
                        g.DrawString(TelemetryApplication.Telemetry.Player.InputBrake.ToString("000%"), font_arial_10,
                                     Brushes.DarkRed,
                                     width + border_bounds - 45, 119);
                    }
                    else
                    {
                        g.DrawString(TelemetryApplication.Telemetry.Player.InputThrottle.ToString("000%"), font_arial_10,
                                     Brushes.DarkGreen,
                                     width + border_bounds - 45, 119);
                    }

                    // Fuel
                    double fuel_state = 0;
                    // TODO: Fix bug where red bar doesn't decrease.
                    if (TelemetryApplication.Car == null || TelemetryApplication.Car.Chassis.FuelTankSize == 0)
                        fuel_state = TelemetryApplication.Telemetry.Player.Fuel/ 100.0;
                    else
                        fuel_state = TelemetryApplication.Telemetry.Player.Fuel / TelemetryApplication.Car.Chassis.FuelTankSize;

                    if (fuel_state > 0.1)
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 140, 12, 13);
                        g.FillRectangle(Brushes.DarkOrange, height + 22, 140, Convert.ToSingle(120*(fuel_state - 0.1)),
                                        13);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 180, Convert.ToSingle(fuel_state*120), 13);
                    }
                    if (fuel_state < 0.1)
                        g.DrawString(TelemetryApplication.Telemetry.Player.Fuel.ToString("00.0").Replace(".", "L"),
                                     font_arial_10, Brushes.Red,
                                     width + border_bounds - 45, 139);
                    else
                        g.DrawString(TelemetryApplication.Telemetry.Player.Fuel.ToString("000.0").Replace(".", "L"),
                                     font_arial_10, Brushes.DarkOrange,
                                     width + border_bounds - 45, 139);

                    // Laps estimation.
                    SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

                    if (FuelUsage.Count > 5)
                    {
                        double avg = 0;
                        for (int i = FuelUsage.Count - 5; i < FuelUsage.Count; i++) avg += FuelUsage[i];
                        avg /= FuelUsage.Count - (FuelUsage.Count - 5);
                        if (avg > 0)
                        {
                            double laps = TelemetryApplication.Telemetry.Player.Fuel / avg;
                            g.DrawString(laps.ToString("(000)"), font_arial_10, Brushes.DarkOrange,
                                         width + border_bounds - 45, 159);
                            g.DrawString(avg.ToString("0.00L") + " per lap", font_arial_10, Brushes.DarkOrange,
                                         height + 10, 159);
                        }
                    }
                    else
                        g.DrawString("(???)", font_arial_10, DimBrush, width + border_bounds - 45, 159);

                    // Engine
                    double engine_live = TelemetryApplication.Telemetry.Player.EngineLifetime;
                    double engine_perc = engine_live/Engine_Max;
                    if (double.IsInfinity(engine_perc) || double.IsNaN(engine_perc)) engine_perc = 1;

                    if (engine_perc > 0.1)
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 180, 12, 13);
                        g.FillRectangle(Brushes.DarkOrange, height + 22, 180, Convert.ToSingle(120*(engine_perc - 0.1)),
                                        13);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 180, Convert.ToSingle(engine_perc*120), 13);
                    }
                    if (engine_perc < 0.4)
                        g.DrawString((100*engine_perc).ToString("00.0").Replace(".", "%"), font_arial_10, Brushes.Red,
                                     width + border_bounds - 45, 179);
                    else
                        g.DrawString((100*engine_perc).ToString("000.0").Replace(".", "%"), font_arial_10,
                                     Brushes.DarkOrange, width + border_bounds - 45, 179);


                    // Laps estimation.
                    if (Counter == 10)
                        g.DrawString("Refreshed!", font_arial_10, Brushes.Yellow, width + border_bounds - 75, 199);
                    else
                    {
                        if (EngineUsage.Count > 5)
                        {
                            double avg = 0;
                            for (int i = EngineUsage.Count - 5; i < EngineUsage.Count; i++) avg += EngineUsage[i];
                            avg /= EngineUsage.Count - (EngineUsage.Count - 5);
                            if (avg > 0)
                            {
                                double engine_laps = Engine_LastLap/avg;
                                g.DrawString(engine_laps.ToString("(000)"), font_arial_10, Brushes.DarkOrange,
                                             width + border_bounds - 45, 199);
                                g.DrawString(avg.ToString("0.00%") + " per lap", font_arial_10, Brushes.DarkOrange,
                                             height + 10, 199);
                            }
                        }
                        else

                            g.DrawString("(???)", font_arial_10, DimBrush,
                                         width + border_bounds - 45, 199);
                    }

                    // Power
                    double power;

                    if (true) //Telemetry.m.Sim.Modules.Engine_Power)
                        power = TelemetryApplication.Telemetry.Player.EngineTorque*
                                TelemetryApplication.Telemetry.Player.EngineRpm*Math.PI*2/60000.0;
                    else
                        power = 0; // Telemetry.m.Computations.GetPower(Telemetry.m.Sim.Player.Engine_RPM, Telemetry.m.Sim.Player.Pedals_Throttle);
                    
                    // TODO: Add power module

                    power = power; // Power.KW_HP(power);
                    PowerUsage.Add(power);
                    PowerUsage.MaxSize = 3;

                    float power_factor = Convert.ToSingle(power/Power_Max);
                    if (power_factor > 1f) power_factor = 1f;
                    if (power_factor < 0f) power_factor = 0f;

                    g.FillRectangle(Brushes.Yellow, height + 10, 220, power_factor*120f, 13);
                    g.DrawString((power).ToString("0000"), font_arial_10, Brushes.Yellow, width + border_bounds - 45, 220);

                    // Todo: Add fast-lap split counter.
                    //g.DrawString(Splits.Split.ToString("0.000"), font_arial_10, Brushes.Red, 0, 10);
                }
            }
            catch (Exception ex)
            {
                // ERROR
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                g.DrawString(ex.Message, small_font, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace.Replace("in", "\r\n"), small_font, Brushes.White, 10, 30);


            }
        }
        #endregion
    }
}