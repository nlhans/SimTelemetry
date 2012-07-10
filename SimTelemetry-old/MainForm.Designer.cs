using System.Windows.Forms;
using Triton.Controls;

namespace SimTelemetry
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cScreenSplit = new System.Windows.Forms.SplitContainer();
            this.cSidePanelSplit = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cScreenSplit)).BeginInit();
            this.cScreenSplit.Panel1.SuspendLayout();
            this.cScreenSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cSidePanelSplit)).BeginInit();
            this.cSidePanelSplit.Panel2.SuspendLayout();
            this.cSidePanelSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // cScreenSplit
            // 
            this.cScreenSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cScreenSplit.Location = new System.Drawing.Point(0, 0);
            this.cScreenSplit.Name = "cScreenSplit";
            // 
            // cScreenSplit.Panel1
            // 
            this.cScreenSplit.Panel1.Controls.Add(this.cSidePanelSplit);
            this.cScreenSplit.Size = new System.Drawing.Size(922, 498);
            this.cScreenSplit.SplitterDistance = 307;
            this.cScreenSplit.TabIndex = 0;
            // 
            // cSidePanelSplit
            // 
            this.cSidePanelSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cSidePanelSplit.Location = new System.Drawing.Point(0, 0);
            this.cSidePanelSplit.Name = "cSidePanelSplit";
            this.cSidePanelSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // cSidePanelSplit.Panel2
            // 
            this.cSidePanelSplit.Panel2.Controls.Add(this.button1);
            this.cSidePanelSplit.Panel2MinSize = 285;
            this.cSidePanelSplit.Size = new System.Drawing.Size(307, 498);
            this.cSidePanelSplit.SplitterDistance = 208;
            this.cSidePanelSplit.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 498);
            this.Controls.Add(this.cScreenSplit);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.cScreenSplit.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cScreenSplit)).EndInit();
            this.cScreenSplit.ResumeLayout(false);
            this.cSidePanelSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cSidePanelSplit)).EndInit();
            this.cSidePanelSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer cScreenSplit;
        private System.Windows.Forms.SplitContainer cSidePanelSplit;
        private VisualListDetails cTracks;
        private VisualListDetails cSessions;
        private VisualListDetails cLaps;
        private Plotter cPlotter;
        private Button cButtonChooseLap;
        private TrackMap cTrackMap;
        private Button cButtonRepaint;
        private Button button1;
    }
}

