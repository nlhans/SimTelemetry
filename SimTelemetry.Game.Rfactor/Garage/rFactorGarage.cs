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
using System.Linq;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorGarage : IGarage
    {
        private List<IMod> _mods;
        private List<ITrack> _tracks;

        public string GamedataDirectory
        {
            get { return InstallationDirectory + "GameData\\"; }
        }

        public string InstallationDirectory
        {
            get
            {
                string program_files = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles);
                if (Directory.Exists(program_files + "\\rfactor\\"))
                    return program_files + "\\rfactor\\";

                program_files = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                if (Directory.Exists(program_files + "\\rfactor\\"))
                    return program_files + "\\rfactor\\";

                return "";

            }
        }

        public string Simulator { get { return "rFactor"; } }
        public bool Available  { get { return true; } }
        public bool Available_Tracks { get { return true; } }
        public bool Available_Mods { get { return true; }  }
        public List<IMod> Mods { get { return _mods; } }
        public List<ITrack> Tracks {  get { return _tracks; } }

        public FileList Files { get; internal set; }

        // Cache with all data files
        private Dictionary<string, ICar> Cars = new Dictionary<string, ICar>();

        private bool ScannedCars = false;
        private bool ScannedTracks = false;
        private bool Scanned = false;
        public void Scan()
        {
            if (!Scanned)
            {
                // Index ALL files in GameDataDirectory:
                Files = new FileList(GamedataDirectory,
                    new string[] { ".aiw", ".gdb", ".veh", ".rfm", ".ini", ".hdv" , ".tbc"});

                Scanned = true;

                ScanTracks();
                ScanCars();


            }
        }

        private void ScanTracks()
        {
            if (!ScannedTracks)
            {
                if (Scanned == false) Scan();

                _tracks = new List<ITrack>();
                // rFactor stores data in GDB files.
                // All relevant path data is in stored in AIW files.
                List<string> tracks = rFactor.Garage.Files.SearchFiles(GamedataDirectory, "*.gdb");
                int count = 0;
                foreach (string track in tracks)
                {
                    // If AIW file exists
                    if (File.Exists(track.Replace(".gdb", ".aiw")))
                    {
                        _tracks.Add(new rFactorTrack(track));
                        count++;
                    }
                }

                Debug.WriteLine(count + " track(s) found");
                ScannedTracks = true;
            }
        }

        private void ScanCars()
        {
            if (!ScannedCars)
            {
                if (Scanned == false) Scan();

                // rFactor stores data in GDB files.
                // All relevant path data is in stored in AIW files.
                List<string> vehicles = GarageTools.SearchFiles(InstallationDirectory + "rFM\\", "*.rfm");

                _mods = new List<IMod>();
                foreach (string mod in vehicles.Where(x => !x.Contains("ANY_DEV_ONLY")))
                    _mods.Add(new rFactorMod(mod));

                ScannedCars = true;
            }
        }

        public ICar CarFactory(IMod mod, string veh)
        {
            lock (Cars)
            {
                if (!Cars.ContainsKey(veh))
                {
                    Cars.Add(veh, new rFactorCar(mod, veh));
                    Cars[veh].Scan();
                }
            }
            return Cars[veh];
        }

        public ICar SearchCar(string CarClass, string CarModel)
        {
            if (ScannedCars==false)
                ScanCars();

            _mods.ForEach(x =>
                              {
                                  if (x != null)
                                  {
                                      x.Scan();
                                  }
                              }
                );

            foreach(rFactorMod mod in _mods.Where(x =>
                                                      {

                                                          if (x == null) return false;
                                                          return x.Classes.Contains(CarModel) ||
                                                                 x.Classes.Contains(CarClass) ||
                                                                 x.Models.Any(y => y.Team.Equals(CarModel)) ||
                                                                 x.Models.Any(y => y.Team.Equals(CarClass));
                                                      }))
            {
                if( mod.Models.Any(x => x.Team == CarModel))
                {
                    return mod.Models.Where(x => x.Team == CarModel).FirstOrDefault();
                }
                IEnumerable<ICar> UniqueCarHdvs =
                    mod.Models.Where(x => x.Classes.Contains(CarModel) || x.Classes.Contains(CarClass))
                    .Distinct(new LambdaComparer<ICar>((a, b) => { return a.PhysicsFile.Equals(b.PhysicsFile); }));
                int uniquecars = UniqueCarHdvs.Count();
                if (uniquecars== 1)
                    return mod.Models.FirstOrDefault();

            }
            return null;
        }

        public ITrack SearchTrack(string path)
        {
            if (ScannedTracks == false)
                ScanTracks();
            path = path.ToLower();
            path = Path.GetFileNameWithoutExtension(path);
            if(_tracks.Count(x=>x.File.Contains(path)) > 0)
            return _tracks.Where(x => x.File.Contains(path)).FirstOrDefault();
            else
            {
                return _tracks.Where(x =>
                                         {
                                             x.Scan();
                                             return x.Name.ToLower().Contains(path);
                                         }).FirstOrDefault();
            }
        }
    }
}
