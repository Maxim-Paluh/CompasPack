using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public static class AidaReportHelper
    {
        public static async Task GetAidaReport(string aidaExeFilePath, string reportPath, string type, string rpfPath, int secondswait)
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
                proc = Process.Start(StartInfo);// TODO
                await Task.Factory.StartNew(() =>
                {
                    if (!proc.WaitForExit(secondswait*1000))
                    {
                        throw new Exception("Aida report time out");
                    }
                });
            }
            catch (Exception)
            {
                try { proc?.Kill(); } catch (Exception) { }
                try { proc?.Close(); } catch (Exception) { }
                throw;
            }
        }
    }
}
