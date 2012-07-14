using System;
using SimTelemetry.Data.Logger;

namespace SimTelemetry
{
    public class EngineRpmRegion
    {
        public TelemetryLogReplay _mMaster { get; set; }
        public double Throttle_LoadBlend_Low
        {
            get { return SFX.Throttle_LoadBlend_Low; }
        }
        public double Throttle_LoadBlend_High
        {
            get { return SFX.Throttle_LoadBlend_High; }
        }

        public double Min;
        public double Max;
        public double Nat;
        public int offset;
        public string file;
        public EngineRpmRegionType type;
        public SoundPlayer player;
        public ReplaySFX SFX;
        public double VolumeMultiplier = 1;
        public double FrequencyMultiplier = 1;
        public double Volume()
        {
            if (this._mMaster == null)
            {
                return 0;
            }
            FrequencyMultiplier = 1*9.54929659643;
            double factor = 0.02 * Math.Pow(VolumeMultiplier, 3) * 2;
            double RPM = _mMaster.GetDouble("Driver.RPM") * FrequencyMultiplier;
            double Throttle = _mMaster.GetDouble("Player.Pedals_Throttle");
            if (RPM > Max || RPM < Min)
                return 0;


            // lower rpm bound
            EngineRpmRegion prev = SFX.GetPrevious(this);
            if (prev != null)
            {
                double EngineRPMRegion_Low = prev.Max - Min;
                double vol = (RPM - Min) / EngineRPMRegion_Low;
                if (vol > 1) vol = 1;
                factor *= Math.Pow(vol, 2);
            }

            // upper rpm bound
            EngineRpmRegion next = SFX.GetNext(this);
            if (next != null)
            {
                double EngineRPMRegion_High = Max - next.Min;
                double vol = (Max - RPM) / EngineRPMRegion_High;
                if (vol > 1) vol = 1;
                factor *= Math.Pow(vol, 2);
            }

            //throttle
            double BlendRegion = Throttle_LoadBlend_High - Throttle_LoadBlend_Low;
            if (type == EngineRpmRegionType.COAST)
            {
                if (Throttle < Throttle_LoadBlend_Low)
                    factor *= 1;
                else
                {
                    if (Throttle > Throttle_LoadBlend_High)
                        factor *= 0;
                    else
                    {
                        factor *= Math.Min(1, Math.Pow(1 - (Throttle - Throttle_LoadBlend_Low) / Throttle_LoadBlend_High, 2));
                    }
                }

            }
            else
            {
                if (Throttle < Throttle_LoadBlend_Low)
                    factor *= 0;
                else
                {
                    if (Throttle > Throttle_LoadBlend_High)
                        factor *= 1;
                    else
                    {
                        factor *= Math.Min(1, Math.Pow((Throttle - Throttle_LoadBlend_Low) / Throttle_LoadBlend_High, 2));
                    }
                }

            }

            return factor;

        }

        private int lastGear = 0;
        public double Pitch()
        {
                if (this._mMaster == null)
                {
                    return 1;
                }
            try
            {
                    if (lastGear != this._mMaster.GetInt32("Driver.Gear"))
                    {
                        lastGear = this._mMaster.GetInt32("Driver.Gear");
                        return -1;
                    }
                
            }catch(Exception ex)
            {
            }
            return _mMaster.GetDouble("Driver.RPM") * FrequencyMultiplier / Nat;


        }
    }
}