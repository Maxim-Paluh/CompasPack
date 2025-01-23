using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CompasPack.Service;
using System.Management;
using System.Windows;

namespace CompasPack.Helper
{
    public static class WinDefenderHelper
    {

        //wmic /namespace:\\root\SecurityCenter2\ path AntivirusProduct get /value
        public static bool IsActiveWindowsDefender()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntivirusProduct"))
                {
                    foreach (var antivirus in searcher.Get())
                    {
                        string displayName = antivirus["displayName"]?.ToString();
                        if (!displayName.Contains("Windows Defender", StringComparison.CurrentCultureIgnoreCase) )
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public static async Task<string> DisableRealtimeMonitoring()
        {
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
        public static async Task<WinDefenderEnum> CheckRealtimeMonitoring()
        {
            try
            {
                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
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
                        return WinDefenderEnum.Enabled;
                }
                return WinDefenderEnum.Disabled;
            }
            catch (Exception)
            {
                return WinDefenderEnum.Error;
            }
        }

        public static void OpenWinDefenderSettings()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "windowsdefender://threatsettings",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Maximized
                };
                Process.Start(startInfo);
            }
            catch (Exception)
            {

            }   
        }

        public static WinDefenderEnum CheckTamperProtection()
        {
            RegistryKey rk = null;
            var path = "SOFTWARE\\Microsoft\\Windows Defender\\Features";
            var key = "TamperProtection";

            try
            {
                if (WinInfoHelper.Isx64)
                    rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path);
                else   
                    rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(path);

                if (rk == null)
                    return WinDefenderEnum.Unknown;
                if (rk.GetValue(key).ToString() == "5")
                    return WinDefenderEnum.Enabled;
                else
                    return WinDefenderEnum.Disabled;
            }
            catch { return WinDefenderEnum.Error; }
        }
    }
}
