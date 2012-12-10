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
using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor
{
    public class Lap : ILap
    {
        private int _lap;
        private float _sector1;
        private float _sector2;
        private float _sector3;

        public Lap(int lap, float s1, float s2, float s3)
        {
            _lap = lap;
            _sector1 = s1;
            _sector2 = s2;
            _sector3 = s3;
        }

        public int LapNumber { get { return _lap; } }

        public float Sector1 { get { return _sector1; } }
        public float Sector2 { get { return _sector2; } }
        public float Sector3 { get { return _sector3; } }

        public float LapTime
        {
            get { return _sector1 + _sector2 + _sector3; }
        }
    }
}