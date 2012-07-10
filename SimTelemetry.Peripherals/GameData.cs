using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using SimTelemetry.Data;
using SimTelemetry.Objects.Peripherals;

namespace SimTelemetry.Peripherals.Dashboard
{
    public class GameData
    {
        private const double rpm_to_rads = 2*Math.PI/60;
        private const double rads_to_rpm = 1 / rpm_to_rads;

        private Thread TickHandler;

        private Display_Left DP_L;
        private Display_Right DP_R;

        //private SerialPort sp;

        public GameData()
        {
            TickHandler = new Thread(Tickjes);
            TickHandler.IsBackground = true;
            TickHandler.Start();

            /*sp = new SerialPort("COM18", 115200);
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            sp.Open();*/
        }



        #region Gears
        private double rpm = 0;
        private ShiftRpm r = new ShiftRpm();

        public static double GetGearRatio_Pure(int g)
        {
            switch(g)
            {
                case 0:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio1 * GetGearRatio_Pure(1)/GetGearRatio_Pure(2);
                    break;
                case 1:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio1;
                    break;

                case 2:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio2;
                    break;

                case 3:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio3;
                    break;

                case 4:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio4;
                    break;

                case 5:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio5;
                    break;

                case 6:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio6;
                    break;

            }
            // TODO: globalize
            double ratio = Telemetry.m.Sim.Memory.ReadFloat((IntPtr)(0x7154C0 + 0x31F8 + 0x4 * g));
            ratio = Telemetry.m.Sim.Memory.ReadDouble((IntPtr)(0x00ADC248 + 0x8 * (g - 1)));
            return ratio;

        }

        public static int GetGear()
        {
            return Telemetry.m.Sim.Player.Gear;
            if (Telemetry.m.Sim.Drivers.Player.Gear == 0) return 0;
            double CurrentRatio = GetGearRatio_Pure(Telemetry.m.Sim.Drivers.Player.Gear);

            for (int i = 0; i <= Telemetry.m.Sim.Drivers.Player.Gears + 3; i++)
            {
                double d = CurrentRatio / ShiftRpm.GetRatio(i);
                if (d < 1.03 && d > 0.97)
                    return i;
                if (d < 0.97 && i == Telemetry.m.Sim.Drivers.Player.Gears + 3)
                    return Telemetry.m.Sim.Drivers.Player.Gears;
            }
            return 0;
        }
        #endregion
        #region Package

        //private Semaphore SerialPort_Lock = new Semaphore(1, 1);
        private void SendPackage(DashboardPackages package, byte[] data)
        {
            DevicePacket packet = new DevicePacket{Data = data, ID=Convert.ToInt16(package), Length=data.Length};

            Telemetry.m.Peripherals.TX(packet, "");
            /*SerialPort_Lock.WaitOne();
                DashboardPacket packet = new DashboardPacket();
                packet.Sync1 = '$';
                packet.Sync2 = '&';
                packet.length = (UInt16) (data.Length);
                packet.id = (byte) package;

                byte[] bPacket = ByteMethods.ToBytes(packet);
                sp.Write(bPacket, 0, 5);
                sp.Write(data, 0, data.Length);
                byte[] bf = new byte[2];
                bf[0] = 0;
                bf[1] = 0;
                sp.Write(bf, 0, 2);
            SerialPort_Lock.Release();*/
        }
        #endregion

        private void Tickjes()
        {
            int i = 0;
            int timestep = 40;
            while (true)
            {
                Thread.Sleep(timestep);
                i += timestep;

                if (Telemetry.m.Active_Session)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Fire_HS), null);

