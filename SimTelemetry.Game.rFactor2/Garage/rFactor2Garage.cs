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
                Files = new rFactor2FileList(GamedataDirectory);

                ScanTracks();
                ScanCars();
            }
        }

        private void ScanTracks()
        {
            _tracks = new List<ITrack>();
            // rFactor stores data in GDB files.
            // All relevant path data is in stored in AIW files.
            List<MAS2File> tracks = rFactor2.Garage.Files.SearchFiles(GamedataDirectory, "*.gdb");
            int count = 0;
            foreach (MAS2File track in tracks)
            {

            }

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
    }
}
