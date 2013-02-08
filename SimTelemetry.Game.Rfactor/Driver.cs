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
using SimTelemetry.Game.Rfactor.MMF;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;

namespace SimTelemetry.Game.Rfactor
{
    public class DriverGeneral : IDriverGeneral
    {
        private rFactorDriver _MMFData;

        private int Base = 0;


        public DriverGeneral(int i)
        {
            this.Base = i;
        }

        public DriverGeneral(rFactorDriver mmf_instance)
        {
            _MMFData = mmf_instance;
        }

        public double Heading
        {
            get { var angle = Math.PI - Math.Atan2( rFactor.Game.ReadFloat(Base + 0x40), rFactor.Game.ReadFloat(Base + 0x48)); 
            return Math.PI + angle;
            }
            set { }
        }

        public bool Driving
        { // 0x1794 seems to be set to 1 at start of each session
            // 0x314 seems to buggy with AI around.
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return ((rFactor.Game.ReadByte(new IntPtr(Base + 0x3CBF)) == 1) ? true : false);
                return true; }
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
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadInt32(new IntPtr(rFactor.Game.Base + 0x0030F988 + 0x04 + 0x04 * 3 * MemoryBlock));
                return 0;   }
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
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return ((rFactor.Game.ReadInt32(new IntPtr(rFactor.Game.Base + 0x0031528C)) == BaseAddress) ? true : false); 
                return _MMFData.IsPlayer; }
        }


        public string Name
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5B08), 64);
                return "Player " + _MMFData.Position; 
            }
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
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x10));
                else return _MMFData.X;
            }

        }
        
        public double CoordinateZ
        {
            //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A0)); }
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x14));
                else return _MMFData.Y;
            }

        }
        
        public double CoordinateY
        {
            //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A4)); }
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)
                    return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x18));
                else return _MMFData.Z;
            }
        }

        
        public double Throttle
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0xA8));
                return 0;
            }
        }
        
        public double Brake
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x2940));
                return 0;  }
        }

        
        public double Fuel
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)
                    return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x315C));
                return 0;
            }
        }

        
        public double Fuel_Max
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x3160)); 
                return 0; }
        }


        public string CarModel
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5C82), 128);
                else
                return _MMFData.VehicleName; }
        }


        public string CarClass
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x39BC), 128);
                else return _MMFData.VehicleName; 
            }
            //get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5C62), 0x20); }
        }


        public bool Control_AI_Aid
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x1FB4)) == 1;
                return _MMFData.AIControl; }
        }

        
        public bool PitLimiter
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x17B1)) == 1;
                return false; }
        }
        
        public bool Pits
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return this.Speed < 120 / 3.6 && rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x27A8)) == 1;
                return _MMFData.InPits;  }
        }
        
        public bool HeadLights
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x17AD)) == 1; }
        }

        
        public int Laps
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x3CF8)); 
                return _MMFData.LapsLeader; }
        }


        public float LapTime_Best
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D6C)); 
                return 0;}
        }

        
        public float LapTime_Last
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D0C));
                return 0;
            }
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
                if (rFactor.Simulator.UseMemoryReader)
                    return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D70));
                return GetBestLap().Sector1;
            }
        }

        
        public float Sector_2_Best
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)
                    return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D74)) - Sector_1_Best;
                return GetBestLap().Sector2;
            }
        }

        
        public float Sector_3_Best
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader)
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D78)) - Sector_2_Best - Sector_1_Best;
                return GetBestLap().Sector3;
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
            if (!rFactor.Simulator.Modules.Times_History_LapTimes)
                return new Lap(0, 0, 0,0 );
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

        
        public int Pitstops
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
            if (rFactor.Simulator.Modules.Times_History_LapTimes == false) return laps;

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
            if (rFactor.Simulator.Modules.Times_History_LapTimes == false) return new Lap(0, 0, 0, 0);
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
                if (rFactor.Simulator.UseMemoryReader)
                    return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x57C0));
                return _MMFData.Speed;
            }
        }

        
        public double RPM
        {
            set { }
            get
            {
                if (rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0xA4));
                return 0;
            }
        }

        public double RPM_Max
        {
            get
            {
                if (rFactor.Simulator.UseMemoryReader) 
                    if(IsPlayer)
                        return rFactor.Player.Engine_RPM_Max_Live;
                    else
                        return 0;
                return 0;
            }
            set { throw new NotImplementedException(); }
        }


        public int Position
        {
            set { }
            get
            {if(rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3D20));
                return _MMFData.Position; }
        }
        
        public int Gear
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x321C)); }
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

        public IWheel Wheel_LeftFront
        {
            get
            {
                return new DriverWheel(BaseAddress, 0); throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
        }

        public IWheel Wheel_RightFront
        {
            get
            {
                return new DriverWheel(BaseAddress, 1); throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
        }

        public IWheel Wheel_LeftRear
        {
            get
            {
                return new DriverWheel(BaseAddress, 2); throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
        }

        public IWheel Wheel_RightRear
        {
            get
            {
                return new DriverWheel(BaseAddress, 3); throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
        }


        public float TyreWear_LF
        {
            set { }
            get
            {
                //if (IsPlayer) return rFactor.MMF.Telemetry.Player.Wheel_LF.TyreWear; 
                if (rFactor.Simulator.UseMemoryReader)
                    return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2A34));
                return 0;
            }
        }

        
        public float TyreWear_RF
        {
            set { }
            get
            {
                //if (IsPlayer) return rFactor.MMF.Telemetry.Player.Wheel_RF.TyreWear;
                if(rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2C1C));return 0;
            }
        }

        
        public float TyreWear_LR
        {
            set { }
            get
            {
                //if (IsPlayer) return rFactor.MMF.Telemetry.Player.Wheel_LR.TyreWear;
                if(rFactor.Simulator.UseMemoryReader) return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2E04)); return 0;
            }
        }

        
        public float TyreWear_RR
        {
            set { }
            get
            {
                //if (IsPlayer) return rFactor.MMF.Telemetry.Player.Wheel_RR.TyreWear;
                if(rFactor.Simulator.UseMemoryReader)return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2FEC)); return 0;
            }
        }

        
        public bool Flag_Yellow
        {
            // 0x7155C4<<<, 0x7157D4
            // ^ This goes high when car is <80km/h. 
            set { }
            get
            {
                if (!rFactor.Simulator.UseMemoryReader) return false;
                return (((rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x104)) == 1) ? false : true) && !Pits && this.Speed*3.6<40);
            }
        }
        
        public bool Flag_Blue
        {
            set { }
            get
            {
                if (!rFactor.Simulator.UseMemoryReader) return false; 
                return ((rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3E39)) == 0) ? false : true);
            }
        }
        
        public bool Flag_Black
        {
            set { }
            get
            {
                if (!rFactor.Simulator.UseMemoryReader) return false; 
                return ((rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3D24)) == 0) ? false : true);
            }
        }

        private int __LapsDataCached;
        
        public IntPtr __LapsData
        {
            set { }
            get
            {
                if (!rFactor.Simulator.UseMemoryReader) return IntPtr.Zero;
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

    public class DriverWheel : IWheel
    {
        public double Wear { get; set; }
        public double Temperature_Inside { get; set; }
        public double Temperature_Middle { get; set; }
        public double Temperature_Outside { get; set; }
        public double Pressure { get; set; }
        public double Rideheight { get; set; }
        public double Radius { get; set; }
        public bool Flat { get; set; }
        public bool Detached { get; set; }

        private int Addr;
        private int Index;
        public DriverWheel(int addr, int ind)
        {
            Index = ind;
            Addr = addr;

            Wear = rFactor.Game.ReadFloat(new IntPtr(addr + 0x2A34 + ind * 0x1E8));

            if (addr == 0x7154C0) // = player
            {
                int ind2 = ind;
                switch(ind)
                {
                    case 0:
                        ind = 0;
                        break;
                    case 2:
                        ind = 2;
                        break;
                    case 1:
                        ind = 1;
                        break;
                    case 3:
                        ind = 3;
                        break;
                }
                Temperature_Inside = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + + 0x006D9F44 + ind * 0x600));
                 Temperature_Middle = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + + 0x006D9F3C + ind * 0x600));
                Temperature_Outside = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + + 0x006D9F34 + ind * 0x600));
                Pressure = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + 0x006D9F5C + ind * 0x600));
                Rideheight = rFactor.Game.ReadDouble(new IntPtr(rFactor.Game.Base + +0x006DB778 + ind2 * 0x10));
            }
        }
    }
}