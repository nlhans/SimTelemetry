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
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry.Data
{
    public sealed class Simulators
    {
        #if DEBUG
        DirectoryCatalog catalog = new DirectoryCatalog("./", "SimTelemetry.Game.*.dll");
        #else
        DirectoryCatalog catalog = new DirectoryCatalog("simulators/", "SimTelemetry.Game.*.dll");
        #endif


        [ImportMany(typeof(ISimulator))]
        public List<ISimulator> Sims { get; set; }
        
        #region Initializing & Plugins
        public Simulators()
        {

            try
            {
                Sims = new List<ISimulator>();
                Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading assemblies");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

        }

        private void Refresh()
        {
            catalog.Refresh();
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (ISimulator sim in Sims)
            {
                sim.Host = Telemetry.m;
                sim.Initialize();
            }
        }
        #endregion
        #region Helpers
        internal string _GetWithoutFileName(string f)
        {
            string file = Path.GetFileName(f);
            return f.Substring(0, f.Length - file.Length);
        }
        #endregion

        public bool Available
        {
            get { return ((Sims.FindAll(delegate(ISimulator sim) { return sim.Memory.Attached; }).Count == 0) ? false : true); }
        }

        public List<ISimulator> GetAllRunning()
        {
            return Sims.FindAll(delegate(ISimulator sim) { return sim.Memory.Attached; });
        }

        public ISimulator GetRunning()
        {
            List<ISimulator> sms = Sims.FindAll(delegate(ISimulator sim) { return sim.Memory.Attached; });
            ;
            if (sms.Count > 0) return sms[0];
            else return null;
        }

    }
}
