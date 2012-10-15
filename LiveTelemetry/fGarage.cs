using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveTelemetry.Garage;
using SimTelemetry.Data;
using SimTelemetry.Objects;

namespace LiveTelemetry
{
    public enum GarageWindow
    {
        GameSelect,
        TrackCars,
        Mod,
        Car,
        Engine,
        Aerodynamics,
        Tyres,
        Gearbox,
        Analysis
    }

    public partial class fGarage : Form
    {
        private GarageWindow Window;

        private ucSelectGame ucGame;
        private ucSelectTrackCars ucTrackCars;
        private Button btBack;

        public static ISimulator Sim { get; set; }

        public fGarage()
        {
            Window = GarageWindow.GameSelect;
            InitializeComponent();

            this.Padding = new Padding(0, 50, 0, 0);
            // Window: Game
            ucGame = new ucSelectGame();
            ucGame.Dock = DockStyle.Fill;
            ucGame.Chosen += new Triton.Signal(ucGame_Chosen);

            ucTrackCars = new ucSelectTrackCars();
            ucTrackCars.Dock = DockStyle.Fill;

            // Button Back
            btBack = new Button();
            btBack.Text = "Back";
            btBack.Size = new Size(50, 25);
            btBack.Location = new Point(10, 10);
            btBack.BackColor = Color.White;
            btBack.Click += new EventHandler(btBack_Click);

            Resize += fGarage_Resize;
            Redraw();
        }

        void ucGame_Chosen(object sim)
        {
            Sim = Telemetry.m.Sims.Sims.Find(delegate(ISimulator s) { return s.Name.Equals(sim.ToString()); });
            if (Sim.Garage == null)
                // TODO: Display errors.
                // TODO: Check if sim is installed.
                Window = GarageWindow.GameSelect;
            else
            {
                Window = GarageWindow.TrackCars;
                Sim.Garage.Scan();
            }

            Redraw();
        }

        private void ActionBack()
        {
            switch(Window)
            {
                case GarageWindow.GameSelect:
                    this.Close();
                    break;

                case GarageWindow.TrackCars:
                    Window = GarageWindow.GameSelect;
                    break;
            }
            if (Window == GarageWindow.GameSelect)
                Sim = null;
            Redraw();
        }

        void btBack_Click(object sender, EventArgs e)
        {
            ActionBack();
        }

        void ucGame_Close()
        {
            ActionBack();
        }

        void Redraw()
        {
            if (Controls.Count > 0)
                ((IGarageUserControl) Controls[0]).Close -= new Triton.AnonymousSignal(ucGame_Close);
            Controls.Clear();
            BackColor = Color.Black;
            switch(Window)
            {
                case GarageWindow.GameSelect:
                    Controls.Add(ucGame);
                    break;

                case GarageWindow.TrackCars:
                    Controls.Add(ucTrackCars);
                    break;
            }

            ((IGarageUserControl)Controls[0]).Close += new Triton.AnonymousSignal(ucGame_Close);
            ((IGarageUserControl)Controls[0]).Draw();

            if(GarageWindow.GameSelect != Window)
                Controls.Add(btBack);
        }

        void fGarage_Resize(object sender, EventArgs e)
        {
            IGarageUserControl uc = (IGarageUserControl) Controls[0];
            uc.Draw();
        }

    }
}
