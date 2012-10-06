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

            master.Track_Loaded += new Triton.Signal(AnnotateSession);
        }

        void master_Track_Load(object sender)
        {

        }

        private void AnnotateLap()
        {
            if (master.Sim.Drivers.Player.Laps != LastLaps)
            {
                LastLaps = master.Sim.Drivers.Player.Laps;
                _logWriter.Annotate(AnnotationDirectory + "Lap " + LastLaps + ".dat", LastLaps);
            }
        }

        private void AnnotateSession(object sender)
        {
            master.Track.DriverLap += new Triton.AnonymousSignal(AnnotateLap);

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
                AnnotationDirectory = "Logs/" + master.Sim.ProcessName + "/" + master.Track.Name + "-" + master.Sim.Session.Type.Type.ToString() + "-" + DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00");
                if (attempt != 0)
                    AnnotationDirectory += "-" + attempt;
                attempt++;

                AnnotationDirectory += "/";
            }
            while (Directory.Exists(AnnotationDirectory));
            Directory.CreateDirectory(AnnotationDirectory);

            _logWriter.Start(AnnotationDirectory + "Lap 0.dat", 0);
            LastLaps = 0;
        }

    }
}
