using System;
using System.Drawing;
using System.Windows.Forms;
using SimTelemetry.Data;

namespace LiveTelemetry
{
    public partial class ucFuel : UserControl
    {
        private double Distance = 0;
        private double Time = 0;
        private double Fuel_Consumped = 0;
        private double Fuel_d = 0;
        private double Fuel_Last = 0;
        private double Fuel_Consumption = 0;
        private double TotalDistance = 0;
        public ucFuel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public void Update()
        {
            if (!Telemetry.m.Active_Session) return;
            double SampleDistance = 100;
            double CurrentTime = Telemetry.m.Sim.Session.Time;
            double dt = CurrentTime - Time;
            Time = CurrentTime;
            double spd = Math.Abs(Telemetry.m.Sim.Player.SpeedSlipping);
            if (dt < 0.1)
            {
                Distance += dt*spd;
                TotalDistance += dt*spd;
            }
            double df =Fuel_Last - Telemetry.m.Sim.Player.Fuel;
            if (df >= 0 && df < 1)
            {
                Fuel_d += df;
                Fuel_Consumped += df;
            }
            Fuel_Last = Telemetry.m.Sim.Player.Fuel;
            if (Math.Abs(Distance) > SampleDistance)
            {
                double filter = 0.6;
                double fc = Fuel_d / (Distance/1000);
                if (Fuel_Consumption == 0) Fuel_Consumption = fc;
                else Fuel_Consumption = filter*Fuel_Consumption + (1-filter)*fc;
                
                Fuel_d = 0;
                Distance = 0;


            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                Graphics g = e.Graphics;
                if (!Telemetry.m.Active_Session)
                {
                    g.FillRectangle(Brushes.Black, e.ClipRectangle); return;
                }
                g.FillRectangle(Brushes.Red, e.ClipRectangle);
                System.Drawing.Font f = new Font("Arial", 8f);
                g.DrawString((1/Fuel_Consumption).ToString("0.00") + " km/l ("+(Telemetry.m.Sim.Player.SpeedSlipping*3.6).ToString("000.0")+"km/h)", f, Brushes.White, 2f, 2f);
                g.DrawString((TotalDistance/1000).ToString("000000.000km") + " with " + (Fuel_Consumped).ToString("00.000")+"L fuel", f, Brushes.White,2f, 14f);
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 8f);
                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);


            }
        }
    }
}