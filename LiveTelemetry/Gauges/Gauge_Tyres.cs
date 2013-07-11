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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Services;

namespace LiveTelemetry
{
    public partial class Gauge_Tyres : UserControl
    {
        public Gauge_Tyres()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private int TI32(double val)
        {
            return Convert.ToInt32(Math.Round(val));
        }

        private SolidBrush GetTyreBrush(double temperature, double optimal)
        {
            optimal -= 273.15;
            temperature -= 273.15;
            // 579 is yelow, optimal
            // 650 is VERY hot (>15%)   75
            // 500 is cold     (<15%)   75
            // 450 is cold     (<30%)   50
            double what = temperature / optimal;
            what = Math.Max(0.7, Math.Min(1.3, what));
            double wavelength = 79 + what * 75 / 0.15;

            if (double.IsNaN(wavelength))
                wavelength = 79;

            return new SolidBrush(getColorFromWaveLength(TI32(wavelength)));

        }
        private Color getColorFromWaveLength(int Wavelength)
        {
            double Gamma = 1.00;
            int IntensityMax = 255;
            double Blue;
            double Green;
            double Red;
            double Factor;

            if (Wavelength >= 350 && Wavelength <= 439)
            {
                Red = -(Wavelength - 440d) / (440d - 350d);
                Green = 0.0;
                Blue = 1.0;
            }
            else if (Wavelength >= 440 && Wavelength <= 489)
            {
                Red = 0.0;
                Green = (Wavelength - 440d) / (490d - 440d);
                Blue = 1.0;
            }
            else if (Wavelength >= 490 && Wavelength <= 509)
            {
                Red = 0.0;
                Green = 1.0;
                Blue = -(Wavelength - 510d) / (510d - 490d);

            }
            else if (Wavelength >= 510 && Wavelength <= 579)
            {
                Red = (Wavelength - 510d) / (580d - 510d);
                Green = 1.0;
                Blue = 0.0;
            }
            else if (Wavelength >= 580 && Wavelength <= 644)
            {
                Red = 1.0;
                Green = -(Wavelength - 645d) / (645d - 580d);
                Blue = 0.0;
            }
            else if (Wavelength >= 645 && Wavelength <= 780)
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 0.0;
            }
            else
            {
                Red = 0.0;
                Green = 0.0;
                Blue = 0.0;
            }
            if (Wavelength >= 350 && Wavelength <= 419)
            {
                Factor = 0.3 + 0.7 * (Wavelength - 350d) / (420d - 350d);
            }
            else if (Wavelength >= 420 && Wavelength <= 700)
            {
                Factor = 1.0;
            }
            else if (Wavelength >= 701 && Wavelength <= 780)
            {
                Factor = 0.3 + 0.7 * (780d - Wavelength) / (780d - 700d);
            }
            else
            {
                Factor = 0.0;
            }

            int R = this.factorAdjust(Red, Factor, IntensityMax, Gamma);
            int G = this.factorAdjust(Green, Factor, IntensityMax, Gamma);
            int B = this.factorAdjust(Blue, Factor, IntensityMax, Gamma);

