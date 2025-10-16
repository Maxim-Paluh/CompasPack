using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public static class NetworkService
    {
        public static async Task<double> SpeedTest(string uRL)
        {
            try
            {
                var watch = new Stopwatch();

                watch.Start();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uRL);
                request.Timeout = 10000;
                WebResponse response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        request.Timeout = 10000;
                        await reader.ReadToEndAsync();
                    }
                }

                watch.Stop();
                return 10485760 / 1024 / 1024 / watch.Elapsed.TotalSeconds;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static void OpenUrl(string uRL)
        {
            try
            {
                Process.Start(uRL);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start(new ProcessStartInfo(uRL.Replace("&", "^&")) { UseShellExecute = true });
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", uRL);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", uRL);
                else
                    throw;
            }
        }
    }

}
