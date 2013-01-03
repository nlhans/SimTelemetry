using System.Drawing;
using System.Windows.Forms;

namespace SimTelemetry.Objects.Plugins
{
    public interface  IWidget : IPlugin
    {
        Control Control { get; }

        Size MinimumSize { get; }
        Size MaximumSize { get; }
        bool Resizable { get; }
    }
}
