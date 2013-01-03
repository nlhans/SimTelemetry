using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using SimTelemetry.Core.Exceptions;
using SimTelemetry.Domain.Events;
using SimTelemetry.Objects.Plugins;

namespace SimTelemetry.Core.Plugins
{
    public class Plugins : IDisposable
    {
        /// <summary>
        /// Directory that needs to be searched for plug-ins
        /// </summary>
        public string PluginDirectory { get; set; }

        #region Imported objects
        /// <summary>
        /// List of simulator objects available in catalog. Searches for objects implementing ISimulator.
        /// </summary>
        [ImportMany(typeof(ISimulator))]
        public IEnumerable<ISimulator> Simulators { get; protected set; }

        /// <summary>
        /// List of graphic interface plug-ins.
        /// </summary>
        [ImportMany(typeof(IWidget))]
        public IEnumerable<IWidget> Widgets { get; protected set; }

        /// <summary>
        /// List of 'extension' objects; these can access simulator data and handle it for e.g. hardware devices.
        /// </summary>
        [ImportMany(typeof(IExtension))]
        public IEnumerable<IExtension> Extensions { get; protected set; }
        #endregion

        public Plugins()
        {
            //
            
        }

        public void Load()
        {
            if (!Directory.Exists(PluginDirectory))
                throw new PluginHostException("Could not find plug-in directory!");

            try
            {
                var catalog = new DirectoryCatalog(PluginDirectory, "SimTelemetry.Game.*.dll");
                catalog.Refresh();

                var container = new CompositionContainer(catalog);
                container.ComposeParts(this);

            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var exc in ex.LoaderExceptions)
                    Events.Fire(new DebugWarningException("Error whilst importing plugin namespaces; the following type couldn't be loaded correctly.", exc), false);
            }
            catch (Exception ex)
            {
                throw new PluginHostException("Could not initialize plug-ins!", ex);
            }

            if (Simulators == null)
            {
                throw new PluginHostException("Simulators aren't properly initialized");
            }
            if (Widgets == null)
            {
                throw new PluginHostException("Widgets aren't properly initialized");
            }
            if (Extensions == null)
            {
                throw new PluginHostException("Extensions aren't properly initialized");
            }

            // 'Initialize' all plug-ins
            foreach (var sim in Simulators) sim.Initialize();
            foreach (var widget in Widgets) widget.Initialize();
            foreach (var ext in Extensions) ext.Initialize();
        }

        public void Unload()
        {
            foreach (var sim in Simulators) sim.Deinitialize();
            foreach (var widget in Widgets) widget.Deinitialize();
            foreach (var ext in Extensions) ext.Deinitialize();

            Simulators = null;
            Widgets = null;
            Extensions = null;

        }

        public void Dispose()
        {
            PluginDirectory = string.Empty;

        }
    }
}
