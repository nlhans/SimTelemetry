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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using Triton.Joysticks;
using Triton.Maths;
using Timer = System.Windows.Forms.Timer;

namespace LiveTelemetry.Gauges
{
    public partial class UcGaugeA1Gp : UserControl
    {
        private readonly List<double> _fuelUsage = new List<double>();
        private readonly List<double> _engineUsage = new List<double>();
        private readonly Filter _powerUsage = new Filter(5);

        private bool _rpmAutoRange;
        private int borderBounds = 60;

        private int rpmGaugeRadiusPenalty = 42;
        private int rpmGaugeRadius = 85;

        
        // Properties of car
        private double _rpmMin;
        private double _rpmMax;
        private double _speedMin;
        private double _speedMax;
        private double _speedStep;
        private double _powerMax;


        private double _engineLastLap = 1;
        private double _engineMax;
        private double _fuelLastLap;
        private double _fuelLastLapNo;

        private Joystick joystickResetConsumptionStats; 
        private Timer tmrUpdateConsumptionStats = new Timer();
        private int _counter; // Timer used to reset engine wear.

        private Image _emptyGauges;
        
        public UcGaugeA1Gp(Joystick joystickObject)
        {
            _counter = 0;
            if (joystickObject != null)
            {
                joystickResetConsumptionStats = joystickObject;
                joystickObject.Release += j_Release;
            }
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            tmrUpdateConsumptionStats = new Timer{Interval=500, Enabled=true};
            tmrUpdateConsumptionStats.Tick += TmrUpdateConsumptionStatsTick;
            tmrUpdateConsumptionStats.Start();

            //GlobalEvents.Hook<SessionStarted>(x => PaintBackground(0), true);
            //GlobalEvents.Hook<SessionStopped>(x => PaintBackground(0), true);
            //GlobalEvents.Hook<DrivingStarted>(x => PaintBackground(0), true);

            GlobalEvents.Hook<CarLoaded>(PaintBackground, true);
            GlobalEvents.Hook<CarUnloaded>(PaintBackground, true);

            _emptyGauges = new Bitmap(Size.Width, Size.Height);
        }

        #region Joystick Engine Wear control
        private void j_Release(Joystick joystick, int button)
        {
            if(_counter == 10 && button == frmLiveTelemetry.Joystick_Button)
            {
                _counter = 0;
                frmLiveTelemetry.StatusMenu = 0;
            }
        }
        private void TmrUpdateConsumptionStatsTick(object sender, EventArgs e)
        {
            if (joystickResetConsumptionStats != null && joystickResetConsumptionStats.GetButton(frmLiveTelemetry.Joystick_Button))
            {
                _counter++;
                if (_counter >= 4 && _counter != 10) // Timer for resetting engine wear.
                {
                    frmLiveTelemetry.StatusMenu = 0;
                    _engineMax = TelemetryApplication.Telemetry.Player.EngineLifetime; //Telemetry.m.Sim.Player.Engine_Lifetime;
                    _counter = 10;
                }
            }
            else
                _counter = 0;
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
                if (_fuelLastLapNo != TelemetryApplication.Telemetry.Player.Laps)
                {
                    var engineState = TelemetryApplication.Telemetry.Player.EngineLifetime/
                                          TelemetryApplication.Car.Engine.Lifetime.EngineRpm.Optimum;//avg

                    _fuelLastLapNo = TelemetryApplication.Telemetry.Player.Laps;

                    var usedFuelPerc = TelemetryApplication.Telemetry.Player.Fuel - _fuelLastLap;
                    var usedEnginePerc = _engineLastLap - engineState;

                    _engineLastLap = engineState;
                    _fuelLastLap = TelemetryApplication.Telemetry.Player.Fuel;

                    if (usedFuelPerc < 0)
                        _fuelUsage.Add(0 - usedFuelPerc);
                    if (usedEnginePerc > 0 && usedEnginePerc < 0.15)
                        _engineUsage.Add(usedEnginePerc);


                }
            }
            catch
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
            var fontArial10Bold = new Font("Arial", 10f, FontStyle.Bold);
            var fontArial8 = new Font("Arial", 8f);

            var height = Height - borderBounds;
            var clipHeight = Height;

            var backgroundGauges = new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(backgroundGauges);

