using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorCarAerodynamics : ICarAerodynamics
    {
        private IniScanner _hdv;
        public rFactorCarAerodynamics(IniScanner hdv)
        {
            _hdv = hdv;

            if (_hdv.Data.ContainsKey("FRONTWING"))
            {
                List<double> fw_factor = new List<double>();
                foreach (string d in _hdv.TryGetData("FRONTWING", "FWDragParams"))
                    fw_factor.Add(double.Parse(d));

                Drag_FrontWing = new Polynomial(fw_factor);
            }
            if (_hdv.Data.ContainsKey("REARWING"))
            {
                List<double> fw_factor = new List<double>();
                foreach (string d in _hdv.TryGetData("REARWING", "RWDragParams"))
                    fw_factor.Add(double.Parse(d));

                Drag_RearWing = new Polynomial(fw_factor);
            }
            if (_hdv.Data.ContainsKey("LEFTFENDER"))
            {
                List<double> fw_factor = new List<double>();
                foreach (string d in _hdv.TryGetData("LEFTFENDER", "FenderDragParams"))
                    fw_factor.Add(double.Parse(d));

                Drag_LeftFender = new Polynomial(fw_factor);
            }
            if (_hdv.Data.ContainsKey("RIGHTFENDER"))
            {
                List<double> fw_factor = new List<double>();
                foreach (string d in _hdv.TryGetData("RIGHTFENDER", "FenderDragParams"))
                    fw_factor.Add(double.Parse(d));

                Drag_RightFender = new Polynomial(fw_factor);
            }

            Drag_Body = hdv.TryGetDouble("BODYAERO", "BodyDragBase");
            if (Drag_Body == 0)
                Drag_Body = double.Parse(hdv.TryGetData("BODYAREO", "BodyDragBase")[0]);
            Drag_BodyHeightAvg =  hdv.TryGetDouble("BODYAREO", "BodyDragHeightAvg");
            Drag_BodyHeightDiff =hdv.TryGetDouble("BODYAREO", "BodyDragHeightDiff");

            Drag_Radiator = new Polynomial(0,hdv.TryGetDouble("BODYAREO", "RadiatorDrag"));
            Drag_BrakesDuct = new Polynomial(0,hdv.TryGetDouble("BODYAREO", "BrakeDuctDrag"));
        }

        public string File
        {
            get { return _hdv.IniFile; }
        }

        public double GetAerodynamicDrag(ISetup setup)
        {
            double fw = 0, fenders = 0, rw = 0;

             // Frontwing
            if (Drag_FrontWing != null)
                fw = Drag_FrontWing.Calculate(setup.Aero_FrontWing);

            // L/R Fenders
            if (Drag_LeftFender != null)
                fenders += Drag_LeftFender.Calculate(setup.Aero_FenderLeft);
            if (Drag_RightFender != null)
                fenders += Drag_RightFender.Calculate(setup.Aero_FenderRight);

            // Rear wing
            if (Drag_RearWing != null)
                rw = Drag_RearWing.Calculate(setup.Aero_RearWing);

            double body = Drag_Body;

            double Height_Front = 0.5*(RideHeight_LF + RideHeight_RF);
            double Height_Rear = 0.5*(RideHeight_LR + RideHeight_RR);

            // TODO: Is this calculate correct??
            double Body_Height_Diff = Height_Front + Height_Rear;
            double bodyheight = (Body_Height_Diff)*0.5*Drag_BodyHeightAvg + Body_Height_Diff*Drag_BodyHeightDiff;

            // Radiator
            double radiator = Drag_Radiator.Calculate(setup.Engine_RadiatorSize);

            // Brake ducts
            double brakes = Drag_BrakesDuct.Calculate(setup.Brakes_DuctSize);

            // general formula: BodyDragBase + BrakeDuctSetting*BrakeDuctDrag + RadiatorSetting*RadiatorDrag + BodyDragHeightAvg*ARH + BodyDragHeightDiff*Rake
            // http://isiforums.net/f/showthread.php/287-Differences-in-aero-calculations-in-CarFactory-vs-rFactor-telemetry
            // http://koti.mbnet.fi/tspartan/gp1975/airoopas/index.php?id=functions.php)
            return fw + bodyheight + body + rw + fenders + radiator + brakes;

        }

        protected Polynomial Drag_FrontWing { get; set; }
        protected Polynomial Drag_LeftFender { get; set; }
        protected Polynomial Drag_RightFender { get; set; }
        protected Polynomial Drag_RearWing { get; set; }
        protected double Drag_Body { get; set; }
        protected double RideHeight_LF { get; set; }
        protected double RideHeight_RF { get; set; }
        protected double RideHeight_LR { get; set; }
        protected double RideHeight_RR { get; set; }
        protected double Drag_BodyHeightAvg { get; set; }
        protected double Drag_BodyHeightDiff { get; set; }
        protected Polynomial Drag_Radiator { get; set; }
        protected Polynomial Drag_BrakesDuct { get; set; }
    }
}