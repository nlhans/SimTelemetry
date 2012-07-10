namespace SimTelemetry
{
    partial class Plotter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chart = new System.Windows.Forms.PictureBox();
            this.timeline = new System.Windows.Forms.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(850, 366);
            this.chart.TabIndex = 0;
            this.chart.TabStop = false;
            // 
            // timeline
            // 
            this.timeline.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.timeline.LargeChange = 5;
            this.timeline.Location = new System.Drawing.Point(0, 349);
            this.timeline.Name = "timeline";
            this.timeline.Size = new System.Drawing.Size(850, 17);
            this.timeline.TabIndex = 1;
            // 
            // Plotter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.timeline);
            this.Controls.Add(this.chart);
            this.Name = "Plotter";
            this.Size = new System.Drawing.Size(850, 366);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox chart;
        public System.Windows.Forms.HScrollBar timeline;
    }
}
