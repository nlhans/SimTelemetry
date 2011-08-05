using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Game.Rfactor;
using SimTelemetry.Objects;

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

                double OLF = rFactor.Player.Tyre_Temperature_LF_Optimal;
                double OLR = rFactor.Player.Tyre_Temperature_LR_Optimal;
                double ORF = rFactor.Player.Tyre_Temperature_RF_Optimal;
                double ORR = rFactor.Player.Tyre_Temperature_RR_Optimal;

                double SLF = -1*3.6*rFactor.Player.Tyre_Speed_LF*rFactor.Player.Wheel_Radius_LF;
                double SLR = -1*3.6*rFactor.Player.Tyre_Speed_LR*rFactor.Player.Wheel_Radius_LR;
                double SRF = -1*3.6*rFactor.Player.Tyre_Speed_RF*rFactor.Player.Wheel_Radius_RF;
                double SRR = -1*3.6*rFactor.Player.Tyre_Speed_RR*rFactor.Player.Wheel_Radius_RR;
                double SPD = 3.6*rFactor.Player.Speed;
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (rFactor.Session.Cars == 0) return;

                System.Drawing.Font f = new Font("Arial", 8f);

                // Draw 3 rectangles
                if (SPD > 5 && (SLF/SPD > 1.1 || SLF/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 50, 40, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_LF_Outside, OLF), 50, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_LF_Middle, OLF), 58, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_LF_Inside, OLF), 66, 40, 8, 50);
                }

                if (SPD > 5 && (SLR/SPD > 1.1 || SLR/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 50, 220, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_LR_Outside, OLR), 50, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_LR_Middle, OLR), 58, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_LR_Inside, OLR), 66, 220, 8, 50);
                }

                if (SPD > 5 && (SRF/SPD > 1.1 || SRF/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 200, 40, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_RF_Outside, ORF), 216, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_RF_Middle, ORF), 208, 40, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_RF_Inside, ORF), 200, 40, 8, 50);
                }


                if (SPD > 5 && (SRR/SPD > 1.1 || SRR/SPD < 0.9))
                {
                    g.FillRectangle(Brushes.DarkRed, 200, 220, 24, 50);
                }
                else
                {
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_RR_Outside, ORR), 216, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_RR_Middle, ORR), 208, 220, 8, 50);
                    g.FillRectangle(GetTyreBrush(rFactor.Player.Tyre_Temperature_RR_Inside, ORR), 200, 220, 8, 50);
                }

                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_LF_Outside).ToString("000") + "C", f,
                             Brushes.White, 12, 20);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_LF_Middle).ToString("000") + "C", f,
                             Brushes.White, 47, 20);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_LF_Inside).ToString("000") + "C", f,
                             Brushes.White, 82, 20);

                g.DrawString(rFactor.Drivers.Player.TyreWear_LF.ToString("000%"), f,
                             Brushes.White, 47, 92);

                g.DrawString(rFactor.Player.Tyre_Pressure_LF.ToString("000.0") + "kPa", f,
                             GetTyreBrush(rFactor.Player.Tyre_Pressure_LF + 273.15,
                                          rFactor.Player.Tyre_Pressure_Optimal_LF + 273.15), 47, 107);


                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_RF_Inside).ToString("000") + "C", f,
                             Brushes.White, 162, 20);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_RF_Middle).ToString("000") + "C", f,
                             Brushes.White, 197, 20);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_RF_Outside).ToString("000") + "C", f,
                             Brushes.White, 232, 20);

                g.DrawString(rFactor.Drivers.Player.TyreWear_RF.ToString("000%"), f,
                             Brushes.White, 197, 92);

                g.DrawString(rFactor.Player.Tyre_Pressure_RF.ToString("000.0") + "kPa", f,
                             GetTyreBrush(rFactor.Player.Tyre_Pressure_LF + 273.15,
                                          rFactor.Player.Tyre_Pressure_Optimal_LF + 273.15), 197, 107);



                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_LR_Outside).ToString("000") + "C", f,
                             Brushes.White, 12, 200);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_LR_Middle).ToString("000") + "C", f,
                             Brushes.White, 47, 200);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_LR_Inside).ToString("000") + "C", f,
                             Brushes.White, 82, 200);

                g.DrawString(rFactor.Drivers.Player.TyreWear_LR.ToString("000%"), f,
                             Brushes.White, 47, 272);

                g.DrawString(rFactor.Player.Tyre_Pressure_LR.ToString("000.0") + "kPa", f,
                             GetTyreBrush(rFactor.Player.Tyre_Pressure_LF + 273.15,
                                          rFactor.Player.Tyre_Pressure_Optimal_LF + 273.15), 47, 287);


                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_RR_Inside).ToString("000") + "C", f,
                             Brushes.White, 162, 200);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_RR_Middle).ToString("000") + "C", f,
                             Brushes.White, 197, 200);
                g.DrawString(Temperature.KC(rFactor.Player.Tyre_Temperature_RR_Outside).ToString("000") + "C", f,
                             Brushes.White, 232, 200);

                g.DrawString(rFactor.Drivers.Player.TyreWear_RR.ToString("000%"), f,
                             Brushes.White, 197, 272);

                g.DrawString(rFactor.Player.Tyre_Pressure_RR.ToString("000.0") + "kPa", f,
                             GetTyreBrush(rFactor.Player.Tyre_Pressure_LF + 273.15,
                                          rFactor.Player.Tyre_Pressure_Optimal_LF + 273.15), 197, 287);

                double BrakeWear_LF = 1 -
                                      (rFactor.Player.Brake_InitialThickness_LF - rFactor.Player.Brake_Thickness_LF)/
                                      (rFactor.Player.Brake_InitialThickness_LF - rFactor.Player.Brake_TypicalFailure_LF);
                double BrakeWear_RF = 1 -
                                      (rFactor.Player.Brake_InitialThickness_RF - rFactor.Player.Brake_Thickness_RF)/
                                      (rFactor.Player.Brake_InitialThickness_RF - rFactor.Player.Brake_TypicalFailure_RF);



                double BrakeWear_LR = 1 -
                                      (rFactor.Player.Brake_InitialThickness_LR - rFactor.Player.Brake_Thickness_LR)/
                                      (rFactor.Player.Brake_InitialThickness_LR - rFactor.Player.Brake_TypicalFailure_LR);
                double BrakeWear_RR = 1 -
                                      (rFactor.Player.Brake_InitialThickness_RR - rFactor.Player.Brake_Thickness_RR)/
                                      (rFactor.Player.Brake_InitialThickness_RR - rFactor.Player.Brake_TypicalFailure_RR);

                // Draw brakes. front
                g.FillRectangle(
                    GetBrakeBrush(rFactor.Player.Brake_Temperature_LF, rFactor.Player.Brake_OptimalTemperature_LF_Low,
                                  rFactor.Player.Brake_OptimalTemperature_LF_High), 78, 50, 7, 30);
                g.DrawString(
                    Temperature.KC(rFactor.Player.Brake_Temperature_LF).ToString("000") + "C", f, Brushes.White, 84, 50);

                if (rFactor.Player.Brake_Thickness_LF == 0)
                    g.DrawString("STUK", f, Brushes.Red, 84, 65);
                else
                    g.DrawString(BrakeWear_LF.ToString("000%"), f,
                                 ((BrakeWear_LF < 0.3) ? Brushes.Yellow : Brushes.White), 84, 65);

                g.FillRectangle(
                    GetBrakeBrush(rFactor.Player.Brake_Temperature_RF, rFactor.Player.Brake_OptimalTemperature_LF_Low,
                                  rFactor.Player.Brake_OptimalTemperature_LF_High), 188, 50, 7, 30);
                g.DrawString(
                    Temperature.KC(rFactor.Player.Brake_Temperature_RF).ToString("000") + "C", f, Brushes.White, 160, 50);

                if (rFactor.Player.Brake_Thickness_RF == 0)
                    g.DrawString("STUK", f, Brushes.Red, 155, 65);
                else
                    g.DrawString(BrakeWear_RF.ToString("000%"), f,
                                 ((BrakeWear_RF < 0.3) ? Brushes.Yellow : Brushes.White), 155, 65);

                // Draw brakes. rear
                g.FillRectangle(
                    GetBrakeBrush(rFactor.Player.Brake_Temperature_LR, rFactor.Player.Brake_OptimalTemperature_LR_Low,
                                  rFactor.Player.Brake_OptimalTemperature_LR_High), 78, 230, 7, 30);
                g.DrawString(
                    Temperature.KC(rFactor.Player.Brake_Temperature_LR).ToString("000") + "C", f, Brushes.White, 84, 230);

                if (rFactor.Player.Brake_Thickness_LR == 0)
                    g.DrawString("STUK", f, Brushes.Red, 84, 245);
                else
                    g.DrawString(BrakeWear_LR.ToString("000%"), f,
                                 ((BrakeWear_LR < 0.3) ? Brushes.Yellow : Brushes.White), 84, 245);

                g.FillRectangle(
                    GetBrakeBrush(rFactor.Player.Brake_Temperature_RR, rFactor.Player.Brake_OptimalTemperature_LR_Low,
                                  rFactor.Player.Brake_OptimalTemperature_LR_High), 188, 230, 7, 30);
                g.DrawString(
                    Temperature.KC(rFactor.Player.Brake_Temperature_RR).ToString("000") + "C", f, Brushes.White, 160,
                    230);

                if (rFactor.Player.Brake_Thickness_RR == 0)
                    g.DrawString("STUK", f, Brushes.Red, 155, 245);
                else
                    g.DrawString(BrakeWear_RR.ToString("000%"), f,
                                 ((BrakeWear_RR < 0.3) ? Brushes.Yellow : Brushes.White), 155, 245);



                double oil = rFactor.Player.Engine_Temperature_Oil;
                double oil_max = rFactor.Player.Engine_Lifetime_Oil_Base + 30;
                double oil_factor = oil/oil_max;
                g.DrawString("OIL", f, DimBrush, 30, 130);
                for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc * oil_max / 120 > rFactor.Player.Engine_Lifetime_Oil_Base)
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

                double water = rFactor.Player.Engine_Temperature_Water;
                double water_max = rFactor.Player.Engine_Lifetime_Oil_Base+30;
              
                double water_factor = water/water_max;
                g.DrawString("Water", f, DimBrush, 30, 150);
                for (int perc = 0; perc < 120; perc += 3)
                {
                    if (perc * water_max / 120 > rFactor.Player.Engine_Lifetime_Oil_Base)
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

                System.Drawing.Font trf = new Font("Calibri", 36f);
                g.DrawString(rFactor.Session.TrackTemperature.ToString("00") + "C", trf, Brushes.DarkGray, 320f, 70f);

                g.DrawString("AMBIENT TEMPERATURE", f, DimBrush, 300f, 150f);
                g.DrawString(rFactor.Session.AmbientTemperature.ToString("00") + "C", trf, Brushes.DarkGray, 320f, 170f);

                Font sf = new Font("Arial", 8f);


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
