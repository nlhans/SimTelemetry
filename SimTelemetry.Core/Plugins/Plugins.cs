using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using SimTelemetry.Core.Events;
using SimTelemetry.Core.Exceptions;
using SimTelemetry.Objects.Plugins;

namespace SimTelemetry.Core.Plugins
{
    public class Plugins : IDisposable
    {
        /// <summary>
        /// Directory that needs to be searched for plug-ins
        /// </summary>
        public string PluginDirectory { get; set; }

        protected DirectoryCatalog PluginCatalog { get; set; }
        protected CompositionContainer PluginContainer { get; set; }

        #region Imported objects
        /// <summary>
        /// List of simulator objects available in catalog. Searches for objects implementing ISimulator.
        /// </summary>
        [ImportMany(typeof(ISimulator))]
        public IList<ISimulator> Simulators { get; protected set; }

        /// <summary>
        /// List of graphic interface plug-ins.
        /// </summary>
        [ImportMany(typeof(IWidget))]
        public IList<IWidget> Widgets { get; protected set; }

        /// <summary>
        /// List of 'extension' objects; these can access simulator data and handle it for e.g. hardware devices.
        /// </summary>
        [ImportMany(typeof(IExtension))]
        public IList<IExtension> Extensions { get; protected set; }
        #endregion

        /// <summary>
        /// Refresh the plugin catalog.
        /// </summary>
        public void Load()
        {
            // Check if environmental settings are correct:
            if (!Directory.Exists(PluginDirectory))
            {
                throw new PluginHostException("Could not find plug-in directory!");
            }

            if (Simulators != null || Widgets != null || Extensions != null)
            {
                throw new PluginHostException("Can only load plug-ins from a clean set of lists.");
            }

            var simulatorsToDrop = new List<ISimulator>();
            var widgetsToDrop = new List<IWidget>();
            var extensionsToDrop = new List<IExtension>();

            Simulators = new List<ISimulator>();
            Widgets = new List<IWidget>();
            Extensions = new List<IExtension>();

            // Try to refresh DLL's from the plugin directory:
            try
            {
                PluginCatalog = new DirectoryCatalog(PluginDirectory, "SimTelemetry.Plugins.*.dll");
                PluginCatalog.Refresh();

               PluginContainer = new CompositionContainer(PluginCatalog);
               PluginContainer.ComposeParts(this);
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var exc in ex.LoaderExceptions)
                    GlobalEvents.Fire(new DebugWarning("Error whilst importing plugin namespaces; the following type couldn't be loaded correctly.", exc), false);
                throw new PluginHostException("Could not initialize plug-ins!", ex);
            }
            catch (CompositionException ex)
            {
                foreach (var exc in ex.Errors)
                    GlobalEvents.Fire(new DebugWarning("Error whilst importing plugin namespaces; the following type couldn't be loaded correctly.", exc.Exception), false);
                throw new PluginHostException("Could not initialize plug-ins!", ex);
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
            // Initialize all plug-ins
            foreach (var sim in Simulators)
            {
                string simName = "??";
                try
                {
                    simName = sim.Name;
                    sim.Initialize();
                }
                catch (Exception ex)
                {
                    simulatorsToDrop.Add(sim);
                    GlobalEvents.Fire(new DebugWarning("Unloading simulator plugin '" + simName + "' (assembly " + ex.Source + "), exception was thrown during initialize()", ex), false);
                }
            }
            Simulators = Simulators.Where(x => !simulatorsToDrop.Contains(x)).ToList();

            foreach (var widget in Widgets)
            {
                string widgetName = "??";
                try
                {
                    widgetName = widget.Name;
                    widget.Initialize();
                }
                catch (Exception ex)
                {
                    widgetsToDrop.Add(widget);
                    GlobalEvents.Fire(new DebugWarning("Unloading widget plugin '" + widgetName + "' (assembly " + ex.Source+ "), exception was thrown during initialize()", ex), false);
                }
            }
            Widgets = Widgets.Where(x => !widgetsToDrop.Contains(x)).ToList();

            foreach (var ext in Extensions)
            {
                string extName = "??";
                try
                {
                    extName = ext.Name;
                    ext.Initialize();
                }
                catch (Exception ex)
                {
                    extensionsToDrop.Add(ext);
                    GlobalEvents.Fire(new DebugWarning("Unloading extension plugin '" + extName + "' (assembly " + ex.Source + "), exception was thrown during initialize()", ex), false);
                }
            }
            Extensions = Extensions.Where(x => !extensionsToDrop.Contains(x)).ToList();

            // Fire PluginsLoaded event
            var loadEvent = new PluginsLoaded
                                {
                                    Simulators = Simulators,
                                    Widgets = Widgets,
                                    Extensions = Extensions,
                                    FailedSimulators = simulatorsToDrop,
                                    FailedWidgets = widgetsToDrop,
                                    FailedExtensions = extensionsToDrop
                                };

            GlobalEvents.Fire(loadEvent, true);
        }

        /// <summary>
        /// Unload the plugin catalog.
        /// Warning: all assemblies created at Load are not destroyed, so limit the usage of this function.
        /// </summary>
        public void Unload()
        {
            if (Simulators == null)
                return;

            foreach (var sim in Simulators)
            {
                try
                {
                    sim.Deinitialize();
                }
                catch (Exception ex)
                {
                    GlobalEvents.Fire(
                        new DebugWarning(
                            "Unloading simulator plugin '" + sim.Name + "' (assembly " + ex.Source +
                            ") failed; exception was thrown during deinitialize()", ex), false);
                }
            }
            foreach (var widget in Widgets)
            {
                try
                {
                    widget.Deinitialize();
                }
                catch (Exception ex)
                {
                    GlobalEvents.Fire(
                        new DebugWarning(
                            "Unloading widget plugin '" + widget.Name + "' (assembly " + ex.Source +
                            ") failed; exception was thrown during deinitialize()", ex), false);
                }
            }
            foreach (var ext in Extensions)
            {
                try
                {
                    ext.Deinitialize();
                }
                catch (Exception ex)
                {
                    GlobalEvents.Fire(
                        new DebugWarning(
                            "Unloading extension plugin '" + ext.Name + "' (assembly " + ex.Source +
                            ") failed; exception was thrown during deinitialize()", ex), false);
                }
            }

            Simulators = null;
            Widgets = null;
            Extensions = null;

        }

        public void Dispose()
        {
            PluginDirectory = string.Empty;
            Unload();

        }
    }
}
