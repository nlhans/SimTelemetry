using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Services;

namespace LiveTelemetry.Gauges
{
    public partial class ucGForce : UserControl
    {
        private Dictionary<float, Tuple<float, float>> persistance = new Dictionary<float, Tuple<float, float>>();  
        
        public ucGForce()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            GlobalEvents.Hook<SessionStarted>((e) => persistance.Clear(), true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            var center = new Point(this.Width / 2+10, this.Height / 2+10);
            var span = new Size(this.Height-20, this.Height-20);

            base.OnPaint(e);
            Graphics g = e.Graphics;

            try
            {
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (!TelemetryApplication.TelemetryAvailable) return;

                // Persistance housekeeping.
                while(persistance.Count > 1000) // Chart updated at 10Hz -> 10 sec persistance
                {
                    persistance.Remove(persistance.Keys.Min());
                }

                var time = TelemetryApplication.Data.Session.Time;
                var accX = TelemetryApplication.Data.Player.AccelerationX;
                var accY = TelemetryApplication.Data.Player.AccelerationY;
                var acc = new Tuple<float, float>(accX, accY);

                if (persistance.ContainsKey(time) == false)
                    persistance.Add(time, acc);

                // Draw G-Force chart
                var scaleX = 5;
                var scaleY = 5;

                // Grid
                g.DrawLine(new Pen(Color.LightGray, 2), center.X - span.Width / 2, center.Y, center.X + span.Width / 2, center.Y);
                g.DrawLine(new Pen(Color.LightGray, 2), center.X, center.Y - span.Height / 2, center.X, center.Y + span.Height / 2);

                var f =new Font("Arial", 8);

                for (var sX = -scaleX; sX <= scaleX; sX++)
                {
                    var x = (float)(center.X + span.Width / 2 * sX/scaleX);
                    var y = (float)(center.Y + span.Height / 2 * 0);

                    g.DrawLine(new Pen(Color.White, 2), x, y + 5, x, y - 5);
                    if (sX != 0) 
                        g.DrawString(sX.ToString(), f, Brushes.Yellow, x - 6, y + 6);
                }

                for (var sY = -scaleY; sY <= scaleY; sY++)
                {
                    var x = (float)(center.X + span.Width / 2 * 0);
                    var y = (float)(center.Y - span.Height / 2 * sY / scaleY);

                    g.DrawLine(new Pen(Color.White, 2), x-5, y, x+5, y);
                    if(sY!=0)
                    g.DrawString(sY.ToString(), f, Brushes.Yellow, x-20,y-6);
                }

                // Dots
                var dot = 5;
                var opacity = 1.0;
                var ind = persistance.Count;
                foreach(var item in persistance.OrderBy(x=>x.Key))
                {
                    ind--;
                    opacity = 1.0 - ind/500.0;
                    if (opacity < 0.3) opacity = 0.3;
                    var p = new SolidBrush(Color.FromArgb((int)Math.Round(opacity * 255), 200, 50, 0));
                    if (Math.Abs(item.Key - time) < 0.001)
                    {
                        p = new SolidBrush(Color.FromArgb((int) Math.Round(opacity*255), 255, 255, 255));
                    }

                    var x = (float)(center.X + span.Width / 2 * item.Value.Item2 / 9.81 / scaleX-dot/2);
                    var y = (float)(center.Y - span.Height / 2 * item.Value.Item1 / 9.81 / scaleY - dot / 2);

                    g.FillEllipse(p, x, y, dot,dot);

                }
            }
            catch(Exception ex)
            {
                Font f = new Font("Arial", 10.0f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);
            }

        }
    }
}
