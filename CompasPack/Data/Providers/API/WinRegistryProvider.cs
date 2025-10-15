using CompasPack.Model.Enum;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Providers.API
{
    public static class WinRegistryProvider
    {
        public static string GetValue(RegistryHive registryHive, WinArchitectureEnum winArchitecture, string path, string key)
        {
            try
            {
                if (winArchitecture == WinArchitectureEnum.x64)
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
    }
}