            lock (g)
            {
                g.FillRectangle(Brushes.Black, 0, 0, Width, Height);
                if (!TelemetryApplication.TelemetryAvailable || !TelemetryApplication.CarAvailable) return;
                if (TelemetryApplication.Telemetry.Player == null) return;

                // ---------------------------------       Speed      ---------------------------------
                _speedMin = 0;
                _speedMax = 360; // TODO: Calculate top speed

                _speedStep = 30;
                if (_speedMax < 300) _speedStep = 25;
                if (_speedMax < 200) _speedStep = 20;

                if (_speedMax%_speedStep > 0)
                    _speedMax += _speedStep - (_speedMax%_speedStep);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawArc(new Pen(Color.White, 2f), borderBounds/2, borderBounds/2, height, height, 90, 225);
                g.DrawArc(new Pen(Color.White, 2f), borderBounds / 2 + 8, borderBounds / 2 + 8, height - 16, height - 16, 90, 225);

                for (double angle = 90; angle <= 90 + 225.1; angle += 225.0 / ((_speedMax - _speedMin) / _speedStep))
                {

                    double sinA = Math.Sin(angle / 180.0 * Math.PI);
                    double cosA = Math.Cos(angle / 180.0 * Math.PI);

                    double yA = clipHeight / 2 + sinA * 133 - 3;
                    double xA = clipHeight / 2 + cosA * 133 - 3;

                    double yC = clipHeight / 2 + sinA * 147 - 10;
                    double xC = clipHeight / 2 + cosA * 147 - 15;

                    g.FillEllipse(Brushes.DarkRed, Convert.ToSingle(xA), Convert.ToSingle(yA), 6, 6);
                    g.DrawEllipse(new Pen(Color.White, 1f), Convert.ToSingle(xA), Convert.ToSingle(yA), 6, 6);

                    double spd = _speedMin + (_speedMax - _speedMin) * (angle - 90) / 225;
                    g.DrawString(spd.ToString("000"), fontArial10Bold, Brushes.White, Convert.ToSingle(xC),
                                 Convert.ToSingle(yC));

                }


                // ---------------------------------        RPM       ---------------------------------
                int retry = 10;
                _rpmAutoRange = false;
                do
                {
                    _rpmMax = 1000*Math.Ceiling(TelemetryApplication.Telemetry.Player.EngineRpmMax/1000);
                    Thread.Sleep(10);
                } while (_rpmMax < 100 && retry-- > 0);

                if(_rpmMax < 1000)
                {
                    _rpmAutoRange = true;
                    _rpmMax = Math.Max(1000 * Math.Ceiling(TelemetryApplication.Telemetry.Player.EngineRpm/ 1000), 6000);
                }

                _rpmMin = 0;

                double rpmStep = 1000;

                // Ranges.
                if (_rpmMax >= 0 && _rpmMax <= 10000) rpmStep = 1000;
                if (_rpmMax > 12000) rpmStep = 2000;

                if (_rpmMax >= 12000 && _rpmMax < 20000)
                {
                    rpmStep = 1000;
                    _rpmMin = _rpmMax - 4*2000;

                }
                if (_rpmMax >= 20000)
                {
                    rpmStep = 2000;
                    _rpmMin = _rpmMax - 7*2000;

                }

                if (_rpmMin%rpmStep != 0)
                    _rpmMin += rpmStep - (_rpmMin%rpmStep);

                if (_rpmMax%rpmStep != 0)
                    _rpmMax += rpmStep - (_rpmMax%rpmStep);

                // ---------------------------------     RPM Gauge    ---------------------------------
                double fAngleRpmRedLine = (TelemetryApplication.Telemetry.Player.EngineRpmMax - rpmStep / 2 - _rpmMin) /
                                            (_rpmMax - _rpmMin)*225;
                if (double.IsInfinity(fAngleRpmRedLine) || double.IsNaN(fAngleRpmRedLine)) fAngleRpmRedLine = 200;
                int angleRpmRedLine = Convert.ToInt32(Math.Round(fAngleRpmRedLine));

                double fAngleRpmWarningLine = (TelemetryApplication.Car.Engine.Lifetime.EngineRpm.Optimum - rpmStep / 2 -
                                                 _rpmMin)/(_rpmMax - _rpmMin)*225;
                if (double.IsInfinity(fAngleRpmWarningLine) || double.IsNaN(fAngleRpmWarningLine))
                    fAngleRpmWarningLine = 180;
                int angleRpmWarningLine = Convert.ToInt32(Math.Round(fAngleRpmWarningLine));
                if (angleRpmWarningLine > angleRpmRedLine)
                {
                    g.DrawArc(new Pen(Color.DarkRed, 10f), borderBounds / 2 + rpmGaugeRadiusPenalty, borderBounds / 2 + rpmGaugeRadiusPenalty, height - rpmGaugeRadiusPenalty*2,
                              height - rpmGaugeRadiusPenalty*2,
                              90 + angleRpmRedLine, 225 - angleRpmRedLine);
                    g.DrawArc(new Pen(Color.Red, 10f), borderBounds / 2 + rpmGaugeRadiusPenalty, borderBounds / 2 + rpmGaugeRadiusPenalty, height - rpmGaugeRadiusPenalty*2,
                              height - rpmGaugeRadiusPenalty*2, 90 + angleRpmWarningLine, 225 - angleRpmWarningLine);
                }
                else
                {

                    g.DrawArc(new Pen(Color.DarkRed, 10f), borderBounds / 2 + rpmGaugeRadiusPenalty, borderBounds / 2 + rpmGaugeRadiusPenalty, height - rpmGaugeRadiusPenalty*2,
                              height - rpmGaugeRadiusPenalty*2, 90 + angleRpmWarningLine, 225 - angleRpmWarningLine);
                    g.DrawArc(new Pen(Color.Red, 10f), borderBounds / 2 + rpmGaugeRadiusPenalty, borderBounds / 2 + rpmGaugeRadiusPenalty, height - rpmGaugeRadiusPenalty * 2,
                              height - rpmGaugeRadiusPenalty*2,
                              90 + angleRpmRedLine, 225 - angleRpmRedLine);
                }
                for (double angle = 90; angle <= 90 + 225; angle += 225.0/((_rpmMax - _rpmMin)/rpmStep))
                {
                    double sinA = Math.Sin(angle/180.0*Math.PI);
                    double cosA = Math.Cos(angle/180.0*Math.PI);

                    double yA = clipHeight/2 + sinA*rpmGaugeRadius;
                    double xA = clipHeight/2 + cosA*rpmGaugeRadius;
                    double yB = clipHeight/2 + sinA*(rpmGaugeRadius+10);
                    double xB = clipHeight/2 + cosA*(rpmGaugeRadius+10);

                    double yC = clipHeight/2 + sinA*(rpmGaugeRadius+25) - 8;
                    double xC = clipHeight/2 + cosA*(rpmGaugeRadius+25) - 8;

                    g.DrawLine(new Pen(Color.White, 2f), Convert.ToSingle(xA), Convert.ToSingle(yA),
                               Convert.ToSingle(xB), Convert.ToSingle(yB));
                    double rpm = _rpmMin + (_rpmMax - _rpmMin)*(angle - 90)/225;
                    g.DrawString((rpm/1000.0).ToString("0"), fontArial10Bold, Brushes.White, Convert.ToSingle(xC),
                                 Convert.ToSingle(yC));

                    for (int i = 1; i < 10; i++)
                    {
                        double angle2 = angle + 225.0/((_rpmMax - _rpmMin)/rpmStep)*i/10;
                        var stripeWidth = 1.0f;
                        var stripeLength = 6;
                        if (i == 5)
                        {
                            stripeLength = 8;
                            stripeWidth = 2.0f;
                        }

                        if (angle2 > 90 + 225) break;
                        double sin2A = Math.Sin(angle2/180.0*Math.PI);
                        double cos2A = Math.Cos(angle2/180.0*Math.PI);

                        double y2A = clipHeight / 2 + sin2A * (rpmGaugeRadius+2);
                        double x2A = clipHeight/2 + cos2A*(rpmGaugeRadius + 2);
                        double y2B = clipHeight / 2 + sin2A * (rpmGaugeRadius+2 + stripeLength);
                        double x2B = clipHeight / 2 + cos2A * (rpmGaugeRadius+2 + stripeLength);

                        if (angle2 > 90 + angleRpmWarningLine)
                            g.DrawLine(new Pen(Color.Red, stripeWidth), Convert.ToSingle(x2A), Convert.ToSingle(y2A),
                                       Convert.ToSingle(x2B), Convert.ToSingle(y2B));
                        else
                            g.DrawLine(new Pen(Color.White, stripeWidth), Convert.ToSingle(x2A), Convert.ToSingle(y2A),
                                       Convert.ToSingle(x2B), Convert.ToSingle(y2B));

                    }
                }

                // Maximum power
                // TODO: Add maximum power
                _powerMax = 600; // Telemetry.m.Computations.MaximumPower(Telemetry.m.Sim, null);
                if (_powerMax <= 0 || _powerMax >= 1000)
                    _powerMax = 1000;

                // ---------------------------------    Labels   ---------------------------------
                var dimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

                // Throttle/brake
                g.DrawString("PEDALS", fontArial10Bold, dimBrush, height - 50, 119);
                g.FillRectangle(dimBrush, height + 10, 120, 120, 13);

                // Fuel remaining / estimated laps.
                g.DrawString("FUEL", fontArial10Bold, dimBrush, height - 50, 139);
                g.FillRectangle(dimBrush, height + 10, 140, 120, 13);

                // Engine wear / laps estimation
                g.DrawString("ENGINE", fontArial10Bold, dimBrush, height - 50, 179);
                g.FillRectangle(dimBrush, height + 10, 180, 120, 13);

                // Power bar.
                g.DrawString("POWER", fontArial10Bold, dimBrush, height - 50, 219);

                // ---------------------------------   Menu Labels   ---------------------------------
                g.DrawString("Engine", fontArial8, Brushes.White, Width - 42, Height - 60);
                g.DrawString("Tyres", fontArial8, dimBrush, Width - 35, Height - 45);
                g.DrawString("Laptimes", fontArial8, dimBrush, Width - 51, Height - 30);
                g.DrawString("Split", fontArial8, dimBrush, Width - 30, Height - 15);
            }

