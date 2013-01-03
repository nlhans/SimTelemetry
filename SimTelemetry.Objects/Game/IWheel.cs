using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects.Game
{
    public interface ISetupWheel
    {
        double Rideheight { get; set; }
        double Pressure { get; set; }
        double Temperature { get; set; }
        
        string Compound { get; set; }
        double BrakeThickness { get; set; }
    }

    public interface IWheel
    {
        [Loggable(1)]
        double Wear { get; set; }

        [Loggable(25)]
        double Temperature_Inside { get; set; }
        [Loggable(25)]
        double Temperature_Middle { get; set; }
        [Loggable(25)]
        double Temperature_Outside { get; set; }

        [Loggable(1)]
        double Pressure { get; set; }

        [Loggable(25)]
        double Rideheight { get; set; }

        [LogOnChange]
        bool Flat { get; set; }

        [LogOnChange]
        bool Detached { get; set; }
    }
}
