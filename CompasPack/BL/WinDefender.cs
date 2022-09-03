using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPac.BL
{
    public static class WinDefender
    {
        public static async Task<string> DisableRealtimeMonitoring()
        {
            if (!WinInfo.GetProductName().Contains("Windows 10", StringComparison.InvariantCultureIgnoreCase))
                return "This Command work onli Windows 10";
            var procinfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                FileName = "powershell.exe",
                Arguments = "Set-MpPreference -DisableRealtimeMonitoring $true",
                CreateNoWindow = true
            };
            var proc =  Process.Start(procinfo);
            return await proc.StandardOutput.ReadToEndAsync();
        }
        
        public static async Task<string> EnableRealtimeMonitoring()
        {
            if (!WinInfo.GetProductName().Contains("Windows 10", StringComparison.InvariantCultureIgnoreCase))
                return "This Command work onli Windows 10";
            var procinfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                FileName = "powershell.exe",
                Arguments = "Set-MpPreference -DisableRealtimeMonitoring $false",
                CreateNoWindow = true
            };
            var proc = Process.Start(procinfo);
            return await proc.StandardOutput.ReadToEndAsync();
        }
        public static async Task<bool> CheckDefenderDisable()
        {
            if (!WinInfo.GetProductName().Contains("Windows 10", StringComparison.InvariantCultureIgnoreCase))
                return false;
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {

                    FileName = "powershell",
                    Arguments = "Get-MpPreference -verbose",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                string line = await proc.StandardOutput.ReadLineAsync();
                if (line.StartsWith(@"DisableRealtimeMonitoring") && line.EndsWith("False"))
                    return false;
            }
            return true;
        }

    }
}
