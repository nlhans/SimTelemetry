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

namespace SimTelemetry.Peripherals
{
    struct GameData_LS
    {
        /** TYRE TEMPERATURES **/
        public byte TyreTemperature_LF;
        public byte TyreTemperature_RF;
        public byte TyreTemperature_LR;
        public byte TyreTemperature_RR;

        /** TYRE & BRAKE WEAR **/
        public byte TyreWear_F;
        public byte TyreWear_R;
        public byte BrakeWear_F;
        public byte BrakeWear_R;

        /** CURRENT FUEL INFO **/
        public UInt16 Fuel_Litre;
        public UInt16 Fuel_Laps;

        /** DRIVING TIMES **/
        public float Laptime_Last;
        public float Laptime_Best;
        public float Split_Sector;
        public float Split_Lap;

        public float RaceTime;
        public float RaceLength;
    }
}