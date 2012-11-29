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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using SimTelemetry.Game.Network;
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry.Data
{
    public sealed class Simulators
    {

        /// <summary>
        /// DLL catalog containing all simulator plug-ins. In debug the bin/ directory is used, because the program
        /// has direct references to all plug-ins (meaning on compile they will be automatically recompiled and copied)
        /// In release mode the simulators should be placed in the directory Simulators.
        /// </summary>
#if DEBUG
        DirectoryCatalog catalog = new DirectoryCatalog("./", "SimTelemetry.Game.*.dll");
#else
        DirectoryCatalog catalog = new DirectoryCatalog("simulators/", "SimTelemetry.Game.*.dll");
#endif

        /// <summary>
        /// List of simulator objects available in catalog. Searches for objects implementing ISimulator.
        /// </summary>
        [ImportMany(typeof(ISimulator))]
        public List<ISimulator> _Sims { get; set; }
        public List<ISimulator> Sims{get
        {
            if (Telemetry.m.Net.IsClient)
            {
                List<ISimulator> simList= new List<ISimulator>();
                simList.Add(Network);
                return simList;
            }
            else
            {
                return _Sims;
            }
        }
        set { _Sims = value; }}

        public ISimulator Network { get; set; }

        /// <summary>
        /// Upon initializing try loading all simulators from the catalog.
        /// </summary>
        public Simulators()
        {
            Sims = new List<ISimulator>();
            Refresh();
            
        }

        /// <summary>
        /// Refreshes the catalog and initializes all simulators with the host (Telemetry class).
        /// </summary>
        private void Refresh()
        {
            try
            {
                catalog.Refresh();
                CompositionContainer container = new CompositionContainer(catalog);
                container.ComposeParts(this);

                foreach (ISimulator sim in Sims)
                {
                    sim.Host = Telemetry.m;
                    sim.Initialize();
                }

                if (Network != null)
                {
                    throw new Exception("Network already added");
                }
                Network = new NetworkGame {Host = Telemetry.m};
                Network.Initialize();

                Sims.Add(Network);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading assemblies");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Returns whether a simulator is available that is active (memory reader is attached).
        /// </summary>
        public bool Available
        {
            get
            {
                return ((GetAllRunning().Count == 0) ? false : true);
            }
        }

        /// <summary>
        /// Gets all simulators that are running(in case of collisions).
        /// </summary>
        /// <returns></returns>
        public List<ISimulator> GetAllRunning()
        {
            return Sims.FindAll(delegate(ISimulator sim)
                                    {
                                        return sim.Attached; // TODO: Use localized detection; not all may use memory.
                                    });
        }

        /// <summary>
        /// Gets the simulator that is running. If mutltiple are; the first one is picked.
        /// </summary>
        /// <returns></returns>
        public ISimulator GetRunning()
        {
            
            if (Network != null && Network.Attached)
                return Network;
            List<ISimulator> sms = GetAllRunning();
            if (sms.Count > 0) 
                return sms[0];
            else 
                return null;
        }

    }
}