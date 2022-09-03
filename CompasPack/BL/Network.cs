
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CompasPac.BL
{
    public static class Network
    {
        public static async Task<bool> IsFastSpeed()
        {
            if (await SpeedTest() >= 1)
                return true;
            else return false;
        }


        public static async Task<double> SpeedTest()
        {
            try
            {
                var watch = new Stopwatch();

                byte[] data;
                using (var client = new WebClient())
                {
                    watch.Start();
                    data = await client.DownloadDataTaskAsync("http://speedtest.tele2.net/10MB.zip");
                    watch.Stop();
                }

                return data.LongLength / 1024 / 1024 / watch.Elapsed.TotalSeconds;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
