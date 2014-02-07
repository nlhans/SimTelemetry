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
            this.tabPlayer1 = new System.Windows.Forms.TabPage();
            this.tabPlayer2 = new System.Windows.Forms.TabPage();
            this.tabPlayer3 = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabEnv);
            this.tabs.Controls.Add(this.tabSession);
            this.tabs.Controls.Add(this.tabPlayer1);
            this.tabs.Controls.Add(this.tabPlayer2);
            this.tabs.Controls.Add(this.tabPlayer3);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(960, 538);
            this.tabs.TabIndex = 0;
            // 
            // tabEnv
            // 
            this.tabEnv.Location = new System.Drawing.Point(4, 22);
            this.tabEnv.Name = "tabEnv";
            this.tabEnv.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnv.Size = new System.Drawing.Size(952, 512);
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
            // tabPlayer1
            // 
            this.tabPlayer1.Location = new System.Drawing.Point(4, 22);
            this.tabPlayer1.Name = "tabPlayer1";
            this.tabPlayer1.Size = new System.Drawing.Size(636, 445);
            this.tabPlayer1.TabIndex = 2;
            this.tabPlayer1.Text = "Player [Car]";
            this.tabPlayer1.UseVisualStyleBackColor = true;
            // 
            // tabPlayer2
            // 
            this.tabPlayer2.Location = new System.Drawing.Point(4, 22);
            this.tabPlayer2.Name = "tabPlayer2";
            this.tabPlayer2.Size = new System.Drawing.Size(636, 445);
            this.tabPlayer2.TabIndex = 3;
            this.tabPlayer2.Text = "Player [General]";
            this.tabPlayer2.UseVisualStyleBackColor = true;
            // 
            // tabPlayer3
            // 
            this.tabPlayer3.Location = new System.Drawing.Point(4, 22);
            this.tabPlayer3.Name = "tabPlayer3";
            this.tabPlayer3.Size = new System.Drawing.Size(636, 445);
            this.tabPlayer3.TabIndex = 4;
            this.tabPlayer3.Text = "Player [Wheels]";
            this.tabPlayer3.UseVisualStyleBackColor = true;
            // 
            // Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 562);
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
        private System.Windows.Forms.TabPage tabPlayer1;
        private System.Windows.Forms.TabPage tabPlayer2;
        private System.Windows.Forms.TabPage tabPlayer3;



    }
}