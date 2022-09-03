
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
        public static async Task<bool> SpeedTestOk()
        {
            try
            {
                var watch = new Stopwatch();

                byte[] data;
                using (var client = new System.Net.WebClient())
                {
                    watch.Start();
                    data = await client.DownloadDataTaskAsync("https://upload.wikimedia.org/wikipedia/commons/f/ff/Pizigani_1367_Chart_10MB.jpg");
                    watch.Stop();
                }

                var speed = data.LongLength / 1024 / 1024 / watch.Elapsed.TotalSeconds;

                if (speed >= 1)
                    return true;
                else return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