                    if (i > 2500)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(Fire_LS), null);
                        i = 0;
                    }
                }
            }
        }

        private void Fire_LS(object o)
        {
            GameData_LS package = new GameData_LS();

            /** TYRE TEMPERATURES **/
            if (Telemetry.m.Sim.Player.Tyre_Temperature_LF_Middle > 0)
            {
                package.TyreTemperature_LF = Convert.ToByte(Telemetry.m.Sim.Player.Tyre_Temperature_LF_Middle - 273.16);
                package.TyreTemperature_RF = Convert.ToByte(Telemetry.m.Sim.Player.Tyre_Temperature_RF_Middle - 273.16);
                package.TyreTemperature_LR = Convert.ToByte(Telemetry.m.Sim.Player.Tyre_Temperature_LR_Middle - 273.16);
                package.TyreTemperature_RR = Convert.ToByte(Telemetry.m.Sim.Player.Tyre_Temperature_RR_Middle - 273.16);
            }
            /** TYRE & BRAKE WEAR **/
            package.TyreWear_F = Convert.ToByte(200 * (Telemetry.m.Sim.Drivers.Player.TyreWear_LF + Telemetry.m.Sim.Drivers.Player.TyreWear_RF) / 2.0);
            package.TyreWear_R = Convert.ToByte(200 * (Telemetry.m.Sim.Drivers.Player.TyreWear_LR + Telemetry.m.Sim.Drivers.Player.TyreWear_RR) / 2.0);
            package.BrakeWear_F = 0;
            package.BrakeWear_R = 0;

            /** CURRENT FUEL INFO **/
            package.Fuel_Litre = Convert.ToUInt16(Telemetry.m.Sim.Player.Fuel * 10);
            package.Fuel_Laps = 0;

            /** DRIVING TIMES **/
            package.Laptime_Last = Telemetry.m.Sim.Drivers.Player.LapTime_Last;
            package.Laptime_Best = Telemetry.m.Sim.Drivers.Player.LapTime_Best;
            switch (Telemetry.m.Sim.Drivers.Player.SectorsDriven % 3)
            {
                case 0:
                    package.Split_Sector = Telemetry.m.Sim.Drivers.Player.Sector_3_Last - Telemetry.m.Sim.Drivers.Player.Sector_3_Best;
                    break;
                case 1:
                    package.Split_Sector = Telemetry.m.Sim.Drivers.Player.Sector_1_Last - Telemetry.m.Sim.Drivers.Player.Sector_1_Best;
                    break;
                case 2:
                    package.Split_Sector = Telemetry.m.Sim.Drivers.Player.Sector_2_Last - Telemetry.m.Sim.Drivers.Player.Sector_2_Best;
                    break;

            }
            package.Split_Lap = Telemetry.m.Sim.Drivers.Player.LapTime_Best - Telemetry.m.Sim.Drivers.Player.LapTime_Last;

            package.RaceTime = Telemetry.m.Sim.Session.Time - 30;
            package.RaceLength = 3600;

            SendPackage(DashboardPackages.PACK_GAMEDATA_LS, ByteMethods.ToBytes(package));

            /*** Temporarely do cardata here as well ***/
            CarData cardata = new CarData();
            cardata.RPM_Max = Convert.ToUInt16(Telemetry.m.Sim.Player.Engine_RPM_Max_Live * rads_to_rpm);
            cardata.RPM_Idle = Convert.ToUInt16(Telemetry.m.Sim.Player.Engine_RPM_Idle_Max * rads_to_rpm);

            //cardata.HP_Max = Convert.ToUInt16(EngineCurve.GetMaxHP());
            cardata.Gears = (byte)Telemetry.m.Sim.Drivers.Player.Gears;
            cardata.Fuel_Max = (byte)Telemetry.m.Sim.Drivers.Player.Fuel_Max;

            cardata.GearRatio0 = Convert.ToSingle(GetGearRatio_Pure(0));
            cardata.GearRatio1 = Convert.ToSingle(GetGearRatio_Pure(1));
            cardata.GearRatio2 = Convert.ToSingle(GetGearRatio_Pure(2));
            cardata.GearRatio3 = Convert.ToSingle(GetGearRatio_Pure(3));
            cardata.GearRatio4 = Convert.ToSingle(GetGearRatio_Pure(4));
            cardata.GearRatio5 = Convert.ToSingle(GetGearRatio_Pure(5));
            cardata.GearRatio6 = Convert.ToSingle(GetGearRatio_Pure(6));
            cardata.GearRatio7 = Convert.ToSingle(GetGearRatio_Pure(7));

            ShiftRpm r = new ShiftRpm();
            cardata.RPM_Shift0 = cardata.RPM_Max;
            /*cardata.RPM_Shift1 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(1)));
            cardata.RPM_Shift2 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(2)));
            cardata.RPM_Shift3 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(3)));
            cardata.RPM_Shift4 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(4)));
            cardata.RPM_Shift5 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(5)));
            cardata.RPM_Shift6 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(6)));*/
            cardata.RPM_Shift1 = cardata.RPM_Max;
            cardata.RPM_Shift2 = cardata.RPM_Max;
            cardata.RPM_Shift3 = cardata.RPM_Max;
            cardata.RPM_Shift4 = cardata.RPM_Max;
            cardata.RPM_Shift5= cardata.RPM_Max;
            cardata.RPM_Shift6 = cardata.RPM_Max;
            cardata.RPM_Shift7 = cardata.RPM_Max;

            SendPackage(DashboardPackages.PACK_CARDATA, ByteMethods.ToBytes(cardata));

            // Send leds config..
            //return;
            string[] leds = System.IO.File.ReadAllLines("LEDs.csv");
            byte[] bf = new byte[leds.Length * 9 + 1];
            bf[0] = 0;
            int i = 1;
            int k = 0;
            foreach (string led in leds)
            {
                string[] data = led.Split(",".ToCharArray());

                bf[i] = (byte)(Convert.ToByte(data[4]));
                bf[i + 1] = (byte)(Convert.ToByte(data[1]));
                bf[i + 2] = (byte)(Convert.ToByte(data[2]));
                bf[i + 3] = (byte)(Convert.ToByte(data[3]));
                byte[] bf_tmp = BitConverter.GetBytes(0);
                bf[i + 4] = bf_tmp[0];
                bf[i + 5] = bf_tmp[1];
                bf[i + 6] = 0;
                bf[i + 7] = 0;
                bf[i + 8] = 12;

                i += 9;
                k++;
            }
            SendPackage(DashboardPackages.PACK_PREFS_SHIFTBAR, bf);
            bf = new byte[leds.Length * 9 + 1];
            bf[0] = 1;
            i = 1;
            k = 0;
            foreach (string led in leds)
            {
                string[] data = led.Split(",".ToCharArray());

                bf[i] = 0;
                bf[i + 1] = 5;
                bf[i + 2] = 0;
                bf[i + 3] = 5;
                byte[] bf_tmp = BitConverter.GetBytes(200);
                bf[i + 4] = bf_tmp[0];
                bf[i + 5] = bf_tmp[1];
                bf[i + 6] = 50;
                bf[i + 7] = Convert.ToByte(((k % 2 == 0) ? 50 : 0));
                bf[i + 8] = 0;

                i += 9;
                k++;
            }
            SendPackage(DashboardPackages.PACK_PREFS_SHIFTBAR, bf);
            
            return;
           
        }

        private float PreviousTime = 0;

        private void Fire_HS(object o)
        {
                GameData_HS package = new GameData_HS();

                /** DRIVER INPUTS **/
                package.Throttle = Convert.ToByte(200*Telemetry.m.Sim.Player.Pedals_Throttle);
                package.Brake = Convert.ToByte(200*Telemetry.m.Sim.Player.Pedals_Brake);
                package.Clutch = Convert.ToByte(200*Telemetry.m.Sim.Player.Pedals_Clutch);
                package.Steer = Convert.ToByte(100*(1 + Telemetry.m.Sim.Player.SteeringAngle));

                /** PITS SITUATION ETC **/
                package.PitLimiter = (byte) ((Telemetry.m.Sim.Drivers.Player.PitLimiter) ? 1 : 0);
                package.InPits = (byte) ((Telemetry.m.Sim.Drivers.Player.Pits) ? 1 : 0);
                package.PitRequired = 0; // TODO;
                package.EngineStall = (byte) ((Telemetry.m.Sim.Drivers.Player.RPM < 500) ? 1 : 0);


                /** DRIVING INFO **/
                package.Gear = (byte) ((Telemetry.m.Sim.Player.Gear == 255) ? 11 : GetGear());
                package.Position = (byte) Telemetry.m.Sim.Drivers.Player.Position;
                package.Wheelslip = 0; // TODO
                package.Cars = (byte) Telemetry.m.Sim.Session.Cars;

                /** TRACK INFO **/
                package.Flag = 0; // TODO 
                package.Temp_Water = Convert.ToByte(Telemetry.m.Sim.Player.Engine_Temperature_Water);
                package.Temp_Oil = Convert.ToByte(Telemetry.m.Sim.Player.Engine_Temperature_Oil);
                package.Temp_Track = Convert.ToByte(Telemetry.m.Sim.Session.TrackTemperature);


                /** MORE DRIVING **/
                package.Speed = Convert.ToUInt16(Math.Round(Math.Abs(Telemetry.m.Sim.Player.Speed*3.6)));
                package.RPM = Convert.ToUInt16(Math.Round(Math.Max(0, rads_to_rpm*Telemetry.m.Sim.Player.Engine_RPM)));
                package.Engine_HP =
                    Convert.ToUInt16(
                        Math.Round(Math.Max(0,
                                            Telemetry.m.Sim.Player.Engine_Torque*Telemetry.m.Sim.Player.Engine_RPM*
                                            rads_to_rpm/5252)));
                package.MetersDriven = Convert.ToUInt16(Math.Max(0, Telemetry.m.Sim.Drivers.Player.MetersDriven));

                /** LIVE DRIVING TIMES **/
                package.Laptime_Current = 0; // TODO
                package.Gap_Front = 0; // TODO
                package.Gap_Back = 0; // TODO

                package.Wipers = 0;
                package.Lights = (byte)((Telemetry.m.Sim.Drivers.Player.HeadLights) ? 1 : 0); ;
                //package.Ignition = (byte)((Telemetry.m.Sim.Player.i) ? 1 : 0); ;
                package.Ignition = (byte)((Telemetry.m.Sim.Drivers.Player.Ignition) ? 1 : 0); ;
                package.Ignition = 1;
                package.Pause = (byte)((Telemetry.m.Sim.Session.Time == PreviousTime) ? 1 : 0);
            PreviousTime = Telemetry.m.Sim.Session.Time;

                // Send package
                SendPackage(DashboardPackages.PACK_GAMEDATA_HS, ByteMethods.ToBytes(package));
        }

    }
}