using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SimTelemetry.Data;
using SimTelemetry.Objects;
using Triton;

namespace LiveTelemetry
{
    public class DataLogging
    {
        private Thread SessionMonitor;
        private event AnonymousSignal SessionChanged;

        private DataCollector collector;

        public DataLogging()
        {
            SessionMonitor = new Thread(tSessionMonitor);
            SessionMonitor.IsBackground = true;
            SessionMonitor.Start();

            collector = new DataCollector();
            SessionChanged += new AnonymousSignal(DataLogging_SessionChanged);
        }

        void DataLogging_SessionChanged()
        {
            return;
            if (!Telemetry.m.Active_Session) return;
            string track_location = "";
            string track_length = "";
            string track_type = "";
            string track_name = "";
            try
            {
                string gdb = Telemetry.m.Sim.Session.GameDirectory + Telemetry.m.Sim.Session.CircuitName.Replace(".AIW", ".gdb");
                gdb = gdb.Replace(".aiw", ".gdb");

                // alright open it
                string[] data = File.ReadLines(gdb).ToArray();


                foreach (string data_line in data)
                {
                    string[] spl = data_line.Split("=".ToCharArray());

                    if (data_line.Contains("TrackName"))
                        track_name = spl[1].Trim();


                    if (data_line.Contains("Location"))
                        track_location = spl[1].Trim();


                    if (data_line.Contains("Length"))
                        track_length = spl[1].Trim();


                    if (data_line.Contains("TrackType"))
                        track_type = spl[1].Trim();

                }


            }
            catch (Exception)
            {
                track_name = Path.GetFileNameWithoutExtension(Telemetry.m.Sim.Session.CircuitName);
            }
            if (track_name == "") return;
            int i = 1;
            string file = "";
            do
            {
                file = "Logs/" + track_name + "+" + Session2String(Telemetry.m.Sim.Session.Type) + "+" +
                       DateTime.Now.ToShortDateString().Replace("/", "-") + "+" + i + "+Lap " +
                       Telemetry.m.Sim.Drivers.Player.Laps +
                       ".dat";
                i++;
            } while (File.Exists(file));

            string last_file = collector.GetFile();
            collector.Stop();

            collector.CreateLog(file);
            collector.Run();

            // last file
            if (last_file != "" && File.Exists(last_file))
            {
                string[] lines = File.ReadAllLines(last_file);
                if (lines.Contains("[Lap]") == false)
                    File.Delete(last_file);
            }
        }

        private string Session2String(SessionInfo type)
        {
            switch(type.Type)
            {
                case SessionType.WARMUP:
                    return "Warm up";
                    break;

                    case SessionType.TEST_DAY:
                    return "Test session";
                    break;

                    case SessionType.PRACTICE:
                    return "Free practice " + type.Number;
                    break;

                case SessionType.RACE:
                    return "Race";
                    break;
                    case SessionType.QUALIFY:
                    return "Qualifying " + type.Number;
                    break;

                default:
                    return "Dunno";
                    break;
            }
        }

        private void tSessionMonitor()
        {
            
            Thread.Sleep(1);
            SessionInfo sessionOld = new SessionInfo();
            int lastlaps = -1;
            sessionOld.Type = SessionType.TEST_DAY;
            
            while(true)
            {
                if (Telemetry.m.Active_Session)
                {
                    SessionInfo sessionCurrent = Telemetry.m.Sim.Session.Type;

                    if (sessionCurrent.Type != sessionOld.Type || sessionCurrent.Number != sessionOld.Number ||
                        lastlaps != Telemetry.m.Sim.Drivers.Player.Laps)
                        if (SessionChanged != null)
                        {
                            SessionChanged();
                            lastlaps = Telemetry.m.Sim.Drivers.Player.Laps;
                            sessionOld = sessionCurrent;
                        }
                }


                Thread.Sleep(50);
            }

        }
    }
}
