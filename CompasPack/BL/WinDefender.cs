using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompasPack.BL
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

        public static bool CheckTamperProtectionDisable()
        {
            if (!WinInfo.GetProductName().Contains("Windows 10", StringComparison.InvariantCultureIgnoreCase))
                return true;
            
            var path = "SOFTWARE\\Microsoft\\Windows Defender\\Features";
            var key = "TamperProtection";
            try
            {
                if (WinInfo.GetIs64BitOperatingSystem())
                {
                    RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path);
                    if (rk == null) return false;

                    if (rk.GetValue(key).ToString() == "5")
                        return false;
                    else
                        return true;
                }
                else
                {
                    RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(path);
                    if (rk == null) return false;

                    if (rk.GetValue(key).ToString() == "5")
                        return false;
                    else
                        return true;
                }

            }
            catch { return false; }
        }
    }
}
