using CompasPack.Model.Enum;
using CompasPack.Model.Support;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Providers.API
{
    public class WinInfoProvider : IWinInfoProvider
    {
        public WinInfo GetWinInfo()
        {
            var winArchitecture = Environment.Is64BitOperatingSystem ? WinArchitectureEnum.x64 : WinArchitectureEnum.x86;
            var productName = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, winArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            var displayVersion = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, winArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion");
            var editionID = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, winArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
            var currentBuild = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, winArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            var winVer = GetOSVersion();
            return new WinInfo(productName, displayVersion, editionID, currentBuild, winArchitecture, winVer);
        }
        private WinVersionEnum GetOSVersion()
        {
            var version = WinKernelVersionProvider.GetKernelVersion();
            var osName = WinVersionEnum.UnknownOS;

            switch (version.Major)
            {
                case 10:
                    if (version.Build >= 22000)
                        osName = WinVersionEnum.Win_11;
                    else
                        osName = WinVersionEnum.Win_10;
                    break;
                case 6:
                    switch (version.Minor)
                    {
                        case 3:
                            osName = WinVersionEnum.Win_8_1;
                            break;
                        case 2:
                            osName = WinVersionEnum.Win_8;
                            break;
                        case 1:
                            osName = WinVersionEnum.Win_7;
                            break;
                        case 0:
                            osName = WinVersionEnum.Win_Vista;
                            break;
                    }
                    break;
                case 5:
                    switch (version.Minor)
                    {
                        case 1:
                            osName = WinVersionEnum.Win_XP;
                            break;
                    }
                    break;
            }
            return osName;
        }
    }
}
