namespace ModernSSClient
{
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public partial class Form1 : Form
    {
        private Process VPNProcess;

        string VPNKey = "";
        string VPNMethod = "";
        string VPNPassword = "";
        string VPNIPAddr = "";
        string VPNPort = "";

        public Form1(string ip)
        {
            InitializeComponent();
            IPLabel.Text = "IP: " + ip;

            ConnectBtn.Visible = true;
            DisconnectBtn.Visible = false;

            notifyIcon1.Icon = this.Icon ?? SystemIcons.Application;
            notifyIcon1.Icon = SystemIcons.Application;
            notifyIcon1.Text = "VPN";

            var contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("Выход");

            exitItem.Click += ExitApp_Click;

            contextMenu.Items.Add(exitItem);
            notifyIcon1.ContextMenuStrip = contextMenu;

            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\FWSSVPN"))
            {
                if (key != null)
                {
                    string saved = key.GetValue("SavedKey") as string;
                    if (!string.IsNullOrEmpty(saved))
                    {
                        VPNKeyTB.Text = saved;
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Hide();

                notifyIcon1.Visible = true;
                return;
            }
            if (VPNProcess != null && !VPNProcess.HasExited)
            {
                VPNProcess.Kill();
            }

            SetSystemProxy(false);

            base.OnFormClosing(e);
        }

        private async void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(VPNIPAddr))
            {
                ParseShadowsocks(VPNKeyTB.Text);
            }
            if (string.IsNullOrEmpty(VPNIPAddr) || string.IsNullOrEmpty(VPNPassword))
            {
                return;
            }

            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\FWSSVPN"))
            {
                key?.SetValue("SavedKey", VPNKeyTB.Text.Trim());
            }

            VPNProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "sslocal.exe",
                Arguments = $"-s \"{VPNIPAddr}:{VPNPort}\" -m \"{VPNMethod}\" -k \"{VPNPassword}\" -b \"127.0.0.1:1080\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });

            SetSystemProxy(true);

            StatusPnl.BackColor = Color.Green;
            IPLabel.Text = $"IP: {VPNIPAddr}";

            ConnectBtn.Visible = false;
            DisconnectBtn.Visible = true;
        }

        private async void DisconnectBtn_Click(object sender, EventArgs e)
        {
            if (VPNProcess != null && !VPNProcess.HasExited)
            {
                VPNProcess.Kill();
                VPNProcess = null;
            }

            SetSystemProxy(false);

            StatusPnl.BackColor = Color.Red;

            IPLabel.Text = "IP: " + await Program.GetPublicIPAddressAsync();

            ConnectBtn.Visible = true;
            DisconnectBtn.Visible = false;
        }

        private void EnterKBtn_Click(object sender, EventArgs e)
        {
            VPNKey = VPNKeyTB.Text;
            string decodedVPNKey = ParseShadowsocks(VPNKey);

            if (!string.IsNullOrEmpty(VPNIPAddr) && !string.IsNullOrEmpty(VPNPassword))
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\FWSSVPN"))
                {
                    key?.SetValue("SavedKey", VPNKey.Trim());
                }
            }
        }

        private string ParseShadowsocks(string base64SS)
        {
            var ipPortMatch = Regex.Match(base64SS, @"@(?<ip>[^:]+):(?<port>\d+)");
            if (ipPortMatch.Success)
            {
                VPNIPAddr = ipPortMatch.Groups["ip"].Value;
                VPNPort = ipPortMatch.Groups["port"].Value;
            }
            else
            {
                MessageBox.Show("ip not found");
                return "";
            }

            var match = Regex.Match(base64SS, @"//(.*?)@");
            if (!match.Success)
            {
                MessageBox.Show("key not found");
                return "";
            }

            string base64Part = match.Groups[1].Value;

            while (base64Part.Length % 4 != 0)
            {
                base64Part += "=";
            }

            string decodedText = "";
            try
            {
                byte[] byteData = Convert.FromBase64String(base64Part);
                decodedText = Encoding.UTF8.GetString(byteData);

                string[] parts = decodedText.Split(':', 2);
                if (parts.Length == 2)
                {
                    VPNMethod = parts[0];
                    VPNPassword = parts[1];
                }
            }
            catch
            {
                MessageBox.Show("error while decoding base64");
            }

            return decodedText;
        }




        private void ExitApp_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;

            Application.Exit();
        }




        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private void SetSystemProxy(bool enable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true))
            {
                if (key == null) return;

                if (enable)
                {
                    key.SetValue("ProxyEnable", 1);
                    key.SetValue("ProxyServer", "socks=127.0.0.1:1080");
                }
                else
                {
                    key.SetValue("ProxyEnable", 0);
                }
            }

            InternetSetOption(IntPtr.Zero, 39, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, 37, IntPtr.Zero, 0);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();

            notifyIcon1.Visible = false;
        }
    }
}
