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
using System.Linq;
using System.Text;
using System.IO;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Logger
{
    public class TelemetryLogger
    {
        private string AnnotationDirectory = "";

        private ITelemetry master;
        private TelemetryLogWriter _logWriter;

        private int LastLaps = 0;

        public TelemetryLogger(ITelemetry telemetry_master)
        {
            master = telemetry_master;
            master.Track_Loaded += AnnotateSession;
        }

        private void AnnotateLap()
        {
            if (!Telemetry.m.Net.IsClient)
            {
                if (master.Sim.Drivers.Player.Laps != LastLaps)
                {
                    LastLaps = master.Sim.Drivers.Player.Laps;
                    _logWriter.Annotate(AnnotationDirectory + "Lap " + LastLaps + ".dat", LastLaps);
                }
            }

        }

        private void AnnotateSession(object sender)
        {
            if (!Telemetry.m.Net.IsClient)
            {
                master.Track.PlayerLap += AnnotateLap;

                _logWriter = new TelemetryLogWriter();
                _logWriter.Subscribe<ISession>("Session", master.Sim.Session);
                _logWriter.Subscribe<IDriverPlayer>("Player", master.Sim.Player);
                _logWriter.Subscribe<IDriverGeneral>("Driver", master.Sim.Drivers.Player);

                // Create directories
                int attempt = 0;

                if (!Directory.Exists("Logs/" + master.Sim.ProcessName + "/"))
                    Directory.CreateDirectory("Logs/" + master.Sim.ProcessName + "/");

                do
                {
                    AnnotationDirectory = "Logs/" + master.Sim.ProcessName + "/" + master.Track.Name + "-" +
                                          master.Sim.Session.Type.Type.ToString() + "-" +
                                          DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") +
                                          "-" + DateTime.Now.Day.ToString("00");
                    if (attempt != 0)
                        AnnotationDirectory += "-" + attempt;
                    attempt++;

                    AnnotationDirectory += "/";
                } while (Directory.Exists(AnnotationDirectory));
                Directory.CreateDirectory(AnnotationDirectory);

                _logWriter.Start(AnnotationDirectory + "Lap 0.dat", 0);
                LastLaps = 0;
            }
        }

    }
}
