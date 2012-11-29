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
namespace SimTelemetry.Objects.Garage
{
    public interface ITrack
    {
        string File { get; }
        string Name { get; }
        string Location { get; }
        string Version { get; }
        string Type { get; }
        bool ImageCache { get; }

        double Length { get; }
        string Qualify_Day { get; }
        double Qualify_Start { get; }
        int Qualify_Laps { get; }
        int Qualify_Minutes { get; }

        string FullRace_Day { get; }
        double FullRace_Start { get; }
        int FullRace_Minutes { get; }
        int FullRace_Laps { get; }

        bool Pitlane { get; }
        int StartingGridSize { get; }
        int PitSpots { get; }

        int PitSpeed_Practice { get; }
        int PitSpeed_Race { get; }

        double Laprecord_Race_Time { get; }
        string Laprecord_Race_Driver { get; }
        double Laprecord_Qualify_Time { get; }
        string Laprecord_Qualify_Driver { get; }

        RouteCollection Route { get; }
        string Thumbnail { get; }

        void Scan();
        void ScanRoute();
    }
}