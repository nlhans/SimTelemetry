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