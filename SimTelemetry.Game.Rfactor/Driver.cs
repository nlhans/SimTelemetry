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
using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor
{
    public class DriverGeneral : IDriverGeneral
    {
        private int Base = 0;


        public DriverGeneral(int i)
        {
            this.Base = i;
        }

        /*
        public bool Retired
        {
            get { return ((rFactor.Game.ReadByte(new IntPtr(Base + 0x27A8)) == 1) ? true : false); }
            set { }
        }*/
        
        public bool Driving
        { // 0x1794 seems to be set to 1 at start of each session
            // 0x314 seems to buggy with AI around.
            get { return ((rFactor.Game.ReadByte(new IntPtr(Base + 0x3CBF)) == 1) ? true : false); }
            set { }
        }

        
        public bool Ignition
        {
            get { return ((rFactor.Game.ReadByte(new IntPtr(Base + 0xAA)) > 0) ? true : false); }
            set { }
        }

        
        public int MemoryBlock
        {
            set { }
            get { return ((Base - 0x7154C0)/0x5F48); }
        }

        
        public int SectorsDriven
        {
            set { }
            get { return rFactor.Game.ReadInt32(new IntPtr(0x0070F988 + 0x04 + 0x04*3*MemoryBlock)); }
        }

        
        public bool Active
        {
            set { }
            get
            {
                double cx = CoordinateX;
                double cy = CoordinateY;
                double cz = CoordinateZ;

                return Name != "" && cx != 0 && cy != 0 && cz != 0;
            }
        }

        
        public bool IsPlayer
        {
            set { }
            get { return ((rFactor.Game.ReadInt32(new IntPtr(0x0071528C)) == BaseAddress) ? true : false); }
        }


        public string Name
        {
            set { }
            get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5B08), 64); }
        }

        
        public int BaseAddress
        {
            set { }
            get { return Base; }
        }

        
        public double CoordinateX
        {
            //set {} get {return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x289C)); }
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x10)); }

        }
        
        public double CoordinateZ
        {
            //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A0)); }
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x14)); }

        }
        
        public double CoordinateY
        {
            //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A4)); }
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x18)); }
        }

        
        public double Throttle
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0xA8)); }
        }
        
        public double Brake
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x2940)); }
        }

        
        public double Fuel
        {
            set { }
            get { double a= rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x315C));
                return a;
            }
        }

        
        public double Fuel_Max
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x3160)); }
        }


        public string CarModel
        {
            set { }
            get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5C82), 128); }
        }


        public string CarClass
        {
            set { }
            get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x39BC), 128); }
            //get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5C62), 0x20); }
        }


        public bool Control_AI_Aid
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x1FB4)) == 1; }
        }

        
        public bool PitLimiter
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x17B1)) == 1; }
        }
        
        public bool Pits
        {
            set { }
            get { return this.Speed < 120 / 3.6 && rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x27A8)) == 1; }
        }
        
        public bool HeadLights
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x17AD)) == 1; }
        }

        
        public int Laps
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x3CF8)); }
        }


        public float LapTime_Best
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D6C)); }
        }

        
        public float LapTime_Last
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D0C)); }
        }

        
        public float LapTime_Best_Sector1
        {
            set { }
            get
            {
                List<ILap> laps = GetLapTimes();

                float best = 1000f;
                foreach (ILap l in laps)
                {
                    if (l.Sector1 > 0.1)
                        best = Math.Min(best,l.Sector1);

                }
                return best;
            }
        }

        
        public float LapTime_Best_Sector2
        {
            set { }
            get
            {
                List<ILap> laps = GetLapTimes();

                float best = 1000f;
                foreach (ILap l in laps)
                {
                    if (l.Sector2 > 0.1)
                        best = Math.Min(best, l.Sector2);

                }
                return best;
            }
        }

        
        public float LapTime_Best_Sector3
        {
            set { }
            get
            {
                List<ILap> laps = GetLapTimes();

                float best = 1000f;
                foreach (ILap l in laps)
                {
                    if (l.Sector3 > 0.1)
                        best = Math.Min(best, l.Sector3);

                }
                return best;
            }
        }
        
        public float Sector_1_Best
        {
            set { }
            get
            {
                return GetBestLap().Sector1;
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D70));
            }
        }

        
        public float Sector_2_Best
        {
            set { }
            get
            {
                return GetBestLap().Sector2;
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D74)) - Sector_1_Best;
            }
        }

        
        public float Sector_3_Best
        {
            set { }
            get
            {
                return GetBestLap().Sector3;
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D78)) - Sector_2_Best - Sector_1_Best;
            }
        }

        
        public float Sector_1_Last
        {
            set { }
            get
            {
                List<ILap> laps = GetLapTimes();
                if (laps.Count <= 1) return -1;
                // Get the last lap
                if (laps[laps.Count - 1].Sector1 <= 0)
                    return laps[laps.Count - 2].Sector1;
                else
                    return laps[laps.Count - 1].Sector1;
                //return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D10));
            }
        }

        
        public float Sector_2_Last
        {
            set { }
            get
            {
                List<ILap> laps = GetLapTimes();
                if (laps.Count <= 1) return -1;
                // Get the last lap
                if (laps[laps.Count - 1].Sector2 <= 0)
                    return laps[laps.Count - 2].Sector2;
                else
                    return laps[laps.Count - 1].Sector2;
                //return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D14)) - Sector_1_Last;
            }
        }

        
        public float Sector_3_Last
        {
            set { }
            get
            {
                List<ILap> laps = GetLapTimes();
                if (laps.Count <= 1) return -1;
                // Get the last lap
                if (laps[laps.Count - 1].Sector3 <= 0)
                    return laps[laps.Count - 2].Sector3;
                else
                    return laps[laps.Count - 1].Sector3;
                //return LapTime_Last - Sector_1_Last - Sector_2_Last;
            }
        }

        public ILap GetBestLap()
        {
            List<ILap> laps = GetLapTimes();
            if (laps.Count == 0) return new Lap(0, 0, 0, 0);
            ILap best = laps[0];
            foreach (ILap l in laps)
            {
                if (best.LapTime <= 0) best = l;
                if (l.LapTime < best.LapTime && l.LapTime > 0)
                    best = l;
            }

            return best;
        }

        
        public double MetersDriven
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(Base + 0x3D04)); }
        }

        
        public int PitStopRuns
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(Base + 0x3D2C)); }
        }
        
        public bool Retired
        { // 0x604E
            set { }
            get { return ((rFactor.Game.ReadByte(new IntPtr(Base + 0x4160)) == 1) ? true : false); } // 0x629C. 27A8
        }

        
        public TrackPosition TrackPosition
        {
            set { }
            get
            {
                if (Pits) return TrackPosition.PITS;
                // get last lap
                List<ILap> laps = GetLapTimes();
                if (laps.Count == 0) return TrackPosition.PITS;
                ILap LastLap = laps[laps.Count - 1];

                if (LastLap.Sector1 <= 0) return TrackPosition.SECTOR1;
                if (LastLap.Sector2 <= 0) return TrackPosition.SECTOR2;
                if (LastLap.Sector3 <= 0) return TrackPosition.SECTOR3;
                return TrackPosition.HELL;
            }
        }

        public double GetRaceTime()
        {
            int lapsbase = __LapsData.ToInt32();
            int l = this.Laps;
            float start = rFactor.Game.ReadFloat(new IntPtr(lapsbase + l * 0x04 * 0x06));
            l++;
            float s1 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x04 + l * 0x04 * 0x06));
            float s2 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x08 + l * 0x04 * 0x06));
            float s3 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x0C + l * 0x04 * 0x06));

            float val = start;
            if (s1 > 0) val += s1;
            if (s2 > 0) val += s2;
            if (s3 > 0) val += s3;

            return val;
        }

        public double GetSplitTime(IDriverGeneral _player)
        {
            DriverGeneral player = (DriverGeneral) _player;
            //return 1;
            int lapsbase_leader =player.__LapsData.ToInt32();
            if (lapsbase_leader == 0) 
                return 1000000f;
            int lapsbase = __LapsData.ToInt32();

            int sectors_leader = player.SectorsDriven ;
            int sectors_follower = SectorsDriven;

            bool ahead = false;
            int plus = 0;

            if (player.Laps - this.Laps == 1)
            {
                if (player.MetersDriven > this.MetersDriven && sectors_leader - 2 >= sectors_follower)
                {
                    return 10000;
                }
            }
            if (player.Laps - this.Laps > 1)
                ahead = true;

            if (ahead)
            {
                if (player.MetersDriven < this.MetersDriven)
                    sectors_leader--;
                return 10000 * (plus + Math.Floor((sectors_leader - sectors_follower) / 3.0));
            }

            // Get the most accurate sectors
            // get both race time values for this sector
            double sector_leader = GetSectorRaceTime(lapsbase_leader, sectors_follower);
            double sector_follower = GetSectorRaceTime(lapsbase, sectors_follower);
            return sector_follower - sector_leader;

        }

        public double GetSectorRaceTime(int intptr, int sector)
        {
            int lap = sector / 3;
            sector %= 3;
            //if (sector == 1) lap--;
            float start = rFactor.Game.ReadFloat(new IntPtr(intptr + lap * 0x04 * 0x06));
            if (lap > 1) lap--;
            float s1 = rFactor.Game.ReadFloat(new IntPtr(intptr + 0x04 + lap * 0x04 * 0x06));
            float s2 = rFactor.Game.ReadFloat(new IntPtr(intptr + 0x08 + lap * 0x04 * 0x06));
            float s3 = rFactor.Game.ReadFloat(new IntPtr(intptr + 0x0C + lap * 0x04 * 0x06));

            if (sector == 1) return (start + s1);
            if (sector == 2) return (start + s2);
            if (sector == 3) return (start + s3);
            return start;
        }

        private DateTime LastLapTimeRetrival = DateTime.Now;
        private List<ILap> LastLapTimes = new List<ILap>();
        public List<ILap> GetLapTimes()
        {
            
            List<ILap> laps = new List<ILap>();

            int lapsbase = __LapsData.ToInt32();

            for (int l = 0; l < this.Laps + 1; l++)
            {
                float start = rFactor.Game.ReadFloat(new IntPtr(lapsbase + l * 0x04 * 0x06));
                float s1 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x04 + l * 0x04 * 0x06));
                float s2 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x08 + l * 0x04 * 0x06));
                float s3 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x0C + l * 0x04 * 0x06));
                s3 -= s2;
                s2 -= s1;
                Lap lap = new Lap(l, s1, s2, s3);
                laps.Add(lap);
            }

            return laps;
        }

        public ILap GetLapTime(int l)
        {
            int lapsbase = __LapsData.ToInt32();

            float start = rFactor.Game.ReadFloat(new IntPtr(lapsbase + l*0x04*0x06));
            float s1 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x04 + l*0x04*0x06));
            float s2 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x08 + l*0x04*0x06));
            float s3 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x0C + l*0x04*0x06));
            s3 -= s2;
            s2 -= s1;
            Lap lap = new Lap(l, s1, s2, s3);

            return lap;
        }

        public LevelIndicator SteeringHelp
        {
            set { }
            get { return (LevelIndicator)rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x1CFC)); }
        }


        public int PitStop_FrontWingSetting
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x2324)); }
        }

        public int PitStop_RearWingSetting
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x2364)); }
        }

        public int PitStop_FuelSetting
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x2024)); }
        }


        public double FuelSetting_Offset
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(BaseAddress + 0x2010)); }
        }


        public double FuelSetting_Scale
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(BaseAddress + 0x2018)); }
        }


        public double MassEmpty
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x28D8)); }
        }


        public double Mass
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x28DC)); }
        }


        public double RPM_Stationary
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x09E4)); }
        }

        public double RPM_Max_Offset
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3180)); }
        }

        public double RPM_Max_Scale
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x31C0)); }
        }

        
        public double Speed
        {
            set { }
            get
            {
                // TODO: If the  player is driving, do max function of SpeedSlipping (aero effective speed and the pointer)
                // Remove the player name in this equation.
                if (this.Name == "Hans")
                    return Math.Max(rFactor.Player.SpeedSlipping,
                                    rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x57C0)));
                return   rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x57C0));
            }
        }

        
        public double RPM
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0xA4)); }
        }

        
        public int Position
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3D20)); }
        }
        
        public int Gear
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x321C)); }
        }


        public int Gears
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3224)); }
        }


        public float GearRatio1
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 1 + 0x31F8)); }
        }


        public float GearRatio2
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 2 + 0x31F8)); }
        }


        public float GearRatio3
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 3 + 0x31F8)); }
        }


        public float GearRatio4
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 4 + 0x31F8)); }
        }


        public float GearRatio5
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 5 + 0x31F8)); }
        }


        public float GearRatio6
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 6 + 0x31F8)); }
        }


        public float GearRatio7
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 7 + 0x31F8)); }
        }

        public float GearRatio8 { get { return 0; } set { } }
        public float GearRatio9 { get { return 0; } set { } }
        public float GearRatio10 { get { return 0; } set { } }
        public float GearRatio11 { get { return 0; } set { } }
        public float GearRatio12 { get { return 0; } set { } }
        public float GearRatio13 { get { return 0; } set { } }
        public float GearRatio14 { get { return 0; } set { } }
        public float GearRatio15 { get { return 0; } set { } }
        public float GearRatio16 { get { return 0; } set { } }
        public float GearRatio17 { get { return 0; } set { } }
        public float GearRatio18 { get { return 0; } set { } }


        public float GearRatioR
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 0 + 0x31F8)); }
        }

        
        public float TyreWear_LF
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2A34)); }
        }

        
        public float TyreWear_RF
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2C1C)); }
        }

        
        public float TyreWear_LR
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2E04)); }
        }

        
        public float TyreWear_RR
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2FEC)); }
        }

        
        public bool Flag_Yellow
        {
            // 0x7155C4<<<, 0x7157D4
            // ^ This goes high when car is <80km/h. 
            set { }
            get { return (((rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x104)) == 1) ? false : true) && !Pits && this.Speed*3.6<40); }
        }
        
        public bool Flag_Blue
        {
            set { }
            get { return ((rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3E39)) == 0) ? false : true); }
        }
        
        public bool Flag_Black
        {
            set { }
            get { return ((rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3D24)) == 0) ? false : true); }
        }

        private int __LapsDataCached;
        
        public IntPtr __LapsData
        {
            set { }
            get
            {

                int data = rFactor.Game.ReadInt32(new IntPtr(BaseAddress + 0x3D90));
                if (data > 0x7154C0)
                    __LapsDataCached = data;
                else
                {
                }
                return new IntPtr(__LapsDataCached);
            }
        }
    }
}