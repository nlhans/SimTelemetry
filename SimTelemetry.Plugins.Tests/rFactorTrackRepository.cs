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
            Debug.WriteLine("Track::Get(\"" + id + "\")");

            string name = string.Empty, location = string.Empty;
            float laprecordRace = 0.0f, laprecordQualify = 0.0f;

            var path = new List<TrackPoint>();
            // Temporary variables: 
            TrackPoint trackPoint = null;
            TrackPointType type = TrackPointType.GRID;

            float x = 0.0f, y = 0.0f, z = 0.0f, meter = 0.0f;
            float[] boundsL = new float[0], boundsR = new float[0];
            var perpVector = new float[3] { 0.0f, 0.0f, 0.0f };

            // Parse main file.
            using (var gdbFile = new IniReader(FilePath + id, true))
            {
                gdbFile.AddHandler(setting =>
                                       {
                                           switch (setting.Key)
                                           {
                                               case "TrackName":
                                                   name = setting.ReadAsString();
                                                   break;

                                               case "Location":
                                                   location = setting.ReadAsString();
                                                   break;

                                               case "Race Laptime":
                                                   laprecordRace = setting.ReadAsFloat();
                                                   break;

                                               case "Qualify Laptime":
                                                   laprecordQualify = setting.ReadAsFloat();
                                                   break;
                                           }
                                       });
                gdbFile.Parse();
            }

            using(var aiwFile = new IniReader(FilePath+id.Replace("gdb","aiw"), true))
            {
                aiwFile.AddHandler(setting =>
                                       {
                                           if (setting.Group != "Waypoint") return;
                                           //Debug.WriteLine(setting.Key);
                                           switch(setting.Key.ToLower())
                                           {
                                               // pos = coordinates)
                                               case "wp_pos":
                                                   x = setting.ReadAsFloat(0);
                                                   z = setting.ReadAsFloat(1);
                                                   y = setting.ReadAsFloat(2);
                                                   break;

                                               // score = sector, distance
                                               case "wp_score":
                                                   switch (setting.ReadAsInteger(0))
                                                   {
                                                       case 0:
                                                           type = TrackPointType.SECTOR1;
                                                           break;

                                                       case 1:
                                                           type = TrackPointType.SECTOR2;
                                                           break;

                                                       case 2:
                                                           type = TrackPointType.SECTOR3;
                                                           break;
                                                   }
                                                   meter = setting.ReadAsInteger(1);
                                                   break;

                                               // branchID = path ID, 0=main, 1=pitlane
                                               case "wp_branchid":

                                                   if (setting.ValueCount== 1)
                                                   {
                                                       switch (setting.ReadAsString(0))
                                                       {
                                                           case "0":
                                                               // do nothing
                                                               break;
                                                           case "1":
                                                               type = TrackPointType.PITS;
                                                               break;

                                                           default:
                                                               type = TrackPointType.GRID;
                                                               break;
                                                       }
                                                   }
                                                   else
                                                       type = TrackPointType.GRID;
                                                   break;

                                               case "wp_perp":

                                                   perpVector = new float[3]
                                                                           {
                                                                               setting.ReadAsFloat(0),
                                                                               setting.ReadAsFloat(1),
                                                                               setting.ReadAsFloat(2)
                                                                           };
                                                   break;

                                               case "wp_width":
                                                   boundsL = new float[2]
                                                                        {
                                                                            x - perpVector[0]*setting.ReadAsFloat(0),
                                                                            y - perpVector[2]*setting.ReadAsFloat(0)
                                                                        };
                                                   boundsR = new float[2]
                                                                        {
                                                                            x + perpVector[0]*setting.ReadAsFloat(1),
                                                                            y + perpVector[2]*setting.ReadAsFloat(1)
                                                                        };
                                                   break;

                                               // ptrs = next path, previous path, pitbox route (-1 for no pitbox), following branchID
                                               case "wp_ptrs":
                                                   path.Add(new TrackPoint(meter, type, x, y, z, boundsL, boundsR));
                                                   break;

                                           }
                                       });
                aiwFile.Parse();
            }


            var t = new Track(id, name, name.Replace(" ","_")+".png", location, laprecordRace, laprecordQualify, "1.0");
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