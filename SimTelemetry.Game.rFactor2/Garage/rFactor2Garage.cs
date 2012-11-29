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
using System.Diagnostics;
using System.IO;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.rFactor2.Garage
{
    public class rFactor2Garage : IGarage
    {
        private List<IMod> _mods;
        private List<ITrack> _tracks;

        public string GamedataDirectory
        {
            get { return InstallationDirectory + "Installed\\"; }
        }

        public string InstallationDirectory
        {
            get
            {
                string mydocuments = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments);
                if (Directory.Exists(mydocuments + "\\rFactor2\\"))
                    return mydocuments + "\\rFactor2\\";

                return "";

            }
        }

        public string Simulator { get { return "rFactor2"; } }
        public bool Available  { get { return true; } }
        public bool Available_Tracks { get { return true; } }
        public bool Available_Mods { get { return true; }  }
        public List<IMod> Mods { get { return _mods; } }
        public List<ITrack> Tracks {  get { return _tracks; } }

        public rFactor2FileList Files { get; internal set; }

        // Cache with all data files
        private Dictionary<string, ICar> Cars = new Dictionary<string, ICar>();

        private bool Scanned = false;
        public void Scan()
        {
            if (!Scanned)
            {
                Scanned = true;

                // Index ALL files in GameDataDirectory:
                Files = new rFactor2FileList(GamedataDirectory,
                    new string[6] { ".aiw", ".gdb", ".veh", ".rfm", ".ini", ".hdv" });

                ScanTracks();
                ScanCars();
            }
        }

        private void ScanTracks()
        {
            _tracks = new List<ITrack>();
            // rFactor stores data in GDB files.
            // All relevant path data is in stored in AIW files.
            List<MAS2File> tracks = rFactor2.Garage.Files.SearchFiles("*.gdb");
            int count = 0;
            foreach (MAS2File track in tracks)
            {
                if (track.Master.ContainsFile(track.Filename.ToLower().Replace("gdb", "aiw")))
                {
                    TrackFactory(track.Filename, Path.GetDirectoryName(track.Master.File));
                }
            }

            Tracks.Sort(delegate(ITrack t1, ITrack t2)
                            {
                                int c1 = t1.Name.CompareTo(t2.Name);
                                if (c1 == 0) return t1.Version.CompareTo(t2.Version);
                                else return c1;
                            });

            Debug.WriteLine(count + " track(s) found");
        }

        private void ScanCars()
        {
            _mods = new List<IMod>();

            // rFactor stores data in GDB files.
            // All relevant path data is in stored in AIW files.
            List<MAS2File> vehicles = rFactor2.Garage.Files.SearchFiles("*.rfm");
            int count = 0;
            foreach (MAS2File mod in vehicles)
            {
                if(!mod.Master.File.Contains("allcarstracks")) // Do not parse all cars/tracks
                {
                    rFactor2Mod rf2mod = new rFactor2Mod(mod);
                    _mods.Add(rf2mod);
                    count++;
                }

            }

            Debug.WriteLine(count + " mod(s) found");
        }

        public ICar CarFactory(IMod mod, string veh)
        {
            if (veh.ToLower().EndsWith(".veh") == false) return null;
            if (!Cars.ContainsKey(veh))
            {
                Cars.Add(veh,  new rFactor2Car(veh));
                Cars[veh].Scan();
            }
            return Cars[veh];
        }

        public ITrack TrackFactory(string track, string directory)
        {
            string myversion = rFactor2Track.ParseVersion(directory);
            ITrack t = Tracks.Find(delegate(ITrack tr) { return myversion == tr.Version && track.Equals(tr.Name); });
            if (t == null)
            {
                t = new rFactor2Track(track, directory);
                t.Scan();
                Tracks.Add(t);
            }
            return t;
        }
    }
}
