using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Data;
using SimTelemetry.Objects.Peripherals;

namespace SimTelemetry.Peripherals.Peripherals
{
    public delegate void PotEvent(int pot, int direction, int steps);
    public class Switchboard
    {
        public event PotEvent PotTurn;
        private List<int> PotSettings = new List<int>();
        public Switchboard()
        {
            Telemetry.m.Peripherals.RX += Peripherals_RX;
            for(int i = 0; i < 6; i++)
                PotSettings.Add(0);

            this.PotTurn += new PotEvent(Switchboard_PotTurn);

        }

        private int Revs = 1000;
        private int CruiseControl = 200;

        void Switchboard_PotTurn(int pot, int direction, int steps)
        {
            // TODO: handle brake & throttle mapping
            int step = steps;
            switch(pot)
            {
                case 0: // brake bias
                    break;

                case 1: // engine boost
                    break;

                case 2: // engine revs
                    if (Revs < 1000)
                        step *= 5;
                    else if (Revs > 1750)
                        step *= 50;
                    else
                        step *= 10;

                    if (direction ==1)
                        Revs += step;
                    else
                        Revs -= step;

                    
                    DevicePacket MaximumRPM = new DevicePacket();
                    MaximumRPM.ID = Convert.ToInt32(DashboardPackages.PACK_PREFS_ENGINEREVS);
                    MaximumRPM.Length = 4;
                    MaximumRPM.Data = BitConverter.GetBytes(Convert.ToInt32(Revs));

                    Telemetry.m.Peripherals.TX(MaximumRPM, "Dashboard");
                    break;

                case 3: // menu
                    break;

                case 4: // throttle map
                    break;

                case 5: // brake map
                    // NOW CRUISE CONTROL
                    if (CruiseControl < 50)
                        step *= 1;
                    if (CruiseControl < 100)
                        step *= 2;
                    else if (CruiseControl < 180)
                        step *= 5;
                    else
                        step *= 10;

                    if (direction == 1)
                        CruiseControl += step;
                    else
                        CruiseControl -= step;
                    
                    DevicePacket Speed = new DevicePacket();
                    Speed.ID = Convert.ToInt32(DashboardPackages.PACK_PREFS_CRUISECONTROL);
                    Speed.Length = 4;
                    Speed.Data = BitConverter.GetBytes(Convert.ToInt32(CruiseControl));

                    Telemetry.m.Peripherals.TX(Speed, "Switchboard");
                    Telemetry.m.Peripherals.TX(Speed, "Dashboard");

                    break;

            }
        }

        void Peripherals_RX(DevicePacket packet, object sender)
        {
            IDevice device = (IDevice) sender;
                if(packet.ID == Convert.ToInt32(DashboardPackages.PACK_POTSINFO))
                {
                    // new potentiometer information!   
                    for(int i= 0 ; i <  6; i++)
                    {
                        Int16 current = BitConverter.ToInt16(packet.Data, i*2);
                        
                        int diff = current - PotSettings[i];
                        PotSettings[i] = current;
                        if (Math.Abs(diff) < 100)
                        {
                            if (diff != 0 && PotTurn != null)
                                PotTurn(i, ((diff > 0) ? 1 : -1), Math.Abs(diff));
                        }
                    }
                }

        }
    }
}
