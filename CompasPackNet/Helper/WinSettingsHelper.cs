using System;
using System.Diagnostics;

namespace CompasPack.Helper
{
    public static class WinSettingsHelper
    {
        public static void OpenDefaultPrograms()
        {
            string? argument = null;
            string? fileName = null;
            if (WinInfoHelper.GetProductName().Contains("Windows 7", StringComparison.InvariantCultureIgnoreCase))
            {
                argument = "/name Microsoft.DefaultPrograms /page pageDefaultProgram";
                fileName = "control.exe";
            }
            else
            {
                argument = "Shell:::{2559a1f7-21d7-11d4-bdaf-00c04f60b9f0}";
                fileName = "explorer.exe";
            }
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = argument,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                }
            };
            proc.Start();
        }
        public static void OpenIcon()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "Shell:::{05d7b0f4-2121-4eff-bf6b-ed3f69b894d9}",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Maximized
                }
            };
            proc.Start();
        }
        public static void OpenAUC()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "UserAccountControlSettings.exe",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Maximized
                }
            };
            proc.Start();
        }
        public static void OpenDesktopIconSettings()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "Rundll32.exe",
                    Arguments= "shell32.dll,Control_RunDLL desk.cpl,,0",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Maximized
                }
            };
            proc.Start();
        }

    }
}
