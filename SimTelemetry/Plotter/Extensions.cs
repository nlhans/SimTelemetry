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
using System.Drawing;

namespace SimTelemetry
{
    public  static partial class Extensions
    {
        public static void FillEllipse(this Graphics g, Brush b, double x, double y, double sx, double sy)
        {
            g.FillEllipse(b, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(sx), Convert.ToSingle(sy));
        }

        public static void DrawLine(this Graphics g, Pen p, double x, double y, double x2, double y2)
        {
            g.DrawLine(p, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(x2), Convert.ToSingle(y2));
        }
        public static void DrawEllipse(this Graphics g, Pen p, double x, double y, double sx, double sy)
        {
            g.DrawEllipse(p, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(sx), Convert.ToSingle(sy));
        }

        public static void DrawString(this Graphics g, string s, System.Drawing.Font f, Brush b, double x, double y)
        {
            g.DrawString(s, f, b, Convert.ToSingle(x), Convert.ToSingle(y));
        }
    }
}