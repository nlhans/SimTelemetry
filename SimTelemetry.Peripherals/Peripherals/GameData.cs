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
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using SimTelemetry.Data;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Peripherals;
using SimTelemetry.Peripherals.Peripherals;

namespace SimTelemetry.Peripherals.Dashboard
{
    public class GameData
    {
        private const double rpm_to_rads = 2 * Math.PI / 60;
        private const double rads_to_rpm = 1 / rpm_to_rads;

        private Thread TickHandler;

        private Display_Left DP_L;
        private Display_Right DP_R;

        private SerialPort sp;

        public GameData()
        {
            Switchboard b = new Switchboard();
            TickHandler = new Thread(Tickjes);
            TickHandler.IsBackground = true;
            TickHandler.Start();

            /*sp = new SerialPort("COM10", 115200);
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            sp.Open();*/
        }



        #region Gears
        private double rpm = 0;
        private ShiftRpm r = new ShiftRpm();

        public static double GetGearRatio_Pure(int g)
        {
            switch (g)
            {
                case 0:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio1 * GetGearRatio_Pure(1) / GetGearRatio_Pure(2);
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

                case 7:
                    return Telemetry.m.Sim.Drivers.Player.GearRatio7;
                    break;

            }
            // TODO: globalize
            //double ratio = Telemetry.m.Sim.Memory.ReadFloat((IntPtr)(0x7154C0 + 0x31F8 + 0x4 * g));
            //ratio = Telemetry.m.Sim.Memory.ReadDouble((IntPtr)(0x00ADC248 + 0x8 * (g - 1)));
            //return ratio;
            return 1.0f;

        }

        public static int GetGear()
        {
            return Telemetry.m.Sim.Player.Gear;
            /*
            if (Telemetry.m.Sim.Drivers.Player.Gear == 0) return 0;
            double CurrentRatio = GetGearRatio_Pure(Telemetry.m.Sim.Drivers.Player.Gear);

            for (int i = 0; i <= Telemetry.m.Sim.Drivers.Player.Gears; i++)
            {
                double d = CurrentRatio / ShiftRpm.GetRatio(i);
                if (d < 1.03 && d > 0.97)
                    return i;
                if (d < 0.97 && i == Telemetry.m.Sim.Drivers.Player.Gears + 3)
                    return Telemetry.m.Sim.Drivers.Player.Gears;
            }
            return 0;
             */
        }
        #endregion
        #region Package

