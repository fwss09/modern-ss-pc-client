using ModernSSClient;

namespace WinForms
{
    internal static class Program
    {
        private static Mutex mutex = new Mutex(true, "{ModernSSClient-SingleInstance-Mutex}");

        [STAThread]
        static async Task Main()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                return;
            }

            try
            {
                ApplicationConfiguration.Initialize();
                string ip = await GetPublicIPAddressAsync();
                Application.Run(new Form1(ip));
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public static async Task<string> GetPublicIPAddressAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(3);
                    string url = "https://api.ipquery.io";
                    var response = await client.GetStringAsync(url);
                    return response.Trim();
                }
            }
            catch
            {
                return "Offline";
            }
        }
    }
}