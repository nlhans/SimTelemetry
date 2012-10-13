using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorGarage : IGarage
    {
        private List<IGarageMod> _mods;
        private List<IGarageTrack> _tracks;

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
        public List<IGarageMod> Mods { get { return _mods; } }
        public List<IGarageTrack> Tracks {  get { return _tracks; } }

        public void Scan()
        {
            ScanTracks();
            ScanCars();
        }

        private List<string> SearchFiles(string directory)
        {
            List<string> files = new List<string>();
            foreach(string file in Directory.GetFiles(directory, "*.gdb", SearchOption.AllDirectories))
                files.Add(file);
            return files;
        }

        private void ScanTracks()
        {
            _tracks = new List<IGarageTrack>();
            // rFactor stores data in GDB files.
            // All relevant path data is in stored in AIW files.
            List<string> tracks = SearchFiles(GamedataDirectory);
            int count = 0;
            foreach(string track in tracks)
            {
                // If AIW file exists
                if(File.Exists(track.Replace(".gdb",".aiw")))
                {
                    _tracks.Add(new rFactorTrack(track));
                    _tracks[count].Scan();
                    count++;
                }
            }

            _tracks[0].Scan();
            Debug.WriteLine(count + " track(s) found");
        }

        private void ScanCars()
        {
            _mods = new List<IGarageMod>();

        }
    }
}
