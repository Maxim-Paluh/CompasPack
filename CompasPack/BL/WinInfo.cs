using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using System.IO;

namespace CompasPac.BL
{
    public static class WinInfo
    {
        private static string Programs = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private static string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        private static string DisplayVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion");
        private static string EditionID = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
        private static string CurrentBuild = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
       
        public static string GetSystemInfo()
        {
            string Type =  Environment.Is64BitOperatingSystem ? "x64" : "x86";

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

        public static bool GetIs64BitOperatingSystem()
        { return Environment.Is64BitOperatingSystem; }
        
        private static string HKLM_GetString(string path, string key)
        {
            try
            {
                if (GetIs64BitOperatingSystem())
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

            if(GetIs64BitOperatingSystem())
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(Programs))
                {
                    foreach (string subkey_name in key.GetSubKeyNames())
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
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var temp = subkey.GetValue("DisplayName");
                        if (temp != null)
                            programs.Add(temp.ToString());
                    }
                }
            }

            //Developers opera very stupid
            var pathOpera = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs\\Opera");
            if (Directory.Exists(pathOpera))
                programs.Add("Opera");

            return programs;
        }

        public static bool IsInstallPrograms(List<string> programs, string? Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;

            if (programs.Where(x => x.Contains(Name, StringComparison.InvariantCultureIgnoreCase)).Count() >= 1)
                return true;
            return false;
        }
    }
}
