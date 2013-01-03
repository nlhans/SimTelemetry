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
using System.Drawing;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using Triton;

namespace SimTelemetry.Game.rFactor2.Garage
{
    public class rFactor2Mod : IMod
    {
        private MAS2File MASFile;

        private string _name;

        private string _author;

        private string _description;

        private string _website;

        private string _version;

        private List<string> _classes;

        private string _directoryVehicles;

        private int _pitSpeedPracticeDefault;

        private int _pitSpeedRaceDefault;

        private int _opponents;

        private List<IModChampionship> _championships;

        private List<ICar> _models;

        private string _image;

        private List<string> _teams;

        public string File
        {
            get { return MASFile.Filename; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Author
        {
            get { return _author; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Website
        {
            get { return _website; }
        }

        public string Version
        {
            get { return _version; }
        }

        public List<string> Classes
        {
            get { return _classes; }
        }

        public string Directory_Vehicles
        {
            get { return _directoryVehicles; }
        }

        public int PitSpeed_Practice_Default
        {
            get { return _pitSpeedPracticeDefault; }
        }

        public int PitSpeed_Race_Default
        {
            get { return _pitSpeedRaceDefault; }
        }

        public int Opponents
        {
            get { return _opponents; }
        }

        public List<IModChampionship> Championships
        {
            get { return _championships; }
        }

        public List<ICar> Models
        {
            get { return _models; }
        }

        public string Image
        {
            get { return _image; }
        }

        public List<string> Teams
        {
            get { return _teams; }
        }

        public rFactor2Mod(MAS2File m)
        {
            this.MASFile = m;
        }

        public void Scan()
        {
            string rfmData = MASFile.Master.ExtractString(MASFile);

            _teams = new List<string>();
            _championships = new List<IModChampionship>();
            _classes = new List<string>();
            _models = new List<ICar>();

            IniScanner scan = new IniScanner { IniData = rfmData };
            scan.HandleCustomKeys += new Signal(Scan_AddTeamAndLayout);
            scan.FireEventsForKeys = new List<string>();
            scan.FireEventsForKeys.Add("Main.layout");
            scan.FireEventsForKeys.Add("Main.team");
            scan.Read();

            _name = scan.TryGetString("Mod Name");
            _version = scan.TryGetString("Mod Version");
            _opponents = scan.TryGetInt32("Max Opponents");

            _author = "Henk";
            _website = "";
            _pitSpeedPracticeDefault = scan.TryGetInt32("NormalPitKPH");
            _pitSpeedRaceDefault = scan.TryGetInt32("RacePitKPH");

            _description = "";
            _directoryVehicles = ""; // Irrelevant?!
            
            _image = "Cache/Mods/rfactor2_ " + _name+".png"; // Search&extract DDS from RFM file

            if(System.IO.File.Exists(_image)==false)
            {
                // Search in the MAS archive for SMICON.DDS
                foreach(MAS2File masf in MASFile.Master.Files)
                {
                    if(masf.Filename.Contains("SMICON"))
                    {
                        try
                        {
                            MASFile.Master.ExtractFile(masf, "tmp.dds");
                            Bitmap dds_bitmap = DevIL.DevIL.LoadBitmap("tmp.dds");
                            System.IO.File.Delete("tmp.dds");
                            DevIL.DevIL.SaveBitmap(_image, dds_bitmap);
                        }
                        catch (Exception ex)
                        {
                            // Failed

                            _image = "";
                        }
                        break;
                    }
                }

            }
            // Search for vehicles in mod directory
            // rFactor2 works with 'teams' instead of car class filters
            List<MAS2File> vehicles = rFactor2.Garage.Files.SearchFiles("*.veh");

            foreach(MAS2File f in vehicles)
            {
                rFactor2Car c = (rFactor2Car)rFactor2.Garage.CarFactory(this, f.Filename);
                c.Scan();
                if(_teams.Contains(c.Team.Trim().ToLower()))
                {
                    _models.Add(c);
                }
            }

        }

        public void AddClasses(List<string> classes)
        {
            classes.ForEach(x =>
            {
                if (_classes.Contains(x) == false)
                    _classes.Add(x);
            });
        }

        private void Scan_AddTeamAndLayout(object sender)
        {
            object[] d = (object[])sender;
            if(d[0].ToString() == "Main.layout")
            {
                // Add to tracklist.
            }
            if (d[0].ToString() == "Main.team")
            {
                string[] k = (string[])d[1];
                _teams.Add(k[0].Trim().ToLower());
            }
        }
    }
}
