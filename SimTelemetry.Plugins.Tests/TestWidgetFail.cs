using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using SimTelemetry.Core;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Plugins;
using SimTelemetry.Tests.Events;

namespace SimTelemetry.Game.Tests
{
    [Export(typeof(IWidget))]
    public class TestWidgetFail : IWidget
    {
        public ITelemetry Host { get; set; }

        public TestWidgetFail()
        {
            GlobalEvents.Fire(new PluginTestWidgetConstructor(), false);
        }

        public string PluginId
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
    }
}