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
    public struct CarData
    {

        /** SETUP INFO **/

        public ushort RPM_Shift0;
        public ushort RPM_Shift1;
        public ushort RPM_Shift2;
        public ushort RPM_Shift3;
        public ushort RPM_Shift4;
        public ushort RPM_Shift5;
        public ushort RPM_Shift6;
        public ushort RPM_Shift7;

        public float GearRatio0;
        public float GearRatio1;
        public float GearRatio2;
        public float GearRatio3;
        public float GearRatio4;
        public float GearRatio5;
        public float GearRatio6;
        public float GearRatio7;
        /** CAR INFO **/
        public UInt16 RPM_Max;
        public UInt16 RPM_Idle;

        public UInt16 Fuel_Max;
        public UInt16 HP_Max;

        public byte Gears;
    }
}