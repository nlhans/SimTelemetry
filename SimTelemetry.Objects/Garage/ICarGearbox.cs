﻿/*************************************************************************
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
namespace SimTelemetry.Objects.Garage
{
    public interface ICarGearbox
    {
        string File { get; }
        
        double ShiftTime_Up { get; }
        double ShiftTime_Down { get; }
        
        int Gears { get; }

        double[] Ratios { get; }
        double[] Finals { get; }

        double[] Ratio_Setup { get; }
        double[] Ratio_Stock { get; }

        double Final_Setup { get; }
        double Final_Stock{ get; }
    }
}