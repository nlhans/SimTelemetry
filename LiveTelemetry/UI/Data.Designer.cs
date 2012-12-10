namespace LiveTelemetry.UI
{
    partial class Data
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
            this.labels = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // labels
            // 
            this.labels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labels.Location = new System.Drawing.Point(0, 0);
            this.labels.Name = "labels";
            this.labels.Size = new System.Drawing.Size(1226, 634);
            this.labels.TabIndex = 0;
            // 
            // Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 634);
            this.Controls.Add(this.labels);
            this.Name = "Data";
            this.Text = "Data";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel labels;

    }
}