using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Utils;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactorTrackRepository : ILazyRepositoryDataSource<Track, string>
    {
        string FilePath = @"C:\Program Files (x86)\rFactor\GameData\Locations\";

        public IList<string> GetIds()
        {
            string[] files = Directory.GetFiles(FilePath, "*.gdb", SearchOption.AllDirectories);
            var fileList = new List<string>(files);
            return fileList.Select(x => x.Substring(FilePath.Length).ToLower()).ToList();
        }

        public bool Add(Track obj)
        {
            return false;
        }

        public bool Store(Track obj)
        {
            return false;
        }

        public Track Get(string id)
        {
            System.Threading.Thread.Sleep(10);
            Debug.WriteLine("Track::Get(\"" + id + "\")");
            //
            var scan = new IniScanner { IniFile = FilePath + id };
            scan.Read();

            // Read data:
            var name = scan.TryGetString("TrackName");
            var location = scan.TryGetString("Location");
            var laprecordRace = 0.0f;
            var laprecordQualify = 0.0f;
            float.TryParse(scan.TryGetString("Race Laptime").Trim(), out laprecordRace);
            float.TryParse(scan.TryGetString("Qualify Laptime").Trim(), out laprecordQualify);

            // Read track path:
            var path = new List<TrackPoint>();
            TrackPoint trackPoint = null;
            
            // Temporary variables: 
            float x = 0.0f, y = 0.0f, z = 0.0f, meter = 0.0f;
            TrackPointType type = TrackPointType.GRID;
            float[] boundsL = new float[0], boundsR = new float[0];
            var perpVector = new float[3] { 0.0f, 0.0f, 0.0f };

            var track_aiw = new IniScanner { IniFile = FilePath + id.Replace("gdb", "aiw") };
            track_aiw.HandleCustomKeys += (d) =>
                                              {

                                                  var a = (object[]) d;
                                                  var key = (string) a[0];
                                                  var values = (string[]) a[1];


                                                  switch (key)
                                                  {
                                                      // pos = coordinates)
                                                      case "Main.wp_pos":
                                                          x = Convert.ToSingle(values[0]);
                                                          z = Convert.ToSingle(values[1]);
                                                          y = Convert.ToSingle(values[2]);
                                                          break;

                                                      // score = sector, distance
                                                      case "Main.wp_score":
                                                          var sector = Convert.ToInt32(values[0]) + 1;
                                                          if (sector == 1) type = TrackPointType.SECTOR1;
                                                          if (sector == 2) type = TrackPointType.SECTOR2;
                                                          if (sector == 3) type = TrackPointType.SECTOR3;
                                                          meter = Convert.ToSingle(values[1]);
                                                          break;

                                                      // branchID = path ID, 0=main, 1=pitlane
                                                      case "Main.wp_branchid":
                                                          if (values.Length == 1)
                                                          {
                                                              switch (values[0])
                                                              {
                                                                  case "1":
                                                                      type = TrackPointType.PITS;
                                                                      break;

                                                                  default:
                                                                      // TODO: Double check this.
                                                                      type = TrackPointType.GRID;
                                                                      break;
                                                              }
                                                          }
                                                          else
                                                              type = TrackPointType.GRID;
                                                          break;

                                                      case "Main.wp_perp":

                                                          perpVector = new float[3]
                                                                           {
                                                                               Convert.ToSingle(values[0]),
                                                                               Convert.ToSingle(values[1]),
                                                                               Convert.ToSingle(values[2])
                                                                           };
                                                          break;

                                                      case "Main.wp_width":
                                                          boundsL = new float[2]
                                                                        {
                                                                            x - perpVector[0]*Convert.ToSingle(values[0]),
                                                                            y - perpVector[2]*Convert.ToSingle(values[0])
                                                                        };
                                                          boundsR = new float[2]
                                                                        {
                                                                            x + perpVector[0]*Convert.ToSingle(values[1]),
                                                                            y + perpVector[2]*Convert.ToSingle(values[1])
                                                                        };
                                                          break;

                                                      // ptrs = next path, previous path, pitbox route (-1 for no pitbox), following branchID
                                                      case "Main.wp_ptrs":
                                                          path.Add(new TrackPoint(meter, type, x, y, z, boundsL, boundsR));
                                                          break;

                                                  }
                                              };
            track_aiw.FireEventsForKeys = new List<string>();
            track_aiw.FireEventsForKeys.AddRange(new string[6]
                                                         {
                                                             "Main.wp_pos", "Main.wp_score", "Main.wp_branchid",
                                                             "Main.wp_perp", "Main.wp_width", "Main.wp_ptrs"
                                                         });
            track_aiw.Read();

            var t = new Track(id, name, location, laprecordRace, laprecordQualify);
            t.SetTrack(path);

            
            return t;
        }

        public bool Remove(string Id)
        {
            return false;
        }

        public bool Clear()
        {
            return false;
        }
    }
}