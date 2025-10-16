using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service.Win
{
    public class WinSettingsLauncherWin10Plus : WinSettingsLauncherBase
    {
        public override void OpenDefaultProgramsSettings()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "Shell:::{2559a1f7-21d7-11d4-bdaf-00c04f60b9f0}",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                }
            };
            proc.Start();
        }
    }
}
