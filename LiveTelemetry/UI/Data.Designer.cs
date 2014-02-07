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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabEnv = new System.Windows.Forms.TabPage();
            this.tabSession = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabEnv);
            this.tabs.Controls.Add(this.tabSession);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(644, 471);
            this.tabs.TabIndex = 0;
            // 
            // tabEnv
            // 
            this.tabEnv.Location = new System.Drawing.Point(4, 22);
            this.tabEnv.Name = "tabEnv";
            this.tabEnv.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnv.Size = new System.Drawing.Size(636, 445);
            this.tabEnv.TabIndex = 0;
            this.tabEnv.Text = "Simulator & Plug-in";
            this.tabEnv.UseVisualStyleBackColor = true;
            // 
            // tabSession
            // 
            this.tabSession.Location = new System.Drawing.Point(4, 22);
            this.tabSession.Name = "tabSession";
            this.tabSession.Padding = new System.Windows.Forms.Padding(3);
            this.tabSession.Size = new System.Drawing.Size(636, 445);
            this.tabSession.TabIndex = 1;
            this.tabSession.Text = "Session";
            this.tabSession.UseVisualStyleBackColor = true;
            // 
            // Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 495);
            this.Controls.Add(this.tabs);
            this.Name = "Data";
            this.Text = "Data";
            this.tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabEnv;
        private System.Windows.Forms.TabPage tabSession;



    }
}