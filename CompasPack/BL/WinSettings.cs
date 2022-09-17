using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace CompasPack.BL
{
    public static class WinSettings
    {
        public static void OpenDefaultPrograms()
        {
            string? argument = null;
            string? fileName = null;
            if (WinInfo.GetProductName().Contains("Windows 7", StringComparison.InvariantCultureIgnoreCase))
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
    }
}
