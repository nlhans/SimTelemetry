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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SimTelemetry
{
    public class PlotterConfigurations
    {
        private List<PlotterGraph> _graphs = new List<PlotterGraph>();
        public List<PlotterGraph> Graphs {get { return _graphs; }}

        public PlotterConfigurations()
        {

        }
        public void Configure(Plotter p)
        {
            p.Graphs.Clear();
            p.Graphs = _graphs;
        }
    

        public void Load(string file)
        {
            // read it
            if(!File.Exists(file))
                throw new Exception("File doesn't exist");
            string[] FileData = File.ReadAllLines(file);

            int GraphIndex = 0;
            foreach(string line in FileData)
            {
                string[] line_structure = new string[0], line_data = new string[0];
                if (line.Contains("="))
                {
                    line_structure = line.Trim().Split("=".ToCharArray(), 2);
                    if (line_structure.Length == 2) line_data = line_structure[1].Split(",".ToCharArray());
                }

                if(line.Trim()=="[Chart]")
                {
                    // A new chart
                    _graphs.Add(new PlotterGraph());
                    GraphIndex = _graphs.Count - 1;
                }

                if(line_structure.Length == 2)
                {
                    // A new curve of yaxis
                    switch(line_structure[0])
                    {
                        case "Curve":

                            PlotterCurves c = new PlotterCurves();

                            c.Legend = line_data[0];
                            c.LineColor = Color.FromName(line_data[1]);
                            Int32.TryParse(line_data[2], out c.LineThickness);
                            Int32.TryParse(line_data[3], out c.YAxis);

                            if(GraphIndex>-1)
                            _graphs[GraphIndex].Curves.Add(c);
                            break;

                        case "Yaxis":

                            PlotterYAxis y = new PlotterYAxis();

                            if (line_data.Length == 1)
                                y.auto = true;
                            else
                            {
                                bool.TryParse(line_data[0], out y.auto);
                                Double.TryParse(line_data[1], out y.min);
                                Double.TryParse(line_data[2], out y.max);
                                Double.TryParse(line_data[3], out y.step);
                                Double.TryParse(line_data[4], out y.step_divider);
                                Int32.TryParse(line_data[5], out y.digits_comma);
                            }

                            if (GraphIndex > -1)
                                _graphs[GraphIndex].Y_Axis.Add(y);
                            break;
                    }
                    
                }

            }

        }

    }
}
