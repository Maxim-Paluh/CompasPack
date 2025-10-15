using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompasPack.Model.Enum;
using Microsoft.Win32;

namespace CompasPack.Data.Providers.API
{
    public class WinInfoProvider : IWinInfoProvider
    {
        private string _productName;
        private string _displayVersion;
        private string _editionID;
        private string _currentBuild;
        private WinArchitectureEnum _currentArchitecture;
        private WinVersionEnum _winVersion;

        public string ProductName { get {return _productName; } set { _productName = value; } }
        public string DisplayVersion { get { return _displayVersion; } set { _displayVersion = value; } }
        public string EditionID { get { return _editionID; } set { _editionID = value; } }
        public string CurrentBuild { get { return _currentBuild; } set { _currentBuild = value; } }
        public WinArchitectureEnum WinArchitecture { get { return _currentArchitecture; } set { _currentArchitecture = value; } }
        public WinVersionEnum WinVer { get { return _winVersion; } private set { _winVersion = value; } }
        public WinInfoProvider()
        {
            WinArchitecture = Environment.Is64BitOperatingSystem ? WinArchitectureEnum.x64 : WinArchitectureEnum.x86;
            _productName = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, WinArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            _displayVersion = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, WinArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion");
            _editionID = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, WinArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
            _currentBuild = WinRegistryProvider.GetValue(RegistryHive.LocalMachine, WinArchitecture, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            WinVer = GetOSVersion();
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
        public override string ToString()
        {
            return $"ProductName: {_productName}\n" +
                   $"EditionID: {_editionID}\n" +
                   $"DisplayVersion: {_displayVersion}\n" +
                   $"CurrentBuild: {_currentBuild}\n" +
                   $"Type: {WinArchitecture}\n";
        }


    }
}