            return Color.FromArgb(R, G, B);
        }
        private int factorAdjust(double Color, double Factor, int IntensityMax, double Gamma)
        {
            if (Color == 0.0)
            {
                return 0;
            }
            else
            {
                return (int)Math.Round(IntensityMax * Math.Pow(Color * Factor, Gamma));
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                var f = new Font("Arial", 8f);
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (!TelemetryApplication.TelemetryAvailable) return;
                if (!TelemetryApplication.CarAvailable) return;

                var wheelLF = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.FRONTLEFT);
                var wheelLR = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.REARLEFT);
                var wheelRF = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.FRONTRIGHT);
                var wheelRR = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.REARRIGHT);

                double SLF = 0, SLR = 0, SRF = 0, SRR = 0;
                double SPD = 3.6*TelemetryApplication.Telemetry.Player.Speed;

                SLF = -1 * 3.6 * TelemetryApplication.Telemetry.Player.WheelLF.Speed * wheelLF.Perimeter;
                SLR = -1 * 3.6 * TelemetryApplication.Telemetry.Player.WheelLR.Speed * wheelRF.Perimeter;
                SRF = -1 * 3.6 * TelemetryApplication.Telemetry.Player.WheelRF.Speed * wheelLR.Perimeter;
                SRR = -1 * 3.6 * TelemetryApplication.Telemetry.Player.WheelRR.Speed * wheelRR.Perimeter;
                SPD = 0;


                // Draw 3 rectangles
                if (SPD > 5 && (SLF/SPD > 1.1 || SLF/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 50, 40, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelLF.TemperatureOutside, wheelLF.PeakTemperature.Optimum), 50, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelLF.TemperatureMiddle, wheelLF.PeakTemperature.Optimum), 58, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelLF.TemperatureInside, wheelLF.PeakTemperature.Optimum), 66, 40, 8, 50);
                }

                if (SPD > 5 && (SLR/SPD > 1.1 || SLR/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 50, 220, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelLR.TemperatureOutside, wheelLR.PeakTemperature.Optimum), 50, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelLR.TemperatureMiddle, wheelLR.PeakTemperature.Optimum), 58, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelLR.TemperatureInside, wheelLR.PeakTemperature.Optimum), 66, 220, 8, 50);
                }

                if (SPD > 5 && (SRF/SPD > 1.1 || SRF/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 200, 40, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureOutside, wheelRF.PeakTemperature.Optimum), 216, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureMiddle, wheelRF.PeakTemperature.Optimum), 208, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureInside, wheelRF.PeakTemperature.Optimum), 200, 40, 8, 50);
                }


                if (SPD > 5 && (SRR/SPD > 1.1 || SRR/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 200, 220, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureOutside, wheelRR.PeakTemperature.Optimum), 216, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureMiddle, wheelRR.PeakTemperature.Optimum), 208, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureInside, wheelRR.PeakTemperature.Optimum), 200, 220, 8, 50);
                }

                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureOutside).ToString("000") + "C", f,
                             Brushes.White, 12, 20);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureMiddle).ToString("000") + "C", f,
                             Brushes.White, 47, 20);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureInside).ToString("000") + "C", f,
                             Brushes.White, 82, 20);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Wear.ToString("000%"), f,
                             Brushes.White, 47, 92);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Pressure.ToString("000.0") + "kPa", f,
                             GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.Pressure + 273.15,
                                          wheelLF.PeakPressure.Optimum + 273.15), 47, 107);


                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureInside).ToString("000") + "C", f,
                             Brushes.White, 162, 20);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureMiddle).ToString("000") + "C", f,
                             Brushes.White, 197, 20);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureOutside).ToString("000") + "C", f,
                             Brushes.White, 232, 20);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Wear.ToString("000%"), f,
                             Brushes.White, 197, 92);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Pressure.ToString("000.0") + "kPa", f,
                             GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.Pressure + 273.15,
                                          wheelLF.PeakPressure.Optimum + 273.15), 197, 107);



                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureOutside).ToString("000") + "C", f,
                             Brushes.White, 12, 200);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureMiddle).ToString("000") + "C", f,
                             Brushes.White, 47, 200);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureInside).ToString("000") + "C", f,
                             Brushes.White, 82, 200);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Wear.ToString("000%"), f,
                             Brushes.White, 47, 272);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Pressure.ToString("000.0") + "kPa", f,
                             GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.Pressure + 273.15,
                                         wheelLF.PeakPressure.Optimum + 273.15), 47, 287);


                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureInside).ToString("000") + "C", f,
                             Brushes.White, 162, 200);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.TemperatureMiddle).ToString("000") + "C", f,
                             Brushes.White, 197, 200);
                g.DrawString(Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRR.TemperatureOutside).ToString("000") + "C", f,
                             Brushes.White, 232, 200);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Wear.ToString("000%"), f,
                             Brushes.White, 197, 272);

                g.DrawString(TelemetryApplication.Telemetry.Player.WheelRF.Pressure.ToString("000.0") + "kPa", f,
                             GetTyreBrush(TelemetryApplication.Telemetry.Player.WheelRF.Pressure + 273.15,
                                          wheelLF.PeakPressure.Optimum + 273.15), 197, 287);

                double BrakeWear_LF = 1 -
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - TelemetryApplication.Telemetry.Player.WheelRF.BrakeThicknessBase) /
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - wheelLF.BrakeThicknessFailurePoint);
                double BrakeWear_RF = 1 -
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - TelemetryApplication.Telemetry.Player.WheelRF.BrakeThicknessBase) /
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - wheelLF.BrakeThicknessFailurePoint);



                double BrakeWear_LR = 1 -
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - TelemetryApplication.Telemetry.Player.WheelRF.BrakeThicknessBase) /
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - wheelLF.BrakeThicknessFailurePoint);
                double BrakeWear_RR = 1 -
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - TelemetryApplication.Telemetry.Player.WheelRF.BrakeThicknessBase) /
                                      (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness - wheelLF.BrakeThicknessFailurePoint);

                // Draw brakes. front
                g.FillRectangle(
                    GetBrakeBrush(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature, wheelLF.BrakeTemperature.Minimum,
                                  wheelLF.BrakeTemperature.Maximum), 78, 50, 7, 30);
                g.DrawString(
                    Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature).ToString("000") + "C", f, Brushes.White, 84, 50);

                if (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness == 0)
                    g.DrawString("STUK", f, Brushes.Red, 84, 65);
                else
                    g.DrawString(BrakeWear_LF.ToString("000%"), f,
                                 ((BrakeWear_LF < 0.3) ? Brushes.Yellow : Brushes.White), 84, 65);

                g.FillRectangle(
                    GetBrakeBrush(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature, wheelLF.BrakeTemperature.Minimum,
                                  wheelLF.BrakeTemperature.Maximum), 188, 50, 7, 30);
                g.DrawString(
                    Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature).ToString("000") + "C", f, Brushes.White, 160, 50);

                if (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness == 0)
                    g.DrawString("STUK", f, Brushes.Red, 155, 65);
                else
                    g.DrawString(BrakeWear_RF.ToString("000%"), f,
                                 ((BrakeWear_RF < 0.3) ? Brushes.Yellow : Brushes.White), 155, 65);

                // Draw brakes. rear
                g.FillRectangle(
                    GetBrakeBrush(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature, wheelLF.BrakeTemperature.Minimum,
                                 wheelLF.BrakeTemperature.Maximum), 78, 230, 7, 30);
                g.DrawString(
                    Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature).ToString("000") + "C", f, Brushes.White, 84, 230);

                if (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness == 0)
                    g.DrawString("STUK", f, Brushes.Red, 84, 245);
                else
                    g.DrawString(BrakeWear_LR.ToString("000%"), f,
                                 ((BrakeWear_LR < 0.3) ? Brushes.Yellow : Brushes.White), 84, 245);

                g.FillRectangle(
                    GetBrakeBrush(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature, wheelLF.BrakeTemperature.Minimum,
                                  wheelLF.BrakeTemperature.Maximum), 188, 230, 7, 30);
                g.DrawString(
                    Temperature.KC(TelemetryApplication.Telemetry.Player.WheelRF.BrakeTemperature).ToString("000") + "C", f, Brushes.White, 160,
                    230);

                if (TelemetryApplication.Telemetry.Player.WheelRF.BrakeThickness == 0)
                    g.DrawString("STUK", f, Brushes.Red, 155, 245);
                else
                    g.DrawString(BrakeWear_RR.ToString("000%"), f,
                                 ((BrakeWear_RR < 0.3) ? Brushes.Yellow : Brushes.White), 155, 245);



                double oil = TelemetryApplication.Telemetry.Player.OilTemperature;
                double oil_max = TelemetryApplication.Car.Engine.MaximumOilTemperature;
                double oil_factor = oil/oil_max;
                g.DrawString("OIL", f, DimBrush, 30, 130);
                for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc * oil_max / 120 > TelemetryApplication.Car.Engine.MaximumOilTemperature)
                    {
                        if (perc * oil_max / 120 > oil)
                            g.FillRectangle(DimRed, 60 + 30 + perc, 130, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkRed, 60 + 30 + perc, 130, 2, 13);
                    }
                    else
                    {
                        if (perc * oil_max / 120 > oil)
                            g.FillRectangle(DimBrush, 60 + 30 + perc, 130, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkGoldenrod, 60 + 30 + perc, 130, 2, 13);
                    }

                }
                g.DrawString((oil).ToString("000") + "C", f, Brushes.Yellow, 220, 130);

                double water = TelemetryApplication.Telemetry.Player.WaterTemperature;
                double water_max = TelemetryApplication.Car.Engine.MaximumWaterTemperature;// TODO: Check this
              
                double water_factor = water/water_max;
                g.DrawString("Water", f, DimBrush, 30, 150);
                for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc * water_max / 120 > TelemetryApplication.Car.Engine.MaximumWaterTemperature)
                    {
                        if (perc * water_max / 120 > water)
                            g.FillRectangle(DimRed, 60 + 30 + perc, 150, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkRed, 60 + 30 + perc, 150, 2, 13);
                    }
                    else
                    {
                        if (perc * water_max / 120 > water)
                            g.FillRectangle(DimBrush, 60 + 30 + perc, 150, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkSlateBlue, 60 + 30 + perc, 150, 2, 13);
                    }


                }
                g.DrawString((water).ToString("000") + "C", f, Brushes.Yellow, 220, 150);

                g.DrawString("TRACK TEMPERATURE", f, DimBrush, 300f, 50f);

                var trf = new Font("Calibri", 36f);
                g.DrawString(TelemetryApplication.Telemetry.Session.TrackTemperature.ToString("00.0") + "C", trf, Brushes.DarkGray, 320f, 70f);

                g.DrawString("AMBIENT TEMPERATURE", f, DimBrush, 300f, 150f);
                g.DrawString(TelemetryApplication.Telemetry.Session.AmbientTemperature.ToString("00.0") + "C", trf, Brushes.DarkGray, 320f, 170f);

                var sf = new Font("Arial", 8f);


                g.DrawString("Engine", sf, DimBrush, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, Brushes.White, this.Width - 35, this.Height - 45);
                g.DrawString("Laptimes", sf, DimBrush, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf, DimBrush, this.Width - 30, this.Height - 15);
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 10f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);


            }
        }
        private Color DimColor = Color.FromArgb(70, 70, 70);
        private SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        private SolidBrush DimRed = new SolidBrush(Color.FromArgb(230, 70, 70));

        private Brush GetBrakeBrush(double temperature, double low, double high)
        {
            low -= 273.15;
            high -= 273.15;
            temperature -= 273.15;
            // 579 is green, optimal
            // 650 is VERY hot (>15%)   75
            // 500 is cold     (<15%)   75
            // 450 is cold     (<30%)   50
            double what = 1;
            if (temperature < low)
                what = temperature / low;
            else if (temperature >= low && temperature <= high)
                what = 1;
            else
                what = temperature/high;

            what = Math.Max(0.7, Math.Min(1.3, what));
            double wavelength = 204 + what * 75 / 0.2;

            return new SolidBrush(getColorFromWaveLength(TI32(wavelength)));
        }
    }
}
