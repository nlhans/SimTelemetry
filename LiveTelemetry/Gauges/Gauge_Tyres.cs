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

                var sf = new Font("Arial", 8f);

                g.DrawString("Engine", sf, DimBrush, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, Brushes.White, this.Width - 35, this.Height - 45);
                g.DrawString("Laptimes", sf, DimBrush, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf, DimBrush, this.Width - 30, this.Height - 15);

                if (!TelemetryApplication.TelemetryAvailable)
                {
                    g.DrawString("No session detected.", f, Brushes.White, 10, 10);
                    return;
                }
                if (!TelemetryApplication.CarAvailable)
                {
                    g.DrawString("Car is not modelled in this plug-in.", f, Brushes.White, 10, 10);
                    return;
                }
                if(TelemetryApplication.Telemetry.Player.WheelLF == null)
                {
                    g.DrawString("Wheels are not modelled in this plug-in.", f, Brushes.White, 10, 10);
                    return;
                }
                var carWheelLF = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.FRONTLEFT);
                var carWheelLR = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.REARLEFT);
                var carWheelRF = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.FRONTRIGHT);
                var carWheelRR = TelemetryApplication.Car.Wheels.FirstOrDefault(x => x.Location == WheelLocation.REARRIGHT);

                var drvWheelLF = TelemetryApplication.Telemetry.Player.WheelLF;
                var drvWheelLR = TelemetryApplication.Telemetry.Player.WheelLR;
                var drvWheelRF = TelemetryApplication.Telemetry.Player.WheelRF;
                var drvWheelRR = TelemetryApplication.Telemetry.Player.WheelRR;
                
                var carBrakeLF = TelemetryApplication.Car.Brakes.FirstOrDefault(x => x.Location == WheelLocation.FRONTLEFT);
                var carBrakeRF = TelemetryApplication.Car.Brakes.FirstOrDefault(x => x.Location == WheelLocation.REARLEFT);
                var carBrakeLR = TelemetryApplication.Car.Brakes.FirstOrDefault(x => x.Location == WheelLocation.FRONTRIGHT);
                var carBrakeRR = TelemetryApplication.Car.Brakes.FirstOrDefault(x => x.Location == WheelLocation.REARRIGHT);
                
                double SLF = 0, SLR = 0, SRF = 0, SRR = 0;
                double SPD = 3.6 * TelemetryApplication.Telemetry.Player.Speed;

                SLF = -1 * 3.6 * drvWheelLF.Speed * carWheelLF.Perimeter;
                SLR = -1 * 3.6 * drvWheelLR.Speed * carWheelRF.Perimeter;
                SRF = -1 * 3.6 * drvWheelRF.Speed * carWheelLR.Perimeter;
                SRR = -1 * 3.6 * drvWheelRR.Speed * carWheelRR.Perimeter;
                SPD = 0;


                // Draw 3 rectangles
                if (SPD > 5 && (SLF / SPD > 1.1 || SLF / SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 50, 40, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(drvWheelLF.TemperatureOutside, carWheelLF.PeakTemperature), 50, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelLF.TemperatureMiddle, carWheelLF.PeakTemperature), 58, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelLF.TemperatureInside, carWheelLF.PeakTemperature), 66, 40, 8, 50);
                }

                if (SPD > 5 && (SLR / SPD > 1.1 || SLR / SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 50, 220, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(drvWheelLR.TemperatureOutside, carWheelLR.PeakTemperature), 50, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelLR.TemperatureMiddle, carWheelLR.PeakTemperature), 58, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelLR.TemperatureInside, carWheelLR.PeakTemperature), 66, 220, 8, 50);
                }

                if (SPD > 5 && (SRF / SPD > 1.1 || SRF / SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 200, 40, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(drvWheelRF.TemperatureOutside, carWheelRF.PeakTemperature), 216, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelRF.TemperatureMiddle, carWheelRF.PeakTemperature), 208, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelRF.TemperatureInside, carWheelRF.PeakTemperature), 200, 40, 8, 50);
                }


                if (SPD > 5 && (SRR / SPD > 1.1 || SRR / SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 200, 220, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(drvWheelRR.TemperatureOutside, carWheelRR.PeakTemperature), 216, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelRR.TemperatureMiddle, carWheelRR.PeakTemperature), 208, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(drvWheelRR.TemperatureInside, carWheelRR.PeakTemperature), 200, 220, 8, 50);
                }

                g.DrawString(Temperature.KC(drvWheelLF.TemperatureOutside).ToString("000") + "C", f, Brushes.White, 12, 20);
                g.DrawString(Temperature.KC(drvWheelLF.TemperatureMiddle).ToString("000") + "C", f, Brushes.White, 47, 20);
                g.DrawString(Temperature.KC(drvWheelLF.TemperatureInside).ToString("000") + "C", f, Brushes.White, 82, 20);

                g.DrawString(drvWheelLF.TyreWear.ToString("000%"), f, Brushes.White, 47, 92);

                g.DrawString(drvWheelLF.TyrePressure.ToString("000.0") + "kPa", f, GetTyreBrush(drvWheelLF.TyrePressure + 273.15, carWheelLF.PeakPressure + 273.15), 47, 107);


                g.DrawString(Temperature.KC(drvWheelRF.TemperatureInside).ToString("000") + "C", f, Brushes.White, 162, 20);
                g.DrawString(Temperature.KC(drvWheelRF.TemperatureMiddle).ToString("000") + "C", f, Brushes.White, 197, 20);
                g.DrawString(Temperature.KC(drvWheelRF.TemperatureOutside).ToString("000") + "C", f, Brushes.White, 232, 20);

                g.DrawString(drvWheelRF.TyreWear.ToString("000%"), f, Brushes.White, 197, 92);

                g.DrawString(drvWheelRF.TyrePressure.ToString("000.0") + "kPa", f, GetTyreBrush(drvWheelRF.TyrePressure + 273.15, carWheelRF.PeakPressure + 273.15), 197, 107);



                g.DrawString(Temperature.KC(drvWheelLR.TemperatureOutside).ToString("000") + "C", f, Brushes.White, 12, 200);
                g.DrawString(Temperature.KC(drvWheelLR.TemperatureMiddle).ToString("000") + "C", f, Brushes.White, 47, 200);
                g.DrawString(Temperature.KC(drvWheelLR.TemperatureInside).ToString("000") + "C", f, Brushes.White, 82, 200);

                g.DrawString(drvWheelLR.TyreWear.ToString("000%"), f, Brushes.White, 47, 272);

                g.DrawString(drvWheelLR.TyrePressure.ToString("000.0") + "kPa", f, GetTyreBrush(drvWheelLR.TyrePressure + 273.15, carWheelLR.PeakPressure + 273.15), 47, 287);


                g.DrawString(Temperature.KC(drvWheelRR.TemperatureInside).ToString("000") + "C", f, Brushes.White, 162, 200);
                g.DrawString(Temperature.KC(drvWheelRR.TemperatureMiddle).ToString("000") + "C", f, Brushes.White, 197, 200);
                g.DrawString(Temperature.KC(drvWheelRR.TemperatureOutside).ToString("000") + "C", f, Brushes.White, 232, 200);

                g.DrawString(drvWheelRR.TyreWear.ToString("000%"), f, Brushes.White, 197, 272);

                g.DrawString(drvWheelRR.TyrePressure.ToString("000.0") + "kPa", f, GetTyreBrush(drvWheelRR.TyrePressure + 273.15, carWheelRR.PeakPressure + 273.15), 197, 287);
                
                double BrakeWear_LF = 1 -
                                      (carBrakeLF.ThicknessStart.Minimum - drvWheelLF.BrakeThickness) /
                                      (carBrakeLF.ThicknessStart.Minimum - carBrakeLF.ThicknessFailure);
                double BrakeWear_RF = 1 -
                                      (carBrakeRF.ThicknessStart.Minimum - drvWheelRF.BrakeThickness) /
                                      (carBrakeRF.ThicknessStart.Minimum - carBrakeRF.ThicknessFailure);
                double BrakeWear_LR = 1 -
                                      (carBrakeLR.ThicknessStart.Minimum - drvWheelLR.BrakeThickness) /
                                      (carBrakeLR.ThicknessStart.Minimum - carBrakeLR.ThicknessFailure);
                double BrakeWear_RR = 1 -
                                      (carBrakeRR.ThicknessStart.Minimum - drvWheelRR.BrakeThickness) /
                                      (carBrakeRR.ThicknessStart.Minimum - carBrakeRR.ThicknessFailure);

                // Draw brakes. front
                g.FillRectangle(GetBrakeBrush(drvWheelLF.BrakeTemperature, carBrakeLF.OptimumTemperature.Minimum, carBrakeLF.OptimumTemperature.Maximum), 78, 50, 7, 30);
                g.DrawString(Temperature.KC(drvWheelLF.BrakeTemperature).ToString("0000") + "C", f, Brushes.White, 84, 50);

                if (drvWheelLF.BrakeThickness == 0)
                    g.DrawString("FAULT", f, Brushes.Red, 84, 65);
                else
                    g.DrawString(BrakeWear_LF.ToString("000%"), f, ((BrakeWear_LF < 0.5) ? Brushes.Orange : Brushes.White), 84, 65);

                g.FillRectangle(GetBrakeBrush(drvWheelRF.BrakeTemperature, carBrakeRF.OptimumTemperature.Minimum, carBrakeRF.OptimumTemperature.Maximum), 188, 50, 7, 30);
                g.DrawString(Temperature.KC(drvWheelRF.BrakeTemperature).ToString("0000") + "C", f, Brushes.White, 149, 50);

                if (drvWheelRF.BrakeThickness == 0)
                    g.DrawString("FAULT", f, Brushes.Red, 155, 65);
                else
                    g.DrawString(BrakeWear_RF.ToString("000%"), f, ((BrakeWear_RF < 0.5) ? Brushes.Orange : Brushes.White), 155, 65);

                // Draw brakes. rear
                g.FillRectangle(GetBrakeBrush(drvWheelLR.BrakeTemperature, carBrakeLR.OptimumTemperature.Minimum, carBrakeLR.OptimumTemperature.Maximum), 78, 230, 7, 30);
                g.DrawString(Temperature.KC(drvWheelLR.BrakeTemperature).ToString("0000") + "C", f, Brushes.White, 84, 230);

                if (drvWheelLR.BrakeThickness == 0)
                    g.DrawString("FAULT", f, Brushes.Red, 84, 245);
                else
                    g.DrawString(BrakeWear_LR.ToString("000%"), f, ((BrakeWear_LR < 0.5) ? Brushes.Orange : Brushes.White), 84, 245);

                g.FillRectangle(GetBrakeBrush(drvWheelRR.BrakeTemperature, carBrakeRR.OptimumTemperature.Minimum, carBrakeRR.OptimumTemperature.Maximum), 188, 230, 7, 30);
                g.DrawString(Temperature.KC(drvWheelRR.BrakeTemperature).ToString("0000") + "C", f, Brushes.White, 149, 230);

                if (drvWheelRR.BrakeThickness == 0)
                    g.DrawString("FAULT", f, Brushes.Red, 155, 245);
                else
                    g.DrawString(BrakeWear_RR.ToString("000%"), f, ((BrakeWear_RR < 0.5) ? Brushes.Orange : Brushes.White), 155, 245);



                var oil = TelemetryApplication.Telemetry.Player.OilTemperature;
                var oil_max = TelemetryApplication.Car.Engine.MaximumOilTemperature;

                g.DrawString("OIL", f, DimBrush, 30, 130);
                for (int perc = 0; perc < 120; perc += 3)
                {
                    var oil_pos = perc * oil_max / 120;
                    if (perc>100)
                    {
                        if (oil_pos > oil)
                            g.FillRectangle(DimRed, 60 + 30 + perc, 130, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkRed, 60 + 30 + perc, 130, 2, 13);
                    }
                    else
                    {
                        if (oil_pos > oil)
                            g.FillRectangle(DimBrush, 60 + 30 + perc, 130, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkGoldenrod, 60 + 30 + perc, 130, 2, 13);
                    }

                }
                g.DrawString((oil).ToString("000") + "C", f, Brushes.Yellow, 220, 130);

                var water = TelemetryApplication.Telemetry.Player.WaterTemperature;
                var water_max = TelemetryApplication.Car.Engine.MaximumWaterTemperature;

                g.DrawString("Water", f, DimBrush, 30, 150);
                for (int perc = 0; perc < 120; perc += 3)
                {
                    var water_pos = perc * water_max / 120;
                    if (perc > 100)
                    {
                        if (water_pos > water)
                            g.FillRectangle(DimRed, 60 + 30 + perc, 150, 2, 13);
                        else
                            g.FillRectangle(Brushes.DarkRed, 60 + 30 + perc, 150, 2, 13);
                    }
                    else
                    {
                        if (water_pos > water)
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
