using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SimTelemetry.Data.Logger;

namespace SimTelemetry
{
    public class ReplaySFX
    {
        private TelemetryLogReplay _mMaster;
        private List<EngineRpmRegion> Regions = new List<EngineRpmRegion>();

        public double Throttle_LoadBlend_High = 1;
        public double Throttle_LoadBlend_Low = 0;

        public EngineRpmRegion GetNext(EngineRpmRegion reg)
        {
            int i = reg.offset;
            foreach (EngineRpmRegion r in Regions)
            {
                if (r.offset == i + 1 && r.type == reg.type) return r;

            }
            return null;
        }

        public EngineRpmRegion GetPrevious(EngineRpmRegion reg)
        {
            int i = reg.offset;
            foreach (EngineRpmRegion r in Regions)
            {
                if (r.offset == i - 1 && r.type == reg.type) return r;

            }
            return null;
        }

        private Form _mForm;
        double MaxEngineRegionVolume = 0;
        string Sound_Directory = @"C:\Program Files (x86)\rFactor\GameData\Sounds\";
        private SoundPlayer TractionControlPlayer;
        public ReplaySFX(Form f, TelemetryLogReplay replay)
        {
            _mForm = f;
            MaxEngineRegionVolume = 0;

            // read the file!
            string SFX_File = @"C:\Program Files (x86)\rFactor\GameData\Vehicles\F1_2010_CODEMASTERS\2010\All_Teams\Vodafone McLaren\McLaren_Sounds10.sfx";
            SFX_File = @"C:\Program Files (x86)\rFactor\GameData\Vehicles\Rayzor\RC.sfx";
            if(!File.Exists(SFX_File))
            {
                MessageBox.Show("Can't find audio file.");
                return;
            }
            string[] data = File.ReadAllLines(SFX_File);
            Dictionary<string, string> keys = new Dictionary<string, string>();
            for (int i = 0; i < data.Length; i++)
            {
                string d = data[i];
                if (d.Trim().Length > 1 && d.Contains("=") && d.StartsWith("//") == false)
                {
                    if (d.Contains("//"))
                        d = d.Substring(0, d.IndexOf("//")).Trim(); // remove comments
                    string[] ds = d.Split("=".ToCharArray(), 2);
                    if (i + 4 < data.Length && data[i + 1].Trim() == "{") // next line is { , then it means we get a block.
                    {
                        if (ds[0].Contains("Inside")) // we don't do outside noises
                        {
                            EngineRpmRegion r = new EngineRpmRegion();
                            r._mMaster = replay;
                            r.offset = Convert.ToInt32(ds[1]);
                            r.SFX = this;
                            r.type = ((ds[0].Contains("Coast"))
                                          ? EngineRpmRegionType.COAST
                                          : EngineRpmRegionType.POWER);
                            // engine block!
                            // parse this
                            for (int j = i + 1; j < i + 5; j++) // walk through the lines
                            {
                                string sd = data[j];
                                if (sd != "}" && sd != "{")
                                {
                                    if (sd.Contains("//"))
                                        sd = sd.Substring(0, sd.IndexOf("//")).Trim();
                                    string[] sds = sd.Split("=".ToCharArray());
                                    double v = Convert.ToDouble(sds[1]);
                                    switch (sds[0])
                                    {
                                        case "MinimumRPM":
                                            r.Min = v;
                                            break;
                                        case "MaximumRPM":
                                            r.Max = v;
                                            break;
                                        case "NaturalRPM":
                                            r.Nat = v;
                                            break;
                                    }
                                }
                            }
                            FinishRPMRegion(keys, r);
                        }
                        i += 5;
                    }
                    else
                    {
                        if (ds[0].StartsWith("EngRPM") && ds[0].Contains("Inside") && ds[1].Contains("("))
                        {
                            string[] region_data =
                                ds[1].Replace(" ", "").Replace("(", "").Replace(")", "").Split(",".ToCharArray());

                            EngineRpmRegion r = new EngineRpmRegion();
                            r._mMaster = replay;
                            r.offset = Convert.ToInt32(region_data[0]);
                            r.SFX = this;
                            r.type = ((ds[0].Contains("Coast"))
                                          ? EngineRpmRegionType.COAST
                                          : EngineRpmRegionType.POWER);
                            r.Min = Convert.ToDouble(region_data[1]);
                            r.Max = Convert.ToDouble(region_data[2]);
                            r.Nat = Convert.ToDouble(region_data[3]);

                            FinishRPMRegion(keys, r);
                        }
                        if (keys.ContainsKey(ds[0]) == false)
                        {
                            keys.Add(ds[0], ds[1]);
                        }
                        else
                        {
                        }
                    }

                }
            }

            // Do load blend
            string EngineLoadBlendInside = keys["EngineLoadBlendInside"].Replace("(", "").Replace(")", "");
            string[] EngineLoadBlendInside_Split = EngineLoadBlendInside.Split(",".ToCharArray());

            this.Throttle_LoadBlend_Low = Convert.ToDouble(EngineLoadBlendInside_Split[0]);
            this.Throttle_LoadBlend_High = Convert.ToDouble(EngineLoadBlendInside_Split[1]);

            for (int i = 0; i < Regions.Count; i++)
                Regions[i].VolumeMultiplier /= MaxEngineRegionVolume;

            // shift noises!
            Timer t = new Timer();
            t.Interval = 1;
            t.Tick += new EventHandler(t_Tick);
            t.Start();

            foreach (KeyValuePair<string, string> k in keys)
            {
                if (k.Key.StartsWith("VS_INSIDE_SHIFT_UP_"))
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Sound_Directory + k.Value);
                    UpShifts.Add(sp);
                }
                if (k.Key.StartsWith("VS_INSIDE_SHIFT_DOWN_"))
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Sound_Directory + k.Value);
                    DownShifts.Add(sp);
                }
                if (k.Key.StartsWith("VS_INSIDE_BACKFIRE_"))
                {
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Sound_Directory + k.Value);
                    Backfire.Add(sp);
                }
            }

            // traction control sound
            if (keys.ContainsKey("VS_INSIDE_TRACTION_CONTROL"))
            {
                TractionControlPlayer = new SoundPlayer(_mForm.Controls[0], SoundPlayer.PullAudio, Sound_Directory + keys["VS_INSIDE_TRACTION_CONTROL"], 1);
                TractionControlPlayer.PullAFrequency = Unity;
                TractionControlPlayer.PullVolume = GetTractionControlVol;
                TractionControlPlayer.Resume();
            }

        }

        private double GetTractionControlVol()
        {
            return 0;

        }

        private double Unity()
        {
            return 1;
        }

        private List<System.Media.SoundPlayer> UpShifts = new List<System.Media.SoundPlayer>();
        private List<System.Media.SoundPlayer> DownShifts = new List<System.Media.SoundPlayer>();
        private List<System.Media.SoundPlayer> Backfire = new List<System.Media.SoundPlayer>();
        private int previous_gear = 0;
        void t_Tick(object sender, EventArgs e)
        {
            return;
            /*
            int gear = _mMaster.GetInt32("Driver.Gear");
            if (gear == 0) return;
            int dgear = gear - previous_gear;
            try
            {
                //if (dgear == 1)
                //    PlayUpShift();
                //if (dgear == -1)
                //    PlayDownShift();

                Random r = new Random();
                int amx = 5000 + Convert.ToInt32(15000 * UserThrottle.Get());
                int va = r.Next(0, amx);
                if (va < 10)
                {
                    int index = r.Next(0, Backfire.Count);
                    Backfire[index].Play();
                }

            }
            catch (Exception ex)
            {
            }

            previous_gear = gear;
             * */
        }

        private void PlayUpShift()
        {
            Random r = new Random();
            int index = r.Next(0, UpShifts.Count);
            UpShifts[index].Play();
        }

        private void PlayDownShift()
        {
            Random r = new Random();
            int index = r.Next(0, DownShifts.Count);

            DownShifts[index].Play();
        }

        private void FinishRPMRegion(Dictionary<string, string> keys, EngineRpmRegion r)
        {
            if (r.file == "" || r.file == null)
            {
                // Look for the file..
                string sound_file = "";
                string file_key = "VS_INSIDE_" + ((r.type == EngineRpmRegionType.COAST) ? "COAST" : "POWER") +
                                  "_ENGINE_" + (r.offset + 1);
                if (keys.ContainsKey(file_key))
                {
                    sound_file = keys[file_key];
                    if (sound_file.Contains(","))
                    {
                        string[] sound_file_split = sound_file.Split(",".ToCharArray(), 2);
                        r.VolumeMultiplier = Convert.ToDouble(sound_file_split[0]);
                        sound_file = sound_file_split[1];

                    }
                    if (sound_file.Contains(":"))
                    {
                        string[] sound_file_split = sound_file.Split(":".ToCharArray(), 2);

                        foreach (string split in sound_file_split[1].Split(",".ToCharArray()))
                        {
                            string[] data = split.Split("=".ToCharArray(), 2);
                            if (data[0] == "V")
                                r.VolumeMultiplier = Convert.ToDouble(data[1]);

                        }

                        sound_file = sound_file_split[0];

                    }
                }
                r.file = Sound_Directory + sound_file;
            }

            if (r.VolumeMultiplier > MaxEngineRegionVolume)
                MaxEngineRegionVolume = r.VolumeMultiplier;
            r.SFX = this;
            r.player = new SoundPlayer(_mForm.Controls[0], SoundPlayer.PullAudio, r.file, 1);
            r.player.PullAFrequency = r.Pitch;
            r.player.PullVolume = r.Volume;
            r.player.Resume();
            Regions.Add(r);


        }

        public void Stop()
        {
            // Do what it says..
            foreach(EngineRpmRegion region in Regions)
            {
                region.player.Close();
                region.player.Dispose();
                region.player = null;
                region.VolumeMultiplier = 0;
            }
            Regions = new List<EngineRpmRegion>();


        }
    }
}