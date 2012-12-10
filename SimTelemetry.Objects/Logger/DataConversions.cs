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
namespace SimTelemetry.Objects
{
    public enum DataConversions
    {
        ROTATION_RADS_TO_RPM,
        ROTATION_RADS_TO_RPS,
        ROTATION_RPS_TO_RPM,
        ROTATION_RPS_TO_RADS,
        ROTATION_RPM_TO_RADS,
        ROTATION_RPM_TO_RPS,

        SPEED_MS_TO_KMH,
        SPEED_MS_TO_MPH,
        SPEED_KMH_TO_MS,
        SPEED_KMH_TO_MPH,
        SPEED_MPH_TO_MS,
        SPEED_MPH_TO_KMH,

        TEMPERATURE_KELVIN_TO_CELSIUS,
        TEMPERATURE_KELVIN_TO_FAHRENHEIT,
        TEMPERATURE_CELSIUS_TO_KELVIN,
        TEMPERATURE_CELSIUS_TO_FAHRENHEIT,
        TEMPERATURE_FAHRENHEIT_TO_KELVIN,
        TEMPERATURE_FAHRENHEIT_TO_CELSIUS,


    }
}