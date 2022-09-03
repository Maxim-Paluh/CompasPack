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
            var proc = new ProcessStartInfo()
            {
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = false,
                FileName = "powershell.exe",
                Arguments = "Set-MpPreference -DisableRealtimeMonitoring $true",
                CreateNoWindow = true
            };
            Process.Start(proc)
                .WaitForExit();

            if(await CheckDefenderDisable())
              return "Set-MpPreference -DisableRealtimeMonitoring $true: OK!!!\n";
            else
              return "Set-MpPreference -DisableRealtimeMonitoring $false: OK!!!\n";
        }
        public static async Task<bool> CheckDefenderDisable()
        {
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
