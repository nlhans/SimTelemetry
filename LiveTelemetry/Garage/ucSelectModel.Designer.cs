namespace LiveTelemetry.Garage
{
    partial class ucSelectModel
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbl_Team = new System.Windows.Forms.Label();
            this.lbl_info1 = new System.Windows.Forms.Label();
            this.lbl_info2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lbl_info2);
            this.splitContainer1.Panel2.Controls.Add(this.lbl_info1);
            this.splitContainer1.Panel2.Controls.Add(this.lbl_Team);
            this.splitContainer1.Size = new System.Drawing.Size(581, 424);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 0;
            // 
            // lbl_Team
            // 
            this.lbl_Team.AutoSize = true;
            this.lbl_Team.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Team.ForeColor = System.Drawing.Color.White;
            this.lbl_Team.Location = new System.Drawing.Point(12, 15);
            this.lbl_Team.Name = "lbl_Team";
            this.lbl_Team.Size = new System.Drawing.Size(70, 25);
            this.lbl_Team.TabIndex = 0;
            this.lbl_Team.Text = "Team";
            // 
            // lbl_info1
            // 
            this.lbl_info1.AutoSize = true;
            this.lbl_info1.ForeColor = System.Drawing.Color.White;
            this.lbl_info1.Location = new System.Drawing.Point(12, 48);
            this.lbl_info1.Name = "lbl_info1";
            this.lbl_info1.Size = new System.Drawing.Size(34, 13);
            this.lbl_info1.TabIndex = 1;
            this.lbl_info1.Text = "Info 1";
            // 
            // lbl_info2
            // 
            this.lbl_info2.AutoSize = true;
            this.lbl_info2.ForeColor = System.Drawing.Color.White;
            this.lbl_info2.Location = new System.Drawing.Point(211, 48);
            this.lbl_info2.Name = "lbl_info2";
            this.lbl_info2.Size = new System.Drawing.Size(34, 13);
            this.lbl_info2.TabIndex = 2;
            this.lbl_info2.Text = "Info 2";
            // 
            // ucSelectModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ucSelectModel";
            this.Size = new System.Drawing.Size(581, 424);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lbl_Team;
        private System.Windows.Forms.Label lbl_info2;
        private System.Windows.Forms.Label lbl_info1;
    }
}
