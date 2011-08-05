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

        public int MemoryBlock
        {
            get { return ((Base - 0x7154C0)/0x5F48); }
        }

        public int SectorsDriven
        {
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

        [Loggable(0.01)]
        public bool IsPlayer
        {
            set { }
            get { return ((rFactor.Game.ReadInt32(new IntPtr(0x0071528C)) == BaseAddress) ? true : false); }
        }

        [Loggable(0.05)]
        public string Name
        {
            set { }
            get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5B08), 64); }
        }

        [Unloggable()]
        public int BaseAddress
        {
            set { }
            get { return Base; }
        }

        [Loggable(5)]
        public double CoordinateX
        {
            //set {} get {return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x289C)); }
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x10)); }

        }
        [Loggable(5)]
        public double CoordinateY
        {
            //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A0)); }
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x14)); }

        }
        [Loggable(5)]
        public double CoordinateZ
        {
            //get { return rFactor.Game.ReadDouble(new IntPtr(this.BaseAddress + 0x28A4)); }
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x18)); }
        }

        [Loggable(25)]
        public double Throttle
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0xA8)); }
        }
        [Loggable(25)]
        public double Brake
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x2940)); }
        }

        [Loggable(1)]
        public double Fuel
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x315C)); }
        }

        [Loggable(1)]
        public double Fuel_Max
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(this.BaseAddress + 0x3160)); }
        }

        [Loggable(0.05)]
        public string CarModel
        {
            set { }
            get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5C82), 128); }
        }

        [Loggable(0.05)]
        public string CarClass
        {
            set { }
            get { return rFactor.Game.ReadString(new IntPtr(this.BaseAddress + 0x5C62), 0x20); }
        }

        [Loggable(0.05)]
        public bool Control_AI_Aid
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x1FB4)) == 1; }
        }

        [Loggable(1)]
        public bool PitLimiter
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x17B1)) == 1; }
        }
        [Loggable(1)]
        public bool Pits
        {
            set { }
            get { return this.Speed < 120 / 3.6 && rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x27A8)) == 1; }
        }
        [Loggable(1)]
        public bool HeadLights
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x17AD)) == 1; }
        }

        [Loggable(25)]
        public int Laps
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(this.BaseAddress + 0x3CF8)); }
        }

        [Loggable(0.05)]
        public float LapTime_Best
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D6C)); }
        }

        [Loggable(1)]
        public float LapTime_Last
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D0C)); }
        }

        [Loggable(1)]
        public float LapTime_Best_Sector1
        {
            get
            {
                List<ILap> laps = GetLapTimes();

                float best = 1000f;
                foreach (ILap l in laps)
                {
                    if (best > l.Sector1 && l.Sector1 > 0)
                        best = l.Sector1;

                }
                return best;
            }
        }

        [Loggable(1)]
        public float LapTime_Best_Sector2
        {
            get
            {
                List<ILap> laps = GetLapTimes();

                float best = 1000f;
                foreach (ILap l in laps)
                {
                    if (best > l.Sector2 && l.Sector2 > 0)
                        best = l.Sector2;

                }
                return best;
            }
        }

        [Loggable(1)]
        public float LapTime_Best_Sector3
        {
            get
            {
                List<ILap> laps = GetLapTimes();

                float best = 1000f;
                foreach (ILap l in laps)
                {
                    if (best > l.Sector3 && l.Sector3 > 0)
                        best = l.Sector3;

                }
                return best;
            }
        }
        [Loggable(1)]
        public float Sector_1_Best
        {
            get
            {
                return GetBestLap().Sector1;
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D70));
            }
        }

        [Loggable(1)]
        public float Sector_2_Best
        {
            get
            {
                return GetBestLap().Sector2;
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D74)) - Sector_1_Best;
            }
        }

        [Loggable(1)]
        public float Sector_3_Best
        {
            get
            {
                return GetBestLap().Sector3;
                return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3D78)) - Sector_2_Best - Sector_1_Best;
            }
        }

        [Loggable(1)]
        public float Sector_1_Last
        {
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

        [Loggable(1)]
        public float Sector_2_Last
        {
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

        [Loggable(1)]
        public float Sector_3_Last
        {
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
            if (laps.Count == 0) return null;
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
            get { return rFactor.Game.ReadFloat(new IntPtr(Base + 0x3D04)); }
        }

        public int PitStopRuns
        {
            get { return rFactor.Game.ReadByte(new IntPtr(Base + 0x3D2C)); }
        }
        public bool Retired
        { // 0x604E
            get { return ((rFactor.Game.ReadByte(new IntPtr(Base + 0x629C)) == 1) ? true : false); }
        }

        public TrackPosition TrackPosition
        {
            get
            {
                if (Pits) return TrackPosition.PITS;
                // get last lap
                List<ILap> laps = GetLapTimes();
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

        public double GetSplitTime(DriverGeneral player)
        {
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

        private double GetSectorRaceTime(int intptr, int sector)
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

            if (DateTime.Now.Subtract(LastLapTimeRetrival).TotalMilliseconds < 3000)
                return LastLapTimes;
            int lapsbase = __LapsData.ToInt32();

            for (int l = 0; l < this.Laps + 1; l++)
            {
                float start = rFactor.Game.ReadFloat(new IntPtr(lapsbase + l * 0x04 * 0x06));
                float s1 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x04 + l * 0x04 * 0x06));
                float s2 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x08 + l * 0x04 * 0x06));
                float s3 = rFactor.Game.ReadFloat(new IntPtr(lapsbase + 0x0C + l * 0x04 * 0x06));
                s3 -= s2;
                s2 -= s1;
                rFactorLap lap = new rFactorLap(l, s1, s2, s3);
                laps.Add(lap);
            }
            LastLapTimes = laps;
            LastLapTimeRetrival = DateTime.Now;
            return laps;
        }

        [Loggable(0.05)]
        public LevelIndicator SteeringHelp
        {
            set { }
            get { return (LevelIndicator)rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x1CFC)); }
        }

        [Loggable(0.05)]
        public int PitStop_FrontWingSetting
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x2324)); }
        }
        [Loggable(0.05)]
        public int PitStop_RearWingSetting
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x2364)); }
        }
        [Loggable(0.05)]
        public int PitStop_FuelSetting
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x2024)); }
        }

        [Loggable(0.05)]
        public double FuelSetting_Offset
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(BaseAddress + 0x2010)); }
        }

        [Loggable(0.05)]
        public double FuelSetting_Scale
        {
            set { }
            get { return rFactor.Game.ReadDouble(new IntPtr(BaseAddress + 0x2018)); }
        }

        [Loggable(0.05)]
        public double MassEmpty
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x28D8)); }
        }

        [Loggable(0.05)]
        public double Mass
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x28DC)); }
        }

        [Loggable(0.05)]
        public double RPM_Stationary
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x09E4)); }
        }
        [Loggable(0.05)]
        public double RPM_Max_Offset
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x3180)); }
        }
        [Loggable(0.05)]
        public double RPM_Max_Scale
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x31C0)); }
        }

        [Loggable(10)]
        public double Speed
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x57C0)); }
        }

        [Loggable(10)]
        public double RPM
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0xA4)); }
        }

        [Loggable(0.2)]
        public int Position
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3D20)); }
        }
        [Loggable(10)]
        public int Gear
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x321C)); }
        }

        [Loggable(0.05)]
        public int Gears
        {
            set { }
            get { return rFactor.Game.ReadByte(new IntPtr(BaseAddress + 0x3224)); }
        }

        [Loggable(0.05)]
        public float GearRatio1
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 1 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatio2
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 2 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatio3
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 3 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatio4
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 4 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatio5
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 5 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatio6
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 6 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatio7
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 7 + 0x31F8)); }
        }

        [Loggable(0.05)]
        public float GearRatioR
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x4 * 0 + 0x31F8)); }
        }

        [Loggable(0.5)]
        public float TyreWear_LF
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2A34)); }
        }

        [Loggable(0.5)]
        public float TyreWear_RF
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2C1C)); }
        }

        [Loggable(0.5)]
        public float TyreWear_LR
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2E04)); }
        }

        [Loggable(0.5)]
        public float TyreWear_RR
        {
            set { }
            get { return rFactor.Game.ReadFloat(new IntPtr(BaseAddress + 0x2FEC)); }
        }

        private int __LapsDataCached;
        [Unloggable()]
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

    public class rFactorLap : ILap
    {
        private int _lap;

        private float _sector1;

        private float _sector2;

        private float _sector3;

        public rFactorLap(int lap, float s1, float s2, float s3)
        {
            _lap = lap;
            _sector1 = s1;
            _sector2 = s2;
            _sector3 = s3;
        }

        public int Lap
        {
            get { return _lap; }
        }

        public float Sector1
        {
            get { return _sector1; }
        }

        public float Sector2
        {
            get { return _sector2; }
        }

        public float Sector3
        {
            get { return _sector3; }
        }

        public float LapTime
        {
            get { return _sector1 + _sector2 + _sector3; }
        }
    }
}