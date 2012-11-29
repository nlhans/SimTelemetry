namespace LiveTelemetry.UI
{
    partial class fNetwork
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
            this.gp_Server = new System.Windows.Forms.GroupBox();
            this.lbl_Server_BandwidthSetting = new System.Windows.Forms.Label();
            this.tb_Server_Bandwidth = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_Server_Clients = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Server_Port = new System.Windows.Forms.TextBox();
            this.bt_Server_StartStop = new System.Windows.Forms.Button();
            this.gb_Client = new System.Windows.Forms.GroupBox();
            this.tb_Client_Port = new System.Windows.Forms.TextBox();
            this.lbl_Client_IP = new System.Windows.Forms.Label();
            this.tb_Client_IP = new System.Windows.Forms.TextBox();
            this.lbl_Client_Port = new System.Windows.Forms.Label();
            this.bt_Client_StartStop = new System.Windows.Forms.Button();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.gp_Server.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Server_Bandwidth)).BeginInit();
            this.gb_Client.SuspendLayout();
            this.SuspendLayout();
            // 
            // gp_Server
            // 
            this.gp_Server.Controls.Add(this.lbl_Server_BandwidthSetting);
            this.gp_Server.Controls.Add(this.tb_Server_Bandwidth);
            this.gp_Server.Controls.Add(this.label4);
            this.gp_Server.Controls.Add(this.label3);
            this.gp_Server.Controls.Add(this.tb_Server_Clients);
            this.gp_Server.Controls.Add(this.label2);
            this.gp_Server.Controls.Add(this.tb_Server_Port);
            this.gp_Server.Controls.Add(this.bt_Server_StartStop);
            this.gp_Server.Location = new System.Drawing.Point(12, 12);
            this.gp_Server.Name = "gp_Server";
            this.gp_Server.Size = new System.Drawing.Size(213, 176);
            this.gp_Server.TabIndex = 0;
            this.gp_Server.TabStop = false;
            this.gp_Server.Text = "Server";
            // 
            // lbl_Server_BandwidthSetting
            // 
            this.lbl_Server_BandwidthSetting.AutoSize = true;
            this.lbl_Server_BandwidthSetting.Location = new System.Drawing.Point(6, 97);
            this.lbl_Server_BandwidthSetting.Name = "lbl_Server_BandwidthSetting";
            this.lbl_Server_BandwidthSetting.Size = new System.Drawing.Size(26, 13);
            this.lbl_Server_BandwidthSetting.TabIndex = 10;
            this.lbl_Server_BandwidthSetting.Text = "1Hz";
            // 
            // tb_Server_Bandwidth
            // 
            this.tb_Server_Bandwidth.LargeChange = 10;
            this.tb_Server_Bandwidth.Location = new System.Drawing.Point(87, 65);
            this.tb_Server_Bandwidth.Maximum = 50;
            this.tb_Server_Bandwidth.Minimum = 1;
            this.tb_Server_Bandwidth.Name = "tb_Server_Bandwidth";
            this.tb_Server_Bandwidth.Size = new System.Drawing.Size(116, 45);
            this.tb_Server_Bandwidth.TabIndex = 9;
            this.tb_Server_Bandwidth.TickFrequency = 5;
            this.tb_Server_Bandwidth.Value = 1;
            this.tb_Server_Bandwidth.ValueChanged += new System.EventHandler(this.tb_Server_Bandwidth_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Bandwidth:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Max. clients:";
            // 
            // tb_Server_Clients
            // 
            this.tb_Server_Clients.Location = new System.Drawing.Point(87, 39);
            this.tb_Server_Clients.Name = "tb_Server_Clients";
            this.tb_Server_Clients.Size = new System.Drawing.Size(116, 20);
            this.tb_Server_Clients.TabIndex = 6;
            this.tb_Server_Clients.Text = "5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Server Port:";
            // 
            // tb_Server_Port
            // 
            this.tb_Server_Port.Location = new System.Drawing.Point(87, 13);
            this.tb_Server_Port.Name = "tb_Server_Port";
            this.tb_Server_Port.Size = new System.Drawing.Size(116, 20);
            this.tb_Server_Port.TabIndex = 3;
            this.tb_Server_Port.Text = "12345";
            // 
            // bt_Server_StartStop
            // 
            this.bt_Server_StartStop.Location = new System.Drawing.Point(87, 132);
            this.bt_Server_StartStop.Name = "bt_Server_StartStop";
            this.bt_Server_StartStop.Size = new System.Drawing.Size(75, 23);
            this.bt_Server_StartStop.TabIndex = 1;
            this.bt_Server_StartStop.Text = "Start";
            this.bt_Server_StartStop.UseVisualStyleBackColor = true;
            this.bt_Server_StartStop.Click += new System.EventHandler(this.bt_Server_StartStop_Click);
            // 
            // gb_Client
            // 
            this.gb_Client.Controls.Add(this.tb_Client_Port);
            this.gb_Client.Controls.Add(this.lbl_Client_IP);
            this.gb_Client.Controls.Add(this.tb_Client_IP);
            this.gb_Client.Controls.Add(this.lbl_Client_Port);
            this.gb_Client.Controls.Add(this.bt_Client_StartStop);
            this.gb_Client.Location = new System.Drawing.Point(231, 12);
            this.gb_Client.Name = "gb_Client";
            this.gb_Client.Size = new System.Drawing.Size(232, 176);
            this.gb_Client.TabIndex = 1;
            this.gb_Client.TabStop = false;
            this.gb_Client.Text = "Client";
            // 
            // tb_Client_Port
            // 
            this.tb_Client_Port.Location = new System.Drawing.Point(87, 39);
            this.tb_Client_Port.Name = "tb_Client_Port";
            this.tb_Client_Port.Size = new System.Drawing.Size(136, 20);
            this.tb_Client_Port.TabIndex = 11;
            this.tb_Client_Port.Text = "12345";
            // 
            // lbl_Client_IP
            // 
            this.lbl_Client_IP.AutoSize = true;
            this.lbl_Client_IP.Location = new System.Drawing.Point(6, 16);
            this.lbl_Client_IP.Name = "lbl_Client_IP";
            this.lbl_Client_IP.Size = new System.Drawing.Size(60, 13);
            this.lbl_Client_IP.TabIndex = 12;
            this.lbl_Client_IP.Text = "Remote IP:";
            // 
            // tb_Client_IP
            // 
            this.tb_Client_IP.Location = new System.Drawing.Point(87, 13);
            this.tb_Client_IP.Name = "tb_Client_IP";
            this.tb_Client_IP.Size = new System.Drawing.Size(136, 20);
            this.tb_Client_IP.TabIndex = 10;
            // 
            // lbl_Client_Port
            // 
            this.lbl_Client_Port.AutoSize = true;
            this.lbl_Client_Port.Location = new System.Drawing.Point(6, 42);
            this.lbl_Client_Port.Name = "lbl_Client_Port";
            this.lbl_Client_Port.Size = new System.Drawing.Size(63, 13);
            this.lbl_Client_Port.TabIndex = 11;
            this.lbl_Client_Port.Text = "Server Port:";
            // 
            // bt_Client_StartStop
            // 
            this.bt_Client_StartStop.Location = new System.Drawing.Point(87, 132);
            this.bt_Client_StartStop.Name = "bt_Client_StartStop";
            this.bt_Client_StartStop.Size = new System.Drawing.Size(75, 23);
            this.bt_Client_StartStop.TabIndex = 10;
            this.bt_Client_StartStop.Text = "Start";
            this.bt_Client_StartStop.UseVisualStyleBackColor = true;
            this.bt_Client_StartStop.Click += new System.EventHandler(this.bt_Client_StartStop_Click);
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Location = new System.Drawing.Point(12, 191);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(66, 13);
            this.lbl_Status.TabIndex = 2;
            this.lbl_Status.Text = "Offline mode";
            // 
            // fNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 213);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.gb_Client);
            this.Controls.Add(this.gp_Server);
            this.Name = "fNetwork";
            this.Text = "Network";
            this.gp_Server.ResumeLayout(false);
            this.gp_Server.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Server_Bandwidth)).EndInit();
            this.gb_Client.ResumeLayout(false);
            this.gb_Client.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gp_Server;
        private System.Windows.Forms.GroupBox gb_Client;
        private System.Windows.Forms.TrackBar tb_Server_Bandwidth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_Server_Clients;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Server_Port;
        private System.Windows.Forms.Button bt_Server_StartStop;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Label lbl_Client_Port;
        private System.Windows.Forms.Button bt_Client_StartStop;
        private System.Windows.Forms.TextBox tb_Client_Port;
        private System.Windows.Forms.Label lbl_Client_IP;
        private System.Windows.Forms.TextBox tb_Client_IP;
        private System.Windows.Forms.Label lbl_Server_BandwidthSetting;
    }
}