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
namespace SimTelemetry.Objects
{
    public interface ISession
    {
        [LogOnChange]
        [LogProperty("Track Location", "Used for track loader. Location of track AIW.")]
        string GameData_TrackFile { get; set; }

        [LogOnChange]
        [LogProperty("Game Location", "Used for track loader. Location of game directory.")]
        string GameDirectory { get; set; }

        [LogOnChange]
        [LogProperty("No. of race laps", "Number of race laps in this session.")]
        int RaceLaps { get; set; }

        [LogOnChange]
        [LogProperty("Is Race Session", "Indicates TRUE or FALSE whether this is a race session")]
        bool IsRace { get; set; }

        [LogOnChange]
        [LogProperty("Track", "Name of the track")]
        string CircuitName { get; set; }

        [Loggable(1)]
        [LogProperty("Track Temperature", "The track asphalt temperature in degrees Celsius.")]
        float TrackTemperature { get; set; }

        [Loggable(1)]
        [LogProperty("Ambient Temperature", "The air temperature in degrees Celsius.")]
        float AmbientTemperature { get; set; }

        [LogOnChange]
        [LogProperty("Session Info", "Information about the session. Not yet logged.")]
        SessionInfo Type { get; set; }

        [Loggable(100)]
        [LogProperty("Time", "The session time, with begin of session taken as zero. Seconds.")]
        float Time { get; set; }

        [LogOnChange]
        [LogProperty("Clock", "The actual time of day.")]
        float TimeClock { get; set; }

        [LogOnChange]
        [LogProperty("No. of cars in pits", "The amount of cars in the pit street.")]
        int Cars_InPits { get; set; }

        [LogOnChange]
        [LogProperty("No. of cars on track", "The amount of cars on track.")]
        int Cars_OnTrack { get; set; }

        [LogOnChange]
        [LogProperty("No. of cars", "Total amount of cars in session")]
        int Cars { get; set; }

        //[Unloggable]
        [Loggable(1)]
        [LogProperty("Session Active", "Is the game currently running a session?")]
        bool Active { get; set; }

        [LogOnChange]
        [LogProperty("Flags: Full Course Yellow", "Indicates a full course yellow situation.")]
        bool Flag_YellowFull { get; set; }

        [LogOnChange]
        [LogProperty("Flags: Red", "Indicates a red flag situation.")]
        bool Flag_Red { get; set; }

        [LogOnChange]
        [LogProperty("Flags: Green", "Indicates a green flag situation, start/restart.")]
        bool Flag_Green { get; set; }

        [LogOnChange]
        [LogProperty("Flags: Chequered Flag", "Indicates a race finish situation.")]
        bool Flag_Finish { get; set; }

        [LogOnChange]
        [LogProperty("Offline session","Is this session offline or multiplayer?")]
        bool IsOffline { get; set; }
    }
}