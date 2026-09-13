using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvaloniaClient.Views;

public partial class MainWindow : Window
{
    private Process? VPNProcess;

    private string VPNKey = "";
    private string VPNMethod = "";
    private string VPNPassword = "";
    private string VPNIPAddr = "";
    private string VPNPort = "";

    public MainWindow()
    {
        InitializeComponent();

        using (var key = Registry.CurrentUser.OpenSubKey(@"Software\FWSSVPN"))
        {
            if (key != null)
            {
                string? saved = key.GetValue("SavedKey") as string;
                if (!string.IsNullOrEmpty(saved))
                {
                    VPNKeyTB.Text = saved;
                }
            }
        }

        _ = LoadInitialIPAsync();
    }

    private async Task LoadInitialIPAsync()
    {
        string currentIp = await GetPublicIPAddressAsync();
        IPLabel.Text = "IP: " + currentIp;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (VPNProcess != null && !VPNProcess.HasExited)
        {
            try { VPNProcess.Kill(true); } catch { }
            VPNProcess = null;
        }

        SetSystemProxy(false);
        base.OnClosing(e);
    }

    private void EnterKBtn_Click(object? sender, RoutedEventArgs e)
    {
        VPNKey = VPNKeyTB.Text?.Trim() ?? "";
        ParseShadowsocks(VPNKey);

        if (!string.IsNullOrEmpty(VPNIPAddr) && !string.IsNullOrEmpty(VPNPassword))
        {
            using var key = Registry.CurrentUser.CreateSubKey(@"Software\FWSSVPN");
            key?.SetValue("SavedKey", VPNKey);
        }
    }

    private async void ConnectBtn_Click(object? sender, RoutedEventArgs e)
    {
        ConnectBtn.IsEnabled = false;

        try
        {
            if (string.IsNullOrEmpty(VPNIPAddr))
            {
                ParseShadowsocks(VPNKeyTB.Text?.Trim() ?? "");
            }

            if (string.IsNullOrEmpty(VPNIPAddr) || string.IsNullOrEmpty(VPNPassword))
            {
                return;
            }

            if (VPNProcess != null && !VPNProcess.HasExited)
            {
                try { VPNProcess.Kill(true); } catch { }
                VPNProcess = null;
            }

            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\FWSSVPN"))
            {
                key?.SetValue("SavedKey", VPNKeyTB.Text?.Trim() ?? "");
            }

            VPNProcess = Process.Start(new ProcessStartInfo
            {
                FileName = System.IO.Path.Combine(AppContext.BaseDirectory, "sslocal.exe"),
                Arguments = $"-s \"{VPNIPAddr}:{VPNPort}\" -m \"{VPNMethod}\" -k \"{VPNPassword}\" -b \"127.0.0.1:1080\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });

            SetSystemProxy(true);

            StatusPnl.Background = new SolidColorBrush(Color.Parse("#22c55e"));
            IPLabel.Text = $"IP: {VPNIPAddr}";

            ConnectBtn.IsVisible = false;
            DisconnectBtn.IsVisible = true;
        }
        catch (Exception ex)
        {
            IPLabel.Text = "Error starting sslocal.exe";
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            ConnectBtn.IsEnabled = true;
        }
    }

    private async void DisconnectBtn_Click(object? sender, RoutedEventArgs e)
    {
        DisconnectBtn.IsEnabled = false;

        try
        {
            if (VPNProcess != null && !VPNProcess.HasExited)
            {
                try { VPNProcess.Kill(true); } catch { }
                VPNProcess = null;
            }

            SetSystemProxy(false);

            StatusPnl.Background = new SolidColorBrush(Color.Parse("#ef4444"));
            IPLabel.Text = "IP: Checking...";

            ConnectBtn.IsVisible = true;
            DisconnectBtn.IsVisible = false;

            IPLabel.Text = "IP: " + await GetPublicIPAddressAsync();
        }
        finally
        {
            DisconnectBtn.IsEnabled = true;
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
            IPLabel.Text = "IP/Port not found";
            return "";
        }

        var match = Regex.Match(base64SS, @"//(.*?)@");
        if (!match.Success)
        {
            IPLabel.Text = "Key not found";
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
            IPLabel.Text = "Base64 decode error";
        }

        return decodedText;
    }

    public static async Task<string> GetPublicIPAddressAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            return (await client.GetStringAsync("https://api.ipify.org")).Trim();
        }
        catch
        {
            return "Unknown";
        }
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
}