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

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class Loggable : Attribute
    {
        public bool LogOnChange { get; internal set; }

        private double _Frequency = 0;
        public double Freqency { get { return _Frequency; } }

        public Loggable(bool OnChange)
        {
            _Frequency = 100; // check 100x per second
            LogOnChange = OnChange;
        }

        public Loggable(double frequency)
        {
            _Frequency = frequency;
            if (_Frequency <= 1) _Frequency = 1;
            else
            _Frequency = 100;

            LogOnChange = false;
        }
    }
}