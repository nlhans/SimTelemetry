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

using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveTelemetry
{
    public class BufferedFlowLayoutPanel  : FlowLayoutPanel
    {
        public BufferedFlowLayoutPanel() : base()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.AutoScroll = true;

            this.BorderStyle = BorderStyle.None;

        }

        public void Rebuffer()
        {
            // This fixes an odd bug where the AutoWrap function is not functioning properly when the container is sized up and then shrunk.
            // Fixed by readding all controls.
            List<Control> controls = new List<Control>();
            for (int i = 0; i < this.Controls.Count; i++) controls.Add(this.Controls[i]);

            this.Controls.Clear();
            this.Controls.AddRange(controls.ToArray());
        }

    }
}