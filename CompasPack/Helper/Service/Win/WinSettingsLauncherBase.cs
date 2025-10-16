using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public class WinSettingsLauncherBase : IWinSettingsLauncher
    {
        public void OpenAUCSettings()
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
       
        public virtual void OpenDefaultProgramsSettings()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "control.exe",
                    Arguments = "/name Microsoft.DefaultPrograms /page pageDefaultProgram",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                }
            };
            proc.Start();
        }

        public void OpenDesktopIconSettings()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "Rundll32.exe",
                    Arguments = "shell32.dll,Control_RunDLL desk.cpl,,0",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Maximized
                }
            };
            proc.Start();
        }

        public void OpenIconSettings()
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
    }
}
