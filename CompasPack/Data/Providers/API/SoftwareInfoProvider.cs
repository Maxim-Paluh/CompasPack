using CompasPack.Model.Enum;
using CompasPack.Model.ViewAndViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Data.Providers.API
{
    public class SoftwareInfoProvider
    {
        public static List<string> GetInstallPrograms(WinArchitectureEnum winArchitecture)
        {
            string Programs = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            List<string> programs = new List<string>();

            if (winArchitecture == WinArchitectureEnum.x64)
            {
                programs.AddRange(WinRegistryProvider.GetValues(RegistryHive.LocalMachine, winArchitecture, Programs, "DisplayName"));
                programs.AddRange(WinRegistryProvider.GetValues(RegistryHive.CurrentUser, winArchitecture, Programs, "DisplayName"));
            }
            programs.AddRange(WinRegistryProvider.GetValues(RegistryHive.LocalMachine, WinArchitectureEnum.x86, Programs, "DisplayName"));
            programs.AddRange(WinRegistryProvider.GetValues(RegistryHive.CurrentUser, WinArchitectureEnum.x86, Programs, "DisplayName"));

            return programs;
        }

        /// <summary>
        ///  wmic /namespace:\\root\SecurityCenter2\ path AntivirusProduct get /value
        ///  Працює як в Win7 так і в на старших версіях через різні namespaces
        /// </summary>
        /// <returns></returns>
        public static List<AntivirusInfo> GetAntivirusProducts()
        {
            var result = new List<AntivirusInfo>();
            var namespaces = new[] { @"root\SecurityCenter2", @"root\SecurityCenter" };

            foreach (var ns in namespaces)
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher(ns, "SELECT * FROM AntivirusProduct"))
                    {
                        foreach (ManagementObject antivirus in searcher.Get())
                        {
                            var info = new AntivirusInfo
                            {
                                DisplayName = antivirus["displayName"]?.ToString(),
                                PathToExe = antivirus["pathToSignedProductExe"]?.ToString(),
                                ProductState = antivirus["productState"]?.ToString(),
                            };

                            if (!string.IsNullOrWhiteSpace(info.DisplayName))
                                result.Add(info);
                        }
                        if (result.Count > 0)
                            break; // якщо знайшли — не перевіряємо інший namespace
                    }
                }
                catch
                {
                    // пропускаємо, якщо namespace недоступний
                }
            }

            return result;
        }
        /*public static List<string> GetAntivirusProduct()
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
        */
    }
}
