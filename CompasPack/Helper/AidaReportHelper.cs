﻿using CompasPack.Settings;
using CompasPack.View.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompasPack.Helper
{
    public static class AidaReportHelper
    {
        public static async Task GetAidaReport(string aidaExeFilePath, string reportPath, string type, string rpfPath)
        {
            if (string.IsNullOrWhiteSpace(aidaExeFilePath))
                throw new ArgumentException($"{nameof(aidaExeFilePath)} Is Null Or White Space");

            if (string.IsNullOrWhiteSpace(reportPath))
                throw new ArgumentException($"{nameof(reportPath)} Is Null Or White Space");

            if (string.IsNullOrWhiteSpace(rpfPath))
                throw new ArgumentException($"{nameof(rpfPath)} Is Null Or White Space");


            Process proc = null;
            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                FileName = aidaExeFilePath,
                Arguments = $"/R {reportPath} {type} /CUSTOM {rpfPath}",
                UseShellExecute = false
            };
            try
            {
                proc = Process.Start(StartInfo);
                var timeoutSignal = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                await proc.WaitForExitAsync(timeoutSignal.Token);
            }
            catch (Exception)
            {
                proc?.Kill();
                throw;
            }
        }
    }
}
