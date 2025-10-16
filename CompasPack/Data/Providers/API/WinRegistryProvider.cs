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
                var view = winArchitecture == WinArchitectureEnum.x64
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (var baseKey = RegistryKey.OpenBaseKey(registryHive, view))
                using (var subKey = baseKey.OpenSubKey(path))
                    return subKey?.GetValue(key)?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<string> GetValues(RegistryHive registryHive, WinArchitectureEnum winArchitecture, string parentPath, string valueName)
        {
            var result = new List<string>();
            try
            {
                var view = winArchitecture == WinArchitectureEnum.x64
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (var baseKey = RegistryKey.OpenBaseKey(registryHive, view))
                using (var parentKey = baseKey.OpenSubKey(parentPath))
                {
                    if (parentKey == null)
                        return result;

                    string[] subKeyNames;
                    try { subKeyNames = parentKey.GetSubKeyNames(); } catch { return result;}

                    foreach (var subKeyName in subKeyNames)
                    {
                        using (var subKey = parentKey.OpenSubKey(subKeyName))
                        {
                            var value = subKey?.GetValue(valueName);
                            if (value != null)
                                result.Add(value.ToString());
                        }
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
