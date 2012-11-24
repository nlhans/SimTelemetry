namespace LiveTelemetry
{
    partial class Error
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
            this.lb_header = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_close = new System.Windows.Forms.Button();
            this.lb_sorry = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_header
            // 
            this.lb_header.AutoSize = true;
            this.lb_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_header.Location = new System.Drawing.Point(12, 9);
            this.lb_header.Name = "lb_header";
            this.lb_header.Size = new System.Drawing.Size(255, 25);
            this.lb_header.TabIndex = 0;
            this.lb_header.Text = "Fatal Application Error!";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(483, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Something inside the SimTelemetry application went wrong. Please send a mail on o" +
                "ur website with a ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(462, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "description of what you were doing, which simulator you were driving and the debu" +
                "g.txt log trace.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(483, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "The debug.txt file will contan backtrace information during the crash for the dev" +
                "elopers to investigate.";
            // 
            // bt_close
            // 
            this.bt_close.Location = new System.Drawing.Point(182, 103);
            this.bt_close.Name = "bt_close";
            this.bt_close.Size = new System.Drawing.Size(157, 23);
            this.bt_close.TabIndex = 4;
            this.bt_close.Text = "Close";
            this.bt_close.UseVisualStyleBackColor = true;
            this.bt_close.Click += new System.EventHandler(this.bt_close_Click);
            // 
            // lb_sorry
            // 
            this.lb_sorry.AutoSize = true;
            this.lb_sorry.Location = new System.Drawing.Point(15, 88);
            this.lb_sorry.Name = "lb_sorry";
            this.lb_sorry.Size = new System.Drawing.Size(207, 13);
            this.lb_sorry.TabIndex = 5;
            this.lb_sorry.Text = "We\'re sorry for the inconvenience caused.";
            // 
            // Error
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 140);
            this.Controls.Add(this.lb_sorry);
            this.Controls.Add(this.bt_close);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lb_header);
            this.Name = "Error";
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_header;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bt_close;
        private System.Windows.Forms.Label lb_sorry;
    }
}