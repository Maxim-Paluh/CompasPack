using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace CompasPack.Helper
{
    public static class WinInfoHelper
    {
        private static string Programs = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private static string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        private static string DisplayVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion");
        private static string EditionID = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
        private static string CurrentBuild = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
        private static bool isx64;
        private static WinVerEnum winVer;

        public static bool Isx64 { get { return isx64; } private set { isx64 = value; } }
        public static WinVerEnum WinVer { get { return winVer; } private set { winVer = value; } }

        static WinInfoHelper()
        {
            Isx64 = Environment.Is64BitOperatingSystem;
            WinVer = GetOSVersion();
        }

        public static string GetSystemInfo()
        {
            string Type = Isx64 ? "x64" : "x86";
            return $"ProductName: {ProductName}\n" +
                   $"EditionID: {EditionID}\n" +
                   $"DisplayVersion: {DisplayVersion}\n" +
                   $"CurrentBuild: {CurrentBuild}\n" +
                   $"Type: {Type}\n";
        }

        public static string GetProductName()
        {
            return HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        }


        private static WinVerEnum GetOSVersion()
        {
            var version = Environment.OSVersion;
            var osName = WinVerEnum.UnknownOS;

            if (version.Platform == PlatformID.Win32NT)
            {
                switch (version.Version.Major)
                {
                    case 10:
                        if (version.Version.Build >= 22000)
                            osName = WinVerEnum.Win11;
                        else
                            osName = WinVerEnum.Win10;
                        break;
                    case 6:
                        switch (version.Version.Minor)
                        {
                            case 3:
                                osName = WinVerEnum.Win8_1;
                                break;
                            case 2:
                                osName = WinVerEnum.Win8;
                                break;
                            case 1:
                                osName = WinVerEnum.Win7;
                                break;
                            case 0:
                                osName = WinVerEnum.WinVista;
                                break;
                        }
                        break;
                    case 5:
                        switch (version.Version.Minor)
                        {
                            case 1:
                                osName = WinVerEnum.WinXP;
                                break;
                        }
                        break;
                }
            }

            return osName;
        }


        private static string HKLM_GetString(string path, string key)
        {
            try
            {
                if (Isx64)
                {
                    RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path);
                    if (rk == null) return "";
                    return (string)rk.GetValue(key);
                }
                else
                {
                    RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(path);
                    if (rk == null) return "";
                    return (string)rk.GetValue(key);
                }

            }
            catch { return ""; }
        }

        public static List<string> ListInstallPrograms()
        {
            List<string> programs = new List<string>();

            if (Isx64)
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(Programs))
                {
                    var subSeyList = new List<string>();
                    if (key != null)
                    {
                        try { subSeyList.AddRange(key.GetSubKeyNames()); } catch (Exception) { }
                    }
                    foreach (string subkey_name in subSeyList)
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            var temp = subkey.GetValue("DisplayName");
                            if (temp != null)
                                programs.Add(temp.ToString());
                        }
                    }
                }
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(Programs))
                {
                    var subSeyList = new List<string>();
                    if (key != null)
                    {
                        try { subSeyList.AddRange(key.GetSubKeyNames()); } catch (Exception) { }
                    }
                    foreach (string subkey_name in subSeyList)
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            var temp = subkey.GetValue("DisplayName");
                            if (temp != null)
                                programs.Add(temp.ToString());
                        }
                    }

                }
            }
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(Programs))
            {
                var subSeyList = new List<string>();
                if (key != null)
                {
                    try { subSeyList.AddRange(key.GetSubKeyNames()); } catch (Exception) { }
                }
                foreach (string subkey_name in subSeyList)
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var temp = subkey.GetValue("DisplayName");
                        if (temp != null)
                            programs.Add(temp.ToString());
                    }
                }

            }
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey(Programs))
            {
                var subSeyList = new List<string>();
                if (key != null)
                {
                    try { subSeyList.AddRange(key.GetSubKeyNames()); } catch (Exception) { }
                }
                foreach (string subkey_name in subSeyList)
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var temp = subkey.GetValue("DisplayName");
                        if (temp != null)
                            programs.Add(temp.ToString());
                    }
                }

            }
            return programs;
        }
    }
}
