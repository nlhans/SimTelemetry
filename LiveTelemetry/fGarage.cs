using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LiveTelemetry.Garage;

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

        public fGarage()
        {
            Window = GarageWindow.GameSelect;
            InitializeComponent();

            this.Padding = new Padding(0, 50, 0, 0);
            // Window: Game
            ucGame = new ucSelectGame();
            ucGame.Dock = DockStyle.Fill;
            ucGame.Close += new Triton.AnonymousSignal(ucGame_Close);
            ucGame.Chosen += new Triton.AnonymousSignal(ucGame_Chosen);

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

        void ucGame_Chosen()
        {
            Window = GarageWindow.TrackCars;
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
            IGarageUserControl uc = (IGarageUserControl)Controls[0];
            uc.Draw();

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
