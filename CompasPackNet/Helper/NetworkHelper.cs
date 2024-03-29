﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace CompasPack.Helper
{
    public static class NetworkHelper
    {
        public static async Task<double> SpeedTest()
        {
            try
            {
                var watch = new Stopwatch();

                watch.Start();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://github.com/Maxim-Paluh/SpeedTest/raw/main/10MB");
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
    }

}
