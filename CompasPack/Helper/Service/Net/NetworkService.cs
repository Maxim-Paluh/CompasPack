using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
                byte[] buffer = new byte[8192];
                long totalReceivedBytes = 0;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uRL);
                request.Timeout = 10000;

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        watch.Start();

                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            totalReceivedBytes += bytesRead;
                        }

                        watch.Stop();
                    }
                }

                double seconds = watch.Elapsed.TotalSeconds;
                if (seconds > 0)
                {
                    return (totalReceivedBytes * 8) / (seconds * 1000000);
                }
                else { return 0; }

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
