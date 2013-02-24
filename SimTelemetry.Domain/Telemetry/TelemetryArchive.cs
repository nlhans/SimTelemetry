using System;
using System.IO;
using System.Linq;
using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryArchive : IFileAnnotater
    {
        public bool QualifiesForStorage(TelemetryLogger logger)
        {
            return (logger.TimeLine.Max() - logger.TimeLine.Min()) >= 10000;
        }

        public string GetPath(TelemetryLogger logger)
        {
            var track = (logger.Track == null)?"Track" : logger.Track.Name;
            var session = (logger.Session == null)?"Session" : logger.Session.Type.ToString();
            var simulator = logger.Simulator;
            var date = DateTime.Now.Year.ToString("0000") + "-" +
                       DateTime.Now.Month.ToString("00") + "-" +
                       DateTime.Now.Day.ToString("00") + "-" +
                       DateTime.Now.Hour.ToString("00") + "-" +
                       DateTime.Now.Minute.ToString("00");

            var suggestedPath = "Logs/" + simulator + "/" + track + "/" + date + " - " + session +".zip";

            Path.GetInvalidPathChars().ToList().ForEach(x => suggestedPath = suggestedPath.Replace(x, '_'));

            return suggestedPath;
        }

        public void Store(TelemetryLogger logger, LogFileWriter writer)
        {
            var suggestedPath = GetPath(logger);

            // Any directories that do not exist yet?
            var directoryParts = suggestedPath.Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var cumalativeParts = new string[directoryParts.Length];
            var i = 0;
            foreach(var part in directoryParts)
            {
                if (i + 1 == directoryParts.Length) break;
                if (i > 0)
                    cumalativeParts[i] = cumalativeParts[i - 1] + "/" + part;
                else
                    cumalativeParts[i] = part;

                if (Directory.Exists(cumalativeParts[i]) == false)
                    Directory.CreateDirectory(cumalativeParts[i]);
                i++;
            }

            // TODO: get configuration file Telemetry directory
            File.Move("./tmp.zip", suggestedPath);
        }
    }
}
