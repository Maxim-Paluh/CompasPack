using CompasPack.Data.Providers.API;
using CompasPack.Model.Enum;
using CompasPack.Model.ViewAndViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public  class WinDefenderWin10Plus
    {
        public bool IsControlled { get { return true; } }

        private readonly IWinInfoProvider _winInfoProvider;
        public WinDefenderWin10Plus(IWinInfoProvider winInfoProvider)
        {
            _winInfoProvider = winInfoProvider;
        }
        public async Task<AntivirusStatusEnum> DisableRealTimeMonitoring()
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
                await proc.StandardOutput.ReadToEndAsync();
                return AntivirusStatusEnum.Unknown;
            }
            catch (Exception)
            {
                return AntivirusStatusEnum.Error;
            }
            
        }
        
        public async Task<AntivirusStatusEnum> EnableRealTimeMonitoring()
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
                await proc.StandardOutput.ReadToEndAsync();
                return AntivirusStatusEnum.Unknown;
            }
            catch (Exception)
            {
                return AntivirusStatusEnum.Error;
            }
           
        }

        public async Task<AntivirusStatus> CheckStatus()
        {
            AntivirusStatus result = new AntivirusStatus();
            result.TamperProtectionStatus = GetTamperProtectionStatus();
            result.RealtimeMonitoringStatus = await GetRealTimeMonitoringStatus();
            return result;
        }

        public async Task<AntivirusStatusEnum> GetRealTimeMonitoringStatus()
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
                        return AntivirusStatusEnum.Enabled;
                }
                return AntivirusStatusEnum.Disabled;
            }
            catch (Exception)
            {
                return AntivirusStatusEnum.Error;
            }
        }

        public AntivirusStatusEnum GetTamperProtectionStatus()
        {
            try
            {
                var tamperProtectionStatus = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, _winInfoProvider.WinArchitecture, "SOFTWARE\\Microsoft\\Windows Defender\\Features", "TamperProtection");
                if (string.IsNullOrWhiteSpace(tamperProtectionStatus))
                    return AntivirusStatusEnum.Unknown;
                if(tamperProtectionStatus == "5")
                    return AntivirusStatusEnum.Enabled;
                else
                    return AntivirusStatusEnum.Disabled;
            }
            catch { return AntivirusStatusEnum.Error; }
        }

        public void OpenSettings()
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
    }
}
