namespace ModernSSClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            ConnectBtn = new Button();
            VPNKeyTB = new TextBox();
            StatusPnl = new Panel();
            IPLabel = new Label();
            DisconnectBtn = new Button();
            EKeyLbl = new Label();
            EnterKBtn = new Button();
            notifyIcon1 = new NotifyIcon(components);
            SuspendLayout();
            // 
            // ConnectBtn
            // 
            ConnectBtn.BackColor = Color.Green;
            ConnectBtn.Cursor = Cursors.Hand;
            ConnectBtn.FlatAppearance.BorderSize = 0;
            ConnectBtn.Font = new Font("Microsoft YaHei UI", 13F);
            ConnectBtn.ForeColor = Color.WhiteSmoke;
            ConnectBtn.Location = new Point(0, 533);
            ConnectBtn.Name = "ConnectBtn";
            ConnectBtn.Size = new Size(410, 56);
            ConnectBtn.TabIndex = 0;
            ConnectBtn.Text = "Connect";
            ConnectBtn.UseVisualStyleBackColor = false;
            ConnectBtn.Click += ConnectBtn_Click;
            // 
            // VPNKeyTB
            // 
            VPNKeyTB.Location = new Point(16, 293);
            VPNKeyTB.Name = "VPNKeyTB";
            VPNKeyTB.Size = new Size(285, 23);
            VPNKeyTB.TabIndex = 1;
            // 
            // StatusPnl
            // 
            StatusPnl.BackColor = Color.Red;
            StatusPnl.Location = new Point(0, -1);
            StatusPnl.Name = "StatusPnl";
            StatusPnl.Size = new Size(410, 29);
            StatusPnl.TabIndex = 2;
            // 
            // IPLabel
            // 
            IPLabel.AutoSize = true;
            IPLabel.Font = new Font("Lucida Console", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            IPLabel.Location = new Point(12, 495);
            IPLabel.Name = "IPLabel";
            IPLabel.Size = new Size(130, 19);
            IPLabel.TabIndex = 3;
            IPLabel.Text = "IP: 0.0.0.0";
            // 
            // DisconnectBtn
            // 
            DisconnectBtn.BackColor = Color.Red;
            DisconnectBtn.Cursor = Cursors.Hand;
            DisconnectBtn.FlatAppearance.BorderSize = 0;
            DisconnectBtn.Font = new Font("Microsoft YaHei UI", 13F);
            DisconnectBtn.ForeColor = Color.WhiteSmoke;
            DisconnectBtn.Location = new Point(0, 533);
            DisconnectBtn.Name = "DisconnectBtn";
            DisconnectBtn.Size = new Size(410, 56);
            DisconnectBtn.TabIndex = 4;
            DisconnectBtn.Text = "Disconnect";
            DisconnectBtn.UseVisualStyleBackColor = false;
            DisconnectBtn.Click += DisconnectBtn_Click;
            // 
            // EKeyLbl
            // 
            EKeyLbl.AutoSize = true;
            EKeyLbl.Font = new Font("Lucida Console", 11.25F);
            EKeyLbl.Location = new Point(16, 275);
            EKeyLbl.Name = "EKeyLbl";
            EKeyLbl.Size = new Size(97, 15);
            EKeyLbl.TabIndex = 5;
            EKeyLbl.Text = "Enter key:";
            // 
            // EnterKBtn
            // 
            EnterKBtn.Location = new Point(307, 293);
            EnterKBtn.Name = "EnterKBtn";
            EnterKBtn.Size = new Size(88, 23);
            EnterKBtn.TabIndex = 6;
            EnterKBtn.Text = "Enter";
            EnterKBtn.UseVisualStyleBackColor = true;
            EnterKBtn.Click += EnterKBtn_Click;
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "FWSS VPN";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(410, 589);
            Controls.Add(EnterKBtn);
            Controls.Add(EKeyLbl);
            Controls.Add(DisconnectBtn);
            Controls.Add(IPLabel);
            Controls.Add(StatusPnl);
            Controls.Add(VPNKeyTB);
            Controls.Add(ConnectBtn);
            Name = "Form1";
            Text = "VPN";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ConnectBtn;
        private TextBox VPNKeyTB;
        private Panel StatusPnl;
        private Label IPLabel;
        private Button DisconnectBtn;
        private Label EKeyLbl;
        private Button EnterKBtn;
        private NotifyIcon notifyIcon1;
    }
}