            _emptyGauges = backgroundGauges;
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            // Fonts
            var fontArial10Bold = new Font("Arial", 10f, FontStyle.Bold);
            var fontArial30 = new Font("Arial", 30f);
            var fontArial18 = new Font("Arial", 18f);
            var fontArial12 = new Font("Arial", 12f);
            var fontArial8 = new Font("Arial", 8f);

            try
            {
                int width = Width - borderBounds;
                int height = Height - borderBounds;

                int clipHeight = Height;
                int clipWidth = Width;

                var g = e.Graphics;

                if (!TelemetryApplication.TelemetryAvailable || !TelemetryApplication.CarAvailable) return;
                if (TelemetryApplication.Telemetry.Player == null) return;
                if (_emptyGauges == null) return;
                lock (g)
                {
                    CompositingMode compMode = g.CompositingMode;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImage(_emptyGauges, new Rectangle(Point.Empty, _emptyGauges.Size));
                    g.CompositingMode = compMode;

                    // ---------------------------------      RPM Needle    ---------------------------------
                    double rpmLive = TelemetryApplication.Telemetry.Player.EngineRpm;
                    double fAngleRpm = 90 + (rpmLive - _rpmMin)/(_rpmMax - _rpmMin)*225;
                    if (fAngleRpm < 90) fAngleRpm = 90;
                    if (fAngleRpm > 90 + 225) fAngleRpm = 90 + 225;
                    double rpmGaugeSin = Math.Sin(fAngleRpm/180.0*Math.PI);
                    double rpmGaugeCos = Math.Cos(fAngleRpm/180.0*Math.PI);

                    double rpmGaugeYa = clipHeight/2 + rpmGaugeSin*-15;
                    double rpmGaugeXa = clipHeight/2 + rpmGaugeCos*-15;
                    double rpmGaugeYb = clipHeight / 2 + rpmGaugeSin * (rpmGaugeRadius+10);
                    double rpmGaugeXb = clipHeight / 2 + rpmGaugeCos * (rpmGaugeRadius + 10);

                    if (_rpmAutoRange)
                    {
                        if (rpmLive > _rpmMax)
                        {
                            _rpmMax += 1000;
                            PaintBackground(null);
                        }

                    }

                    g.DrawLine(new Pen(Color.DarkRed, 4f), Convert.ToSingle(rpmGaugeXa),
                               Convert.ToSingle(rpmGaugeYa),
                               Convert.ToSingle(rpmGaugeXb), Convert.ToSingle(rpmGaugeYb));

                    // ---------------------------------     Speed Needle    ---------------------------------
                    double speedLive = TelemetryApplication.Telemetry.Player.Speed* 3.6;

                    double fAngleSpeed = 90 + (speedLive - _speedMin)/(_speedMax - _speedMin)*225;
                    if (fAngleSpeed < 90) fAngleSpeed = 90;
                    if (fAngleSpeed > 90 + 225) fAngleSpeed = 90 + 225;

                    double speedGaugeSin = Math.Sin(fAngleSpeed/180.0*Math.PI);
                    double speedGaugeCos = Math.Cos(fAngleSpeed/180.0*Math.PI);

                    double speedGaugeYa = clipHeight/2 + speedGaugeSin*-20;
                    double speedGaugeXa = clipHeight/2 + speedGaugeCos*-20;
                    double speedGaugeYb = clipHeight/2 + speedGaugeSin*133;
                    double speedGaugeXb = clipHeight/2 + speedGaugeCos*133;

                    g.DrawLine(new Pen(Color.DarkGreen, 6f), Convert.ToSingle(speedGaugeXa),
                               Convert.ToSingle(speedGaugeYa), Convert.ToSingle(speedGaugeXb),
                               Convert.ToSingle(speedGaugeYb));
                    g.FillEllipse(Brushes.DarkGreen, clipHeight/2 - 10, clipHeight/2 - 10, 20, 20);

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
                        g.DrawString("R", fontArial30, Brushes.White, borderBounds/2 + height/2 + 5,
                                     borderBounds/2 + height/2 + 40);
                    else if (TelemetryApplication.Telemetry.Player.Gear > 0)
                        g.DrawString(TelemetryApplication.Telemetry.Player.Gear.ToString(), fontArial30, Brushes.White,
                                     borderBounds/2 + height/2 + 5,
                                     borderBounds/2 + height/2 + 40);
                    else if (TelemetryApplication.Telemetry.Player.Gear == 0)
                        g.DrawString("N", fontArial30, Brushes.White, borderBounds/2 + height/2 + 5,
                                     borderBounds/2 + height/2 + 40);

                    if (TelemetryApplication.Telemetry.Player.IsLimiter)
                        g.DrawString(Math.Abs(TelemetryApplication.Telemetry.Player.Speed* 3.6).ToString("000") + "km/h", fontArial18,
                                     Brushes.DarkOrange,
                                     borderBounds/2 + height/2 + 10, borderBounds/2 + height/2 + 80);
                    else
                        g.DrawString(Math.Abs(TelemetryApplication.Telemetry.Player.Speed * 3.6).ToString("000") + "km/h", fontArial18,
                                     Brushes.White,
                                     borderBounds/2 + height/2 + 10, borderBounds/2 + height/2 + 80);

                    // TODO: Add odo-meter
                    //g.DrawString(Math.Round(0.0, 2).ToString("00000.00km"), fontArial12,
                    //             Brushes.LightGray, borderBounds/2 + height/2 + 10, borderBounds/2 + height/2 + 110);

                    // ---------------------------------    Labels   ---------------------------------

                    // Throttle
                    g.FillRectangle(Brushes.DarkRed, height + 10, 120,
                                    Convert.ToSingle(TelemetryApplication.Telemetry.Player.InputBrake* 120), 13);
                    g.FillRectangle(Brushes.DarkGreen, height + 10, 120,
                                    Convert.ToSingle(TelemetryApplication.Telemetry.Player.InputThrottle* 120), 13);
                    if (TelemetryApplication.Telemetry.Player.InputBrake > TelemetryApplication.Telemetry.Player.InputThrottle)
                    {
                        g.DrawString(TelemetryApplication.Telemetry.Player.InputBrake.ToString("000%"), fontArial10Bold,
                                     Brushes.DarkRed,
                                     width + borderBounds - 45, 119);
                    }
                    else
                    {
                        g.DrawString(TelemetryApplication.Telemetry.Player.InputThrottle.ToString("000%"), fontArial10Bold,
                                     Brushes.DarkGreen,
                                     width + borderBounds - 45, 119);
                    }

                    // Fuel
                    // Check if car data is available. Otherwise, assume the tanksize is 150L (?)
                    double fuelState = TelemetryApplication.Car == null || TelemetryApplication.Car.Chassis.FuelTankSize == 0
                                           ? TelemetryApplication.Telemetry.Player.Fuel/150.0
                                           : TelemetryApplication.Telemetry.Player.Fuel/ TelemetryApplication.Car.Chassis.FuelTankSize;


                    if (fuelState > 0.1)
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 141, 12, 12);
                        g.FillRectangle(Brushes.DarkOrange, height + 22, 141, Convert.ToSingle(120*(fuelState - 0.1)), 12);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 141, Convert.ToSingle(fuelState*120), 12);
                    }
                    if (fuelState < 0.1)
                        g.DrawString(TelemetryApplication.Telemetry.Player.Fuel.ToString("00.00").Replace(".", "L"),
                                     fontArial10Bold, Brushes.Red,
                                     width + borderBounds - 45, 139);
                    else
                        g.DrawString(TelemetryApplication.Telemetry.Player.Fuel.ToString("000.0").Replace(".", "L"),
                                     fontArial10Bold, Brushes.DarkOrange,
                                     width + borderBounds - 45, 139);

