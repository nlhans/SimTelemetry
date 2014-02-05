using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Repositories
{
    public class TelemetryRepository : InMemoryRepository<TelemetryLog, int>
    {
        public string DiskFile { get; private set; }

        public TelemetryRepository(string source)
        {
            DiskFile = source;

            if (File.Exists(source) )
            {
                // parse xml file
                using (var telemetryRepo = new XmlTextReader(source))
                {
                    var activeLogFile = new TelemetryLog(0, "");
                    var activeLogFileSet = false;

                    while (telemetryRepo.Read())
                    {
                        switch (telemetryRepo.Name)
                        {
                            case "log":
                                var id = int.Parse(telemetryRepo.GetAttribute("id"));
                                var file = telemetryRepo.GetAttribute("file");
                                string[] drivers = new string[0];
                                if (telemetryRepo.GetAttribute("drivers") != null)
                                    drivers = telemetryRepo.GetAttribute("drivers").Split(',');

                                activeLogFile = new TelemetryLog(id, file);
                                activeLogFileSet = true;
                                foreach (var d in drivers) if (d != "")
                                    activeLogFile.AddDriver(d);

                                Add(activeLogFile);

                                break;

                            case "lap":
                                var driver = int.Parse(telemetryRepo.GetAttribute("driver"));
                                var number = int.Parse(telemetryRepo.GetAttribute("number"));
                                var laptime = float.Parse(telemetryRepo.GetAttribute("laptime"));
                                var time = float.Parse(telemetryRepo.GetAttribute("sessiontime"));
                                var sector1 = float.Parse(telemetryRepo.GetAttribute("sector1"));
                                var sector2 = float.Parse(telemetryRepo.GetAttribute("sector2"));
                                var sector3 = float.Parse(telemetryRepo.GetAttribute("sector3"));
                                var inl = bool.Parse(telemetryRepo.GetAttribute("inlap"));
                                var outl = bool.Parse(telemetryRepo.GetAttribute("outlap"));

                                var l = new Lap(driver, true, number, time, sector1, sector2, sector3, inl, outl);

                                if (activeLogFileSet)
                                    activeLogFile.AddLap(l);
                                else
                                    GlobalEvents.Fire(new DebugWarning("Wanted to add lap without log file", null,
                                                                       "TelemetryRepository.cs"), true);

                                break;
                        }
                    }
                }
            }
        }

        public void Export()
        {
            Debug.WriteLine("Creating file " + DiskFile);
            //
            using (XmlWriter writer = XmlWriter.Create(DiskFile))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("telemetry");

                // Add all logs
                foreach(var telemetryLog in base.data.Values)
                {
                    writer.WriteStartElement("log");
                    writer.WriteAttributeString("id", telemetryLog.ID.ToString());
                    writer.WriteAttributeString("file", telemetryLog.File);
                    writer.WriteAttributeString("drivers", string.Join(",",telemetryLog.Drivers));

                    foreach(var lap in telemetryLog.Laps)
                    {
                        writer.WriteStartElement("lap");
                        writer.WriteAttributeString("driver", lap.Driver.ToString());
                        writer.WriteAttributeString("number", lap.LapNumber.ToString());
                        writer.WriteAttributeString("laptime", lap.Total.ToString());
                        writer.WriteAttributeString("sessiontime", lap.TimeStart.ToString());
                        writer.WriteAttributeString("sector1", lap.Sector1.ToString());
                        writer.WriteAttributeString("sector2", lap.Sector2.ToString());
                        writer.WriteAttributeString("sector3", lap.Sector3.ToString());
                        writer.WriteAttributeString("inlap", lap.InLap.ToString());
                        writer.WriteAttributeString("outlap", lap.OutLap.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
               

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
