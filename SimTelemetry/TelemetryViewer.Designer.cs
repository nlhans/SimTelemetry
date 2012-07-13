namespace SimTelemetry
{
    partial class TelemetryViewer
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.GraphSplit = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btOpen = new System.Windows.Forms.ToolStripButton();
            this.lbLoadingbar = new System.Windows.Forms.ToolStripProgressBar();
            this.lbLoading = new System.Windows.Forms.ToolStripLabel();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphSplit)).BeginInit();
            this.GraphSplit.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.GraphSplit);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1181, 669);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1181, 694);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // GraphSplit
            // 
            this.GraphSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GraphSplit.Location = new System.Drawing.Point(0, 0);
            this.GraphSplit.Name = "GraphSplit";
            this.GraphSplit.Size = new System.Drawing.Size(1181, 669);
            this.GraphSplit.SplitterDistance = 393;
            this.GraphSplit.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btOpen,
            this.lbLoading,
            this.lbLoadingbar});
            this.toolStrip1.Location = new System.Drawing.Point(4, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(221, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btOpen
            // 
            this.btOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btOpen.Image = global::SimTelemetry.Properties.Resources.IconOpen;
            this.btOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(23, 22);
            this.btOpen.Text = "Open log file";
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // lbLoadingbar
            // 
            this.lbLoadingbar.Maximum = 1000;
            this.lbLoadingbar.Name = "lbLoadingbar";
            this.lbLoadingbar.Size = new System.Drawing.Size(100, 22);
            this.lbLoadingbar.Visible = false;
            // 
            // lbLoading
            // 
            this.lbLoading.Name = "lbLoading";
            this.lbLoading.Size = new System.Drawing.Size(53, 22);
            this.lbLoading.Text = "Loading:";
            this.lbLoading.Visible = false;
            // 
            // TelemetryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 694);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "TelemetryViewer";
            this.Text = "Telemetry Viewer";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GraphSplit)).EndInit();
            this.GraphSplit.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.SplitContainer GraphSplit;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btOpen;
        private System.Windows.Forms.ToolStripLabel lbLoading;
        private System.Windows.Forms.ToolStripProgressBar lbLoadingbar;
    }
}

