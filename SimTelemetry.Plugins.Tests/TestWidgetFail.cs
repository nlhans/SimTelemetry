using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Plugins;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Game.Tests
{
    [Export(typeof(IPluginWidget))]
    public class TestWidgetFail : IPluginWidget
    {
        public TestWidgetFail()
        {
            GlobalEvents.Fire(new PluginTestWidgetConstructor(), false);
        }

        public string PluginId
        {
            get { throw new NotImplementedException(); }
        }

        public int ID
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "Test Widget Fail"; }
        }

        public string Version
        {
            get { throw new NotImplementedException(); }
        }

        public string Author
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CompilationTime
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Deinitialize()
        {
            throw new NotImplementedException();
        }

        public Control Control
        {
            get { throw new NotImplementedException(); }
        }

        public Size MinimumSize
        {
            get { throw new NotImplementedException(); }
        }

        public Size MaximumSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool Resizable
        {
            get { throw new NotImplementedException(); }
        }

        public bool Equals(IPluginBase other)
        {
            return other.ID == ID;
        }
    }
}