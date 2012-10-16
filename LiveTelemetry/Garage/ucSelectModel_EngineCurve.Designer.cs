namespace LiveTelemetry.Garage
{
    partial class ucSelectModel_EngineCurve
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_throttle = new System.Windows.Forms.Label();
            this.tb_throttle = new System.Windows.Forms.TrackBar();
            this.lbl_speed = new System.Windows.Forms.Label();
            this.tb_speed = new System.Windows.Forms.TextBox();
            this.lbl_mode = new System.Windows.Forms.Label();
            this.cb_mode = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_throttle)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbl_throttle);
            this.panel1.Controls.Add(this.tb_throttle);
            this.panel1.Controls.Add(this.lbl_speed);
            this.panel1.Controls.Add(this.tb_speed);
            this.panel1.Controls.Add(this.lbl_mode);
            this.panel1.Controls.Add(this.cb_mode);
            this.panel1.Location = new System.Drawing.Point(0, 450);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 40);
            this.panel1.TabIndex = 0;
            // 
            // lbl_throttle
            // 
            this.lbl_throttle.AutoSize = true;
            this.lbl_throttle.ForeColor = System.Drawing.Color.White;
            this.lbl_throttle.Location = new System.Drawing.Point(299, 9);
            this.lbl_throttle.Name = "lbl_throttle";
            this.lbl_throttle.Size = new System.Drawing.Size(78, 13);
            this.lbl_throttle.TabIndex = 11;
            this.lbl_throttle.Text = "Throttle (100%)";
            // 
            // tb_throttle
            // 
            this.tb_throttle.LargeChange = 10;
            this.tb_throttle.Location = new System.Drawing.Point(383, 0);
            this.tb_throttle.Maximum = 100;
            this.tb_throttle.Name = "tb_throttle";
            this.tb_throttle.Size = new System.Drawing.Size(214, 45);
            this.tb_throttle.SmallChange = 5;
            this.tb_throttle.TabIndex = 10;
            this.tb_throttle.TickFrequency = 10;
            this.tb_throttle.ValueChanged += new System.EventHandler(this.tb_throttle_ValueChanged);
            // 
            // lbl_speed
            // 
            this.lbl_speed.AutoSize = true;
            this.lbl_speed.ForeColor = System.Drawing.Color.White;
            this.lbl_speed.Location = new System.Drawing.Point(194, 9);
            this.lbl_speed.Name = "lbl_speed";
            this.lbl_speed.Size = new System.Drawing.Size(41, 13);
            this.lbl_speed.TabIndex = 9;
            this.lbl_speed.Text = "Speed:";
            // 
            // tb_speed
            // 
            this.tb_speed.Location = new System.Drawing.Point(241, 7);
            this.tb_speed.Name = "tb_speed";
            this.tb_speed.Size = new System.Drawing.Size(52, 20);
            this.tb_speed.TabIndex = 8;
            this.tb_speed.TextChanged += new System.EventHandler(this.tb_speed_TextChanged);
            // 
            // lbl_mode
            // 
            this.lbl_mode.AutoSize = true;
            this.lbl_mode.ForeColor = System.Drawing.Color.White;
            this.lbl_mode.Location = new System.Drawing.Point(3, 9);
            this.lbl_mode.Name = "lbl_mode";
            this.lbl_mode.Size = new System.Drawing.Size(73, 13);
            this.lbl_mode.TabIndex = 7;
            this.lbl_mode.Text = "Engine Mode:";
            // 
            // cb_mode
            // 
            this.cb_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_mode.FormattingEnabled = true;
            this.cb_mode.Location = new System.Drawing.Point(82, 7);
            this.cb_mode.Name = "cb_mode";
            this.cb_mode.Size = new System.Drawing.Size(106, 21);
            this.cb_mode.TabIndex = 6;
            this.cb_mode.SelectedValueChanged += new System.EventHandler(this.cb_mode_SelectedValueChanged);
            // 
            // ucSelectModel_EngineCurve
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ucSelectModel_EngineCurve";
            this.Size = new System.Drawing.Size(687, 489);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_throttle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_throttle;
        private System.Windows.Forms.TrackBar tb_throttle;
        private System.Windows.Forms.Label lbl_speed;
        private System.Windows.Forms.TextBox tb_speed;
        private System.Windows.Forms.Label lbl_mode;
        private System.Windows.Forms.ComboBox cb_mode;
    }
}
