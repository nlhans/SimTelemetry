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
            this.GraphSplit = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.GraphSplit)).BeginInit();
            this.GraphSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // GraphSplit
            // 
            this.GraphSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GraphSplit.Location = new System.Drawing.Point(0, 0);
            this.GraphSplit.Name = "GraphSplit";
            this.GraphSplit.Size = new System.Drawing.Size(1181, 694);
            this.GraphSplit.SplitterDistance = 393;
            this.GraphSplit.TabIndex = 0;
            // 
            // TelemetryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 694);
            this.Controls.Add(this.GraphSplit);
            this.Name = "TelemetryViewer";
            this.Text = "Telemetry Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.GraphSplit)).EndInit();
            this.GraphSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer GraphSplit;
    }
}

