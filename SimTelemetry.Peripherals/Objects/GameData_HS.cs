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
using System.Runtime.InteropServices;

namespace SimTelemetry.Peripherals
{
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct GameData_HS
    {
        /** CAR INPUTS **/
        public byte Throttle;
        public byte Brake;
        public byte Clutch;
        public byte Steer;

        /** PITS SITUATION ETC **/
        public byte PitLimiter;
        public byte InPits;
        public byte PitRequired;
        public byte EngineStall;

        /** DRIVING INFO **/
        public byte Gear;
        public byte Position;
        public byte Wheelslip;
        public byte Cars;

        /** TRACK INFO **/
        public byte Flag;
        public byte Temp_Water;
        public byte Temp_Oil;
        public byte Temp_Track;

        /** MORE DRIVING **/
        public UInt16 Speed;
        public UInt16 RPM;
        public UInt16 Engine_HP;
        public UInt16 MetersDriven;


        /** LIVE DRIVING TIMES **/
        public float Laptime_Current;
        public float Gap_Front;
        public float Gap_Back;
        public byte FlagIntensity;

        /* MORE SHIT */
        public byte Ignition;
        public byte Lights;
        public byte Pause;
        public byte Wipers;
    }
}