        private Semaphore SerialPort_Lock = new Semaphore(1, 1);
        private void SendPackage(DashboardPackages package, byte[] data)
        {
            DevicePacket pk = new DevicePacket((int)package, data);
            Telemetry.m.Peripherals.TX(pk, "");
            return;
            SerialPort_Lock.WaitOne();
            DashboardPacket packet = new DashboardPacket();
            packet.Sync1 = '$';
            packet.Sync2 = '&';
            packet.length = (UInt16)(data.Length);
            packet.id = (byte)package;

            byte[] bPacket = ByteMethods.ToBytes(packet);
            sp.Write(bPacket, 0, 5);
            sp.Write(data, 0, data.Length);
            byte[] bf = new byte[2];
            bf[0] = 0;
            bf[1] = 0;
            sp.Write(bf, 0, 2);
            SerialPort_Lock.Release();
        }
        #endregion

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (sp.BytesToRead > 0)
                {
                    sp.ReadByte();
                }
            }
            catch (Exception ex) { }
        }










        private byte YellowFlagIntensity()
        {
            if (Telemetry.m.Sim.Drivers.Player.Flag_Yellow)
                return 0xFF;
            
            int distance = 0;
            bool wave_flag = false;
            double shortest_situation = 1000000;
            double mymeters = Telemetry.m.Sim.Drivers.Player.MetersDriven;
            lock (Telemetry.m.Sim.Drivers.AllDrivers)
            {
                foreach (IDriverGeneral driver in Telemetry.m.Sim.Drivers.AllDrivers)
                {
                    if (driver == null) continue;
                    if (!driver.Retired && driver.Flag_Yellow && !driver.Pits && driver.Speed * 3.6 < 120)
                    {
                        wave_flag = true;
                        double dist = 0;
                        double d1 = driver.MetersDriven - mymeters;
                        if (d1 < 0)
                        {
                            if (Telemetry.m.Track == null)
                                dist = 400;
                            else
                            {
                                
                                double mydist_to_end = Telemetry.m.Track.Length - mymeters;
                                double hisdist_from_begin = driver.MetersDriven;
                                double d2 = hisdist_from_begin + mydist_to_end;
                                dist = d2;
                            }
                        }
                        else dist = d1;
                        if (dist > 0)
                            shortest_situation = Math.Min(shortest_situation, dist);
                    }
                }
            }
            if (!wave_flag) return 0;
            else
            {
                if (shortest_situation < 400) return 0xFF;
                else
                {
                    int va = (int) (shortest_situation - 400) / 10;
                    if (va > 240 || va < 25) return 0;
                    else
                        return (byte)(va);
                }
            }

        }


        private void Tickjes()
        {
            int i = 0;
            int timestep = 10;
            while (true)
            {
                Thread.Sleep(timestep);
                i += timestep;

                if (Telemetry.m.Active_Session)
                {
                    ThreadPool.QueueUserWorkItem(Fire_HS, null);

                    if (i > 2500)
                    {
                        ThreadPool.QueueUserWorkItem(Fire_LS, null);
                        i = 0;
                    }
                }
            }
        }

        private void Fire_LS(object o)
        {
            try
            {
                ICar car = Telemetry.m.Sim.Garage.SearchCar(Telemetry.m.Sim.Drivers.Player.CarClass,
                                                 Telemetry.m.Sim.Drivers.Player.CarModel);
                car.Scan();
                car.ScanGeneral();
                car.ScanEngine();

            GameData_LS package = new GameData_LS();

            /** TYRE TEMPERATURES **/
            try
            {
                if (Telemetry.m.Sim.Player.Tyre_Temperature_LF_Middle > 0)
                {
                    package.TyreTemperature_LF = Convert.ToByte(Limits(Telemetry.m.Sim.Player.Tyre_Temperature_LF_Middle - 273.16, 0, 150));
                    package.TyreTemperature_RF = Convert.ToByte(Limits(Telemetry.m.Sim.Player.Tyre_Temperature_RF_Middle - 273.16, 0, 150));
                    package.TyreTemperature_LR = Convert.ToByte(Limits(Telemetry.m.Sim.Player.Tyre_Temperature_LR_Middle - 273.16, 0, 150));
                    package.TyreTemperature_RR = Convert.ToByte(Limits(Telemetry.m.Sim.Player.Tyre_Temperature_RR_Middle - 273.16, 0, 150));
                }
            }
            catch (Exception ex)
            { }
            /** TYRE & BRAKE WEAR **/
            // package.TyreWear_F = Convert.ToByte(200 * Limits(Telemetry.m.Sim.Drivers.Player.Wheel_LeftFront.Wear + Telemetry.m.Sim.Drivers.Player.Wheel_RightFront.Wear, 0, 2) / 2.0);
            //package.TyreWear_R = Convert.ToByte(200 * Limits(Telemetry.m.Sim.Drivers.Player.Wheel_LeftRear.Wear + Telemetry.m.Sim.Drivers.Player.Wheel_RightRear.Wear, 0, 2) / 2.0);
            package.BrakeWear_F = 0;
            package.BrakeWear_R = 0;

            /** CURRENT FUEL INFO **/
            package.Fuel_Litre = Convert.ToUInt16(Limits(Telemetry.m.Sim.Player.Fuel, 0, 150) * 10);
            package.Fuel_Laps = 0;

            /** DRIVING TIMES **/
            package.Laptime_Last = Convert.ToSingle(Limits(Telemetry.m.Sim.Drivers.Player.LapTime_Last,0,1200));
            package.Laptime_Best = Convert.ToSingle(Limits(Telemetry.m.Sim.Drivers.Player.LapTime_Best,0,1200));
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
            package.Split_Sector = Convert.ToSingle(Limits(package.Split_Sector, -2, 2));
            package.Split_Lap = Telemetry.m.Sim.Drivers.Player.LapTime_Best - Telemetry.m.Sim.Drivers.Player.LapTime_Last;
            package.Split_Lap = Convert.ToSingle(Limits(package.Split_Lap, -2, 2));

            package.RaceTime = (float)Limits(Telemetry.m.Sim.Session.Time - 30, 0, 3600);
            package.RaceLength = 3600;

            SendPackage(DashboardPackages.PACK_GAMEDATA_LS, ByteMethods.ToBytes(package));

            /*** Temporarely do cardata here as well ***/
            CarData cardata = new CarData();

            cardata.RPM_Max = Convert.ToUInt16(Limits(Telemetry.m.Sim.Player.Engine_RPM_Max_Live ,200,25000)-200);
            cardata.RPM_Idle = Convert.ToUInt16(Limits(car.Engine.IdleRPM * rads_to_rpm, 0, 7000));
                cardata.RPM_Idle = 1000;

            //cardata.HP_Max = Convert.ToUInt16(EngineCurve.GetMaxHP());
            cardata.Gears = (byte)Limits(car.Gearbox.Gears,0, 10);
            //cardata.Fuel_Max = (byte)Limits(car.General.Fueltank,0,100);
                cardata.Fuel_Max = 100;
            cardata.GearRatio0 = Convert.ToSingle(Limits(GetGearRatio_Pure(0),0,20));
            cardata.GearRatio1 = Convert.ToSingle(Limits(GetGearRatio_Pure(1),0,20));
            cardata.GearRatio2 = Convert.ToSingle(Limits(GetGearRatio_Pure(2),0,20));
            cardata.GearRatio3 = Convert.ToSingle(Limits(GetGearRatio_Pure(3),0,20));
            cardata.GearRatio4 = Convert.ToSingle(Limits(GetGearRatio_Pure(4),0,20));
            cardata.GearRatio5 = Convert.ToSingle(Limits(GetGearRatio_Pure(5),0,20));
            cardata.GearRatio6 = Convert.ToSingle(Limits(GetGearRatio_Pure(6),0,20));
            cardata.GearRatio7 = Convert.ToSingle(Limits(GetGearRatio_Pure(7),0,20));


            //if (Telemetry.m.Sim.Name == "GTR2") // messy bugfix
            /*
            {
                cardata.GearRatio0 = 12.0f;
                cardata.GearRatio1 = cardata.GearRatio0 * 0.85f;
                cardata.GearRatio2 = cardata.GearRatio1 * 0.85f;
                cardata.GearRatio3 = cardata.GearRatio2 * 0.85f;
                cardata.GearRatio4 = cardata.GearRatio3 * 0.85f;
                cardata.GearRatio5 = cardata.GearRatio4 * 0.85f;
                cardata.GearRatio6 = cardata.GearRatio5 * 0.85f;
            }*/
            ShiftRpm r = new ShiftRpm();
            cardata.RPM_Shift0 = cardata.RPM_Max;
            cardata.RPM_Shift1 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(1)));
            cardata.RPM_Shift2 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(2)));
            cardata.RPM_Shift3 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(3)));
            cardata.RPM_Shift4 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(4)));
            cardata.RPM_Shift5 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(5)));
            cardata.RPM_Shift6 = Convert.ToUInt16(r.Get(1, r.GetRatioBetween(6)));
            /*ShiftRpm r = new ShiftRpm();
            cardata.RPM_Shift0 = cardata.RPM_Max;
            cardata.RPM_Shift1 = (ushort)Limits(cardata.RPM_Max, 0, 25000);
            cardata.RPM_Shift2 = (ushort)Limits(cardata.RPM_Max, 0, 25000);
            cardata.RPM_Shift3 = (ushort)Limits(cardata.RPM_Max, 0, 25000);
            cardata.RPM_Shift4 = (ushort)Limits(cardata.RPM_Max, 0, 25000);
            cardata.RPM_Shift5 = (ushort)Limits(cardata.RPM_Max, 0, 25000);
            cardata.RPM_Shift6 = (ushort)Limits(cardata.RPM_Max, 0, 25000);
            cardata.RPM_Shift7 = (ushort)Limits(cardata.RPM_Max, 0, 25000);*/

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
                bf[i + 6] = 64;
                bf[i + 7] = Convert.ToByte(k * 50 / 16);
                bf[i + 8] = 255;

                i += 9;
                k++;
            }
                string t = BitConverter.ToString(bf);
            SendPackage(DashboardPackages.PACK_PREFS_SHIFTBAR, bf);
                /*
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
                byte[] bf_tmp = BitConverter.GetBytes(600);
                bf[i + 4] = bf_tmp[0];
                bf[i + 5] = bf_tmp[1];
                bf[i + 6] = 50;
                bf[i + 7] = Convert.ToByte(((k % 2 == 0) ? 50 : 0));
                bf[i + 8] = 200;

                i += 9;
                k++;
            }
            SendPackage(DashboardPackages.PACK_PREFS_SHIFTBAR, bf);
                */

            }
            catch (Exception ex)
            { }
            return;

        }

        private float PreviousTime = 0;

        private double Limits(double v, double min, double max)
        {
            if (double.IsInfinity(v)) v = 0;
            if (double.IsNaN(v)) v = 0;
            return Math.Max(min, Math.Min(max, v));
        }
        private void Fire_HS(object o)
        {
            try
            {

                //if (Telemetry.m.Sim.Drivers.Player.PitLimiter || Telemetry.m.Sim.Drivers.Player.Pits)
                //    return;
                GameData_HS package = new GameData_HS();

                /** DRIVER INPUTS **/
                package.Throttle = Convert.ToByte(200 * Limits(Telemetry.m.Sim.Player.Pedals_Throttle,0,1));
                package.Brake = Convert.ToByte(200 * Limits(Telemetry.m.Sim.Player.Pedals_Brake,0,1));
                package.Clutch = Convert.ToByte(200 * Limits(Telemetry.m.Sim.Player.Pedals_Clutch,0,1));
                package.Steer = Convert.ToByte(100 * (1 + Limits(Telemetry.m.Sim.Player.SteeringAngle, -1, 1)));

                /** PITS SITUATION ETC **/
                package.PitLimiter = (byte)((Telemetry.m.Sim.Drivers.Player.PitLimiter) ? 1 : 0);
                package.InPits = (byte)((Telemetry.m.Sim.Drivers.Player.Pits) ? 1 : 0);
                package.PitRequired = 0; // TODO;
                package.EngineStall = (byte)((Telemetry.m.Sim.Drivers.Player.RPM < 500) ? 1 : 0);


                /** DRIVING INFO **/
                package.Gear = (byte)((Telemetry.m.Sim.Player.Gear == 255) ? 11 : GetGear());
                package.Position = (byte)Telemetry.m.Sim.Drivers.Player.Position;
                package.Wheelslip = 0; // TODO
                package.Cars = (byte)Telemetry.m.Sim.Session.Cars;

                /** TRACK INFO **/
                package.Flag = (byte)Flags.FLAG_Clear; // TODO 

                //BLACK
                /*if (Telemetry.m.Sim.Drivers.Player.Flag_Black)
                    package.Flag = (byte)Flags.FLAG_Black;
                //BLUE
                if (Telemetry.m.Sim.Drivers.Player.Flag_Blue)
                    package.Flag = (byte)Flags.FLAG_Blue;

                //YELLOW
                package.FlagIntensity = YellowFlagIntensity();
                if (package.FlagIntensity != 0)
                    package.Flag = (byte)Flags.FLAG_Yellow;

                //FULLCOURSE
                if (Telemetry.m.Sim.Session.Flag_YellowFull)
                    package.Flag = (byte)Flags.FLAG_FullYellow;*/
                package.Flag = 0;
                //Console.WriteLine(((Flags) package.Flag).ToString() + " @ " + package.FlagIntensity);
                package.Temp_Water = Convert.ToByte(Limits(Telemetry.m.Sim.Player.Engine_Temperature_Water,0,150) % 255);
                package.Temp_Oil = Convert.ToByte(Limits(Telemetry.m.Sim.Player.Engine_Temperature_Oil,0,150) % 255);
                package.Temp_Track = Convert.ToByte(Limits(Telemetry.m.Sim.Session.TrackTemperature,0,50));


                /** MORE DRIVING **/
                package.Speed = Convert.ToUInt16(Math.Round(Math.Abs(Limits(Telemetry.m.Sim.Drivers.Player.Speed,0,125) * 3.6)));
                package.RPM = Convert.ToUInt16(Math.Round(Limits(rads_to_rpm * Telemetry.m.Sim.Drivers.Player.RPM, 0, 25000)));
                double torque = Limits(Telemetry.m.Sim.Player.Engine_Torque,-200,1000);
                if (double.IsNaN(torque) == false && double.IsInfinity(torque) == false && torque > -10000 && torque <= 100000)
                {
                    package.Engine_HP =
                        Convert.ToUInt16(
                            Math.Round(Math.Max(0,
                                                torque * Telemetry.m.Sim.Player.Engine_RPM *
                                                rads_to_rpm / 5252)));
                }
                if (Telemetry.m.Sim.Modules.DistanceOnLap)
                package.MetersDriven = Convert.ToUInt16(Math.Max(0, Telemetry.m.Sim.Drivers.Player.MetersDriven));

                /** LIVE DRIVING TIMES **/
                package.Laptime_Current = 0; // TODO
                package.Gap_Front = (float)Splits.Split;
                if (float.IsNaN(package.Gap_Front) || float.IsInfinity(package.Gap_Front))
                    package.Gap_Front = 0;
                if (package.Gap_Front >= 9.99)
                    package.Gap_Front = 9.99;
                package.Gap_Back = 0; // TODO

                package.Wipers = 0;
                package.Lights = (byte)((Telemetry.m.Sim.Drivers.Player.HeadLights) ? 1 : 0); ;
                //package.Ignition = (byte)((Telemetry.m.Sim.Player.i) ? 1 : 0); ;
                package.Ignition = (byte)((Telemetry.m.Sim.Drivers.Player.Ignition) ? 1 : 0); ;
                package.Ignition = 1;
                package.Pause = 0;// (byte)((Telemetry.m.Sim.Session.Time == PreviousTime) ? 1 : 0);
                PreviousTime = Telemetry.m.Sim.Session.Time;

                // Send package
                SendPackage(DashboardPackages.PACK_GAMEDATA_HS, ByteMethods.ToBytes(package));
            }
            catch (Exception ex)
            {
            }
        }

    }
}