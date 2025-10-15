using System;
using System.Collections.Generic;

using Microsoft.Win32;

using CompasPack.Model.Enum;

namespace CompasPack.Helper.Service
{
    public static class WinInfoHelper
    {

        private static string Programs = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        public static List<string> ListInstallPrograms(WinArchitectureEnum winArchitecture)
        {
            List<string> programs = new List<string>();

            if (winArchitecture == WinArchitectureEnum.x64)
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
