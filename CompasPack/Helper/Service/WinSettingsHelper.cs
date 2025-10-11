using System;
using System.Diagnostics;
using CompasPack.Service;
using CompasPack.View.Service;

namespace CompasPack.Helper
{
    public static class WinSettingsHelper
    {
        public static void OpenDefaultPrograms(IMessageDialogService messageDialogService)
        {
            string argument = null;
            string fileName = null;

            switch (WinInfoHelper.WinVer)
            {
                case WinVerEnum.Win7: case WinVerEnum.Win8: case WinVerEnum.Win8_1:
                    argument = "/name Microsoft.DefaultPrograms /page pageDefaultProgram";
                    fileName = "control.exe";
                    break;
                case WinVerEnum.Win10: case WinVerEnum.Win11:
                    argument = "Shell:::{2559a1f7-21d7-11d4-bdaf-00c04f60b9f0}";
                    fileName = "explorer.exe";
                    break;
                default:
                    break;
            }
            if (argument != null && fileName != null)
            {
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
            else
                messageDialogService.ShowInfoDialog("Не реалізовано для поточної операційної сситеми","Помилка!");
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
