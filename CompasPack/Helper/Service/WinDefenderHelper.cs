using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CompasPack.Service;
using System.Management;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

namespace CompasPack.Helper
{
    public static class WinDefenderHelper
    {

        //wmic /namespace:\\root\SecurityCenter2\ path AntivirusProduct get /value
        public static List<string> GetAntivirusProduct()
        {
            var antivirusProductList = new List<string>();
            try
            {
                using (var searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntivirusProduct"))
                {
                    foreach (var antivirus in searcher.Get())
                    {
                        string displayName = antivirus["displayName"]?.ToString();
                        antivirusProductList.Add(displayName);
                    }
                }
            }
            catch (Exception)
            {
                return antivirusProductList;
            }
            return antivirusProductList;
        }

        public static async Task<string> DisableRealtimeMonitoring()
        {
            try
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
                var proc = Process.Start(procinfo);
                return await proc.StandardOutput.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
           
        }
        
        public static async Task<string> EnableRealtimeMonitoring()
        {
            try
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
            catch (Exception ex)
            {
                return ex.Message;
            }
           
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