                    // Laps estimation.
                    var dimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

                    if (_fuelUsage.Count > 3)
                    {
                        double avg = _fuelUsage.Take(8).Where(x=>x > 0).Average();
                        if (avg > 0)
                        {
                            double laps = TelemetryApplication.Telemetry.Player.Fuel / avg;
                            g.DrawString(laps.ToString("(000)"), fontArial10Bold, Brushes.DarkOrange,
                                         width + borderBounds - 45, 159);
                            g.DrawString(avg.ToString("0.000L") + " per lap", fontArial10Bold, Brushes.DarkOrange,
                                         height + 10, 159);
                        }
                    }
                    else
                        g.DrawString("(???)", fontArial10Bold, dimBrush, width + borderBounds - 45, 159);

                    // Engine
                    double engineLive = TelemetryApplication.Telemetry.Player.EngineLifetime;
                    double enginePerc = engineLive/_engineMax;
                    if (double.IsInfinity(enginePerc) || double.IsNaN(enginePerc)) enginePerc = 1;

                    if (enginePerc > 0.1)
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 180, 12, 13);
                        g.FillRectangle(Brushes.DarkOrange, height + 22, 180, Convert.ToSingle(120*(enginePerc - 0.1)),
                                        13);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.Red, height + 10, 180, Convert.ToSingle(enginePerc*120), 13);
                    }
                    if (enginePerc < 0.4)
                        g.DrawString((100*enginePerc).ToString("00.0").Replace(".", "%"), fontArial10Bold, Brushes.Red,
                                     width + borderBounds - 45, 179);
                    else
                        g.DrawString((100*enginePerc).ToString("000.0").Replace(".", "%"), fontArial10Bold,
                                     Brushes.DarkOrange, width + borderBounds - 45, 179);


                    // Laps estimation.
                    if (_counter == 10)
                        g.DrawString("Refreshed!", fontArial10Bold, Brushes.Yellow, width + borderBounds - 75, 199);
                    else
                    {
                        if (_engineUsage.Count > 5)
                        {
                            double avg = 0;
                            for (int i = _engineUsage.Count - 5; i < _engineUsage.Count; i++) avg += _engineUsage[i];
                            avg /= _engineUsage.Count - (_engineUsage.Count - 5);
                            if (avg > 0)
                            {
                                double engine_laps = _engineLastLap/avg;
                                g.DrawString(engine_laps.ToString("(000)"), fontArial10Bold, Brushes.DarkOrange,
                                             width + borderBounds - 45, 199);
                                g.DrawString(avg.ToString("0.00%") + " per lap", fontArial10Bold, Brushes.DarkOrange,
                                             height + 10, 199);
                            }
                        }
                        else

                            g.DrawString("(???)", fontArial10Bold, dimBrush,
                                         width + borderBounds - 45, 199);
                    }

                    // Power
                    double power;

                    if (true) //Telemetry.m.Sim.Modules.Engine_Power)
                        power = TelemetryApplication.Telemetry.Player.EngineTorque*
                                TelemetryApplication.Telemetry.Player.EngineRpm*Math.PI*2/60000.0;
                    else
                        power = 0; // Telemetry.m.Computations.GetPower(Telemetry.m.Sim.Player.Engine_RPM, Telemetry.m.Sim.Player.Pedals_Throttle);
                    
                    _powerUsage.Add(power);
                    _powerUsage.MaxSize = 3;

                    float powerFactor = Convert.ToSingle(power/_powerMax);
                    if (powerFactor > 1f) powerFactor = 1f;
                    if (powerFactor < 0f) powerFactor = 0f;

                    g.FillRectangle(Brushes.Yellow, height + 10, 220, powerFactor*120f, 13);
                    g.DrawString((power).ToString("0000"), fontArial10Bold, Brushes.Yellow, width + borderBounds - 45, 220);

                    // Todo: Add fast-lap split counter.
                    //g.DrawString(Splits.Split.ToString("0.000"), font_arial_10, Brushes.Red, 0, 10);
                }
            }
            catch (Exception ex)
            {
                // ERROR
                var g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                g.DrawString(ex.Message, fontArial8, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace.Replace("in", "\r\n"), fontArial8, Brushes.White, 10, 30);


            }
        }
        #endregion
    }
}