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
namespace SimTelemetry.Controls
{
    public struct WayPoint
    {
        public string Pos;
        public double PosX;
        public double PosZ;
        public string Perp;
        public string Normal;
        public string Vect;
        public string Width;
        public string DWidth;
        public string Path;
        public string LockedAlpha;
        public string GAlpha;
        public string Groove_lat;
        public string Test_speed;
        public string Score;
        public string Sector;
        public double Distance;
        public string Cheat;
        public string Pathabstractionspeed;
        public string Pathabstraction;
        public string Wpse;
        public string BranchID;
        public string BranchIDstr;
        public string Bitfields;
        public string LockedLats;
        public string Multipathlat;
        public string Pitlane;
        public string PTRS;
        public string PTRS4;
    }
}