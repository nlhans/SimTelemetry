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
using System.Drawing;

namespace SimTelemetry
{
    public class PlotterPalette
    {
        public Color Background { get { return Color.FromArgb(255,20,20,20); } }
        public Color AxisLine { get { return Color.FromArgb(255, 230,230, 230); } }
        public Color AxisText { get { return Color.FromArgb(255, 230, 230, 230); } }
        public Color Text { get { return Color.YellowGreen; } }
        public Color Window { get { return Color.LightGray; } }
        public Color Cursor { get { return Color.DarkRed; } }
        public Color[] Graphs
        {
            get
            {
                return new Color[4]
                           {
                               Color.Cyan,
                               Color.LightGreen,
                               Color.LightYellow,
                               Color.LightBlue
                           };
            }
        }
    }
